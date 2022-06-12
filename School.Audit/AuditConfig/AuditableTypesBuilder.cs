using System;
using School.Audit.Abstractions;

namespace School.Audit.AuditConfig
{
    internal class AuditableTypesBuilder : IAuditableTypesBuilder
    {
        public AuditableTypes Types { get; } = new();

        public IAuditableTypePropertiesBuilder<T> Add<T>(string keyPropertyName) where T : IAuditable
        {
            if (string.IsNullOrWhiteSpace(keyPropertyName))
            {
                throw new ArgumentNullException(nameof(keyPropertyName));
            }
            
            var type = typeof(T);
            if (Types.Contains(type))
            {
                throw new ArgumentException($"The type {typeof(T)} already added.");
            }

            var keyProperty = type.GetProperty(keyPropertyName);
            if (keyProperty is null)
            {
                throw new ArgumentException($"Property {keyPropertyName} is not contained in type {typeof(T)}");
            }
            
            Types.Add(type, keyPropertyName);
            
            return new AuditableTypePropertiesBuilder<T>(Types.Get(type));
        }
    }
}