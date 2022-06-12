using System;
using System.Collections.Generic;
using System.Linq;

namespace School.Audit.AuditConfig
{
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

        public bool TryGet(Type auditableEntityType, out AuditableEntityMetaData auditableEntityMetaData)
        {
            if (!Contains(auditableEntityType))
            {
                auditableEntityMetaData = null;
                return false;
            }

            auditableEntityMetaData = Get(auditableEntityType);
            return true;
        }
    }
}