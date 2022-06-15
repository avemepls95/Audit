﻿using System;
using System.Collections.Generic;
using System.Linq;

using School.Audit.AuditConfig.Abstractions;

namespace School.Audit.AuditConfig
{
    internal class AuditableTypePropertiesBuilder<T> : IAuditableTypePropertiesBuilder<T> where T : class
    {
        private readonly Type[] _allowPropertyTypes = 
        {
            typeof(string),
            typeof(Guid),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(Enum)
        };

        private readonly AuditableEntityMetaData _auditableEntityMetaData;
        
        public AuditableTypePropertiesBuilder(AuditableEntityMetaData auditableEntityMetaData)
        {
            _auditableEntityMetaData = auditableEntityMetaData;
        }
        
        public void AddProperties(params string[] propertyNames)
        {
            if (propertyNames.Length != propertyNames.Distinct().Count())
            {
                throw new ArgumentException("Duplicate property names found.");
            }
            
            var allObjectProperties = typeof(T).GetProperties();
            var objectPropertyNames = allObjectProperties.Select(p => p.Name);
            var invalidNames = propertyNames.Where(name => !objectPropertyNames.Contains(name)).ToArray();
            if (invalidNames.Any())
            {
                throw new ArgumentException($"Invalid property names: {string.Join(", ", invalidNames)}");
            }

            foreach (var propertyName in propertyNames)
            {
                var propertyType = allObjectProperties.First(p => p.Name == propertyName).PropertyType;
                if (propertyType.IsPrimitive || _allowPropertyTypes.Contains(propertyType))
                {
                    continue;
                }

                throw new ArgumentException("Only primitive types of properties are supported.");
            }

            var allPropertyNames = _auditableEntityMetaData.PropertyNames?.ToList() ?? new List<string>();
            allPropertyNames.AddRange(propertyNames);
            
            if (allPropertyNames.Contains(_auditableEntityMetaData.KeyPropertyName))
            {
                allPropertyNames.Remove(_auditableEntityMetaData.KeyPropertyName);
            }
            
            _auditableEntityMetaData.PropertyNames = allPropertyNames.ToArray();
        }
    }
}