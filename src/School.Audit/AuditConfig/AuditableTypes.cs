using System;
using System.Collections.Generic;
using System.Linq;

namespace School.Audit.AuditConfig
{
    /// <summary>
    /// Данные об аудируемых сущностях.  
    /// </summary>
    /// <remarks>
    /// Класс для внутреннего использования. Впоследствии может измениться.
    /// </remarks>
    public sealed class AuditableTypes
    {
        private readonly HashSet<AuditableEntityMetaData> _items;

        public AuditableTypes()
        {
            _items = new HashSet<AuditableEntityMetaData>();
        }
        
        public bool Contains(Type auditableEntityType)
        {
            return _items.Any(i => i.Type == auditableEntityType);
        }

        public void Add(Type auditableEntityType, string keyPropertyName)
        {
            if (string.IsNullOrWhiteSpace(keyPropertyName))
            {
                throw new ArgumentNullException(nameof(keyPropertyName));
            }
            
            if (Contains(auditableEntityType))
            {
                throw new ArgumentException($"The type {auditableEntityType} already added.");
            }
            
            var keyProperty = auditableEntityType.GetProperty(keyPropertyName);
            if (keyProperty is null)
            {
                throw new ArgumentException($"Property {keyPropertyName} is not contained in type {auditableEntityType}");
            }
            
            if (Contains(auditableEntityType))
            {
                throw new ArgumentException($"The type {auditableEntityType} already added.");
            }

            _items.Add(new AuditableEntityMetaData
            {
                Type = auditableEntityType,
                KeyPropertyName = keyPropertyName
            });
        }

        public AuditableEntityMetaData Get(Type auditableEntityType)
        {
            return _items.First(i => i.Type == auditableEntityType);
        }
        
        public AuditableEntityMetaData Get<T>()
        {
            return _items.First(i => i.Type == typeof(T));
        }
    }
}