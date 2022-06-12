using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using School.Audit.Abstractions;
using School.Audit.AuditConfig;
using School.Audit.Models;

namespace School.Audit.Db.Implementation
{
    internal class ChangeTracker<TDbContext> : IChangeTracker where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;
        private readonly AuditableTypes _auditableTypes;

        public ChangeTracker(TDbContext dbContext, AuditableTypes auditableTypes)
        {
            _dbContext = dbContext;
            _auditableTypes = auditableTypes;
        }

        public bool IsAnyChanges()
        {
            return _dbContext.ChangeTracker.HasChanges();
        }

        public AuditItem[] GetChanges()
        {
            if (!IsAnyChanges())
            {
                return Array.Empty<AuditItem>();
            }

            if (!_dbContext.ChangeTracker.AutoDetectChangesEnabled)
            {
                _dbContext.ChangeTracker.DetectChanges();
            }
            
            var changedAuditableEntries = _dbContext.ChangeTracker.Entries()
                .Where(e => e.Entity is IAuditable)
                .ToArray();

            var auditItems = new List<AuditItem>();
            foreach (var changedEntry in changedAuditableEntries)
            {
                var auditableType = changedEntry.Entity.GetType();
                if (!_auditableTypes.TryGet(auditableType, out var auditableEntityMetaData))
                {
                    continue;
                }
                
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

                var changedProperties = changedEntry.OriginalValues.Properties
                    .Where(p => auditableEntityMetaData.PropertyNames.Contains(p.Name))
                    .ToArray();
                
                foreach (var changedProperty in changedProperties)
                {
                    var oldValue = changedEntry.Property(changedProperty.Name).OriginalValue;
                    var newValue = changedEntry.Property(changedProperty.Name).CurrentValue;
                    if (oldValue.Equals(newValue))
                    {
                        continue;
                    }
                    
                    var newAuditItem = new AuditItem
                    {
                        TargetType = auditableType.ToString(),
                        KeyPropertyValue = keyPropertyValue.ToString(),
                        ChangedPropertyName = changedProperty.Name,
                        NewValue = newValue.ToString(),
                        Date = DateTimeOffset.Now
                    };
                    auditItems.Add(newAuditItem);
                    
                    if (operationType == OperationType.Create)
                    {
                        newAuditItem.OperationType = OperationType.Create;
                    }
                    else
                    {
                        newAuditItem.OldValue = oldValue.ToString();
                        newAuditItem.OperationType = OperationType.Modify;
                    }
                }
            }
            
            return auditItems.ToArray();
        }
    }
}