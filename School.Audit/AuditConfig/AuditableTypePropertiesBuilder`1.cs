using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

                var supportedTypesAsString = string.Join(", ", _allowPropertyTypes.Select(t => t.ToString()));
                throw new ArgumentException($"There are invalid type(s). Supported: {supportedTypesAsString}.");
            }

            var allPropertyNames = _auditableEntityMetaData.PropertyNames?.ToList() ?? new List<string>();
            allPropertyNames.AddRange(propertyNames);
            
            if (allPropertyNames.Contains(_auditableEntityMetaData.KeyPropertyName))
            {
                throw new ArgumentException("Key property name passed.");
            }
            
            _auditableEntityMetaData.PropertyNames = allPropertyNames.ToArray();
        }
        
        public IAuditableTypePropertiesBuilder<T> AddProperty<TProperty>(Expression<Func<T, TProperty>> propertyFunc)
        {
            if (propertyFunc.Body is not MemberExpression memberExpression)
            {
                throw new ArgumentException("Invalid type of property.");
            }

            AddPropertyCore(memberExpression);
            
            return this;
        }
        
        public IAuditableTypePropertiesBuilder<T> AddProperties(params Expression<Func<T, object>>[] propertyFunctions)
        {
            foreach (var propertyFunc in propertyFunctions)
            {
                var memberExpression = ((propertyFunc.Body as UnaryExpression)?.Operand as MemberExpression)
                                       ?? propertyFunc.Body as MemberExpression;
                
                if (memberExpression is null)
                {
                    throw new ArgumentException("Invalid type of key.");
                }

                AddPropertyCore(memberExpression);
            }
            
            return this;
        }
        
        public void AddAllProperties()
        {
            var allPropertyNames = typeof(T).GetProperties()
                .Where(p => _allowPropertyTypes.Contains(p.PropertyType))
                .Select(p => p.Name)
                .ToArray();

            AddProperties(allPropertyNames);
        }

        private void AddPropertyCore(MemberExpression memberExpression)
        {
            var propertyName = memberExpression.Member.Name;
            if (propertyName.Equals(_auditableEntityMetaData.KeyPropertyName, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Key property name passed.");
            }
            
            if (_auditableEntityMetaData.PropertyNames.Contains(propertyName))
            {
                throw new ArgumentException($"Name of property `{propertyName}` already auditable.");
            }

            var allPropertyNames = _auditableEntityMetaData.PropertyNames?.ToList() ?? new List<string>();
            allPropertyNames.Add(propertyName);
            _auditableEntityMetaData.PropertyNames = allPropertyNames.ToArray();
        }
    }
}