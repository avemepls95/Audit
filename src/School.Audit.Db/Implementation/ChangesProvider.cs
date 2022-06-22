using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using School.Audit.Abstractions;
using School.Audit.AuditConfig;
using School.Audit.Models;

namespace School.Audit.Db.Implementation
{
    internal class ChangesProvider<TDbContext> : IChangesProvider where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;
        private readonly AuditableTypes _auditableTypes;

        public ChangesProvider(TDbContext dbContext, AuditableTypes auditableTypes)
        {
            _dbContext = dbContext;
            _auditableTypes = auditableTypes;
        }

        public bool IsAnyChanges()
        {
            if (!_dbContext.ChangeTracker.AutoDetectChangesEnabled)
            {
                _dbContext.ChangeTracker.DetectChanges();
            }
            
            var changedAuditableEntriesCount = _dbContext.ChangeTracker.Entries()
                .Count(e => _auditableTypes.Contains(e.Entity.GetType()));

            return changedAuditableEntriesCount != 0;
        }

        public AuditItem[] GetChanges()
        {
            if (!IsAnyChanges())
            {
                return Array.Empty<AuditItem>();
            }
            
            var changedAuditableEntries = _dbContext.ChangeTracker.Entries()
                .Where(e => _auditableTypes.Contains(e.Entity.GetType()))
                .ToArray();

            var auditItems = new List<AuditItem>();
            foreach (var changedEntry in changedAuditableEntries)
            {
                var auditableType = changedEntry.Entity.GetType();
                var auditableEntityMetaData = _auditableTypes.Get(auditableType);
                
                var operationType = changedEntry.State switch
                {
                    EntityState.Added => OperationType.Create,
                    EntityState.Modified => OperationType.Modify,
                    EntityState.Deleted => OperationType.Delete,
                    _ => OperationType.None
                };

                var keyPropertyValue = changedEntry.Property(auditableEntityMetaData.KeyPropertyName).CurrentValue;
                
                if (operationType == OperationType.Delete)
                {
                    auditItems.Add(new AuditItem
                    {
                        TargetType = auditableType.ToString(),
                        KeyPropertyValue = keyPropertyValue.ToString(),
                        OperationType = OperationType.Delete,
                        Date = DateTimeOffset.Now
                    });

                    continue;
                }

                var auditItemsFromChangedProperties = GetAuditItemsFromChangedProperties(
                    changedEntry,
                    auditableEntityMetaData,
                    auditableType,
                    keyPropertyValue,
                    operationType);
                
                auditItems.AddRange(auditItemsFromChangedProperties);
            }
            
            return auditItems.ToArray();
        }

        private static AuditItem[] GetAuditItemsFromChangedProperties(
            EntityEntry changedEntry,
            AuditableEntityMetaData auditableEntityMetaData,
            Type auditableType,
            object keyPropertyValue,
            OperationType operationType)
        {
            var auditableProperties = changedEntry.OriginalValues.Properties
                .Where(p => auditableEntityMetaData.PropertyNames.Contains(p.Name))
                .ToArray();

            var now = DateTimeOffset.Now;
            
            var result = new List<AuditItem>();
            foreach (var auditableProperty in auditableProperties)
            {
                var oldValue = changedEntry.Property(auditableProperty.Name).OriginalValue;
                var newValue = changedEntry.Property(auditableProperty.Name).CurrentValue;
                if (ValuesAreEqual(oldValue, newValue) && operationType != OperationType.Create)
                {
                    continue;
                }
                    
                var newAuditItem = new AuditItem
                {
                    TargetType = auditableType.ToString(),
                    KeyPropertyValue = keyPropertyValue.ToString(),
                    ChangedPropertyName = auditableProperty.Name,
                    NewValue = newValue?.ToString(),
                    Date = now
                };
                result.Add(newAuditItem);
                    
                if (operationType == OperationType.Create)
                {
                    newAuditItem.OperationType = OperationType.Create;
                }
                else
                {
                    newAuditItem.OldValue = oldValue?.ToString();
                    newAuditItem.OperationType = OperationType.Modify;
                }
            }

            return result.ToArray();
        }

        private static bool ValuesAreEqual(object firstValue, object secondValue)
        {
            if (firstValue == null && secondValue == null)
            {
                return true;
            }

            if (firstValue == null || secondValue == null)
            {
                return false;
            }

            return firstValue.Equals(secondValue);
        }
    }
}