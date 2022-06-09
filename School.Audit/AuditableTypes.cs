using System;
using System.Collections.Generic;
using System.Linq;

namespace School.Audit
{
    public class AuditableTypes
    {
        private readonly Dictionary<Type, string[]> _types = new();

        public void Add(Type type, string[] propertyNames)
        {
            _types.Add(type, propertyNames);
        }

        public bool Contains(Type type)
        {
            return _types.Keys.Contains(type);
        }
    }
}