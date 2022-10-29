using System;
using System.Linq.Expressions;
using School.Audit.AuditConfig.Abstractions;

namespace School.Audit.AuditConfig
{
    /// <inheritdoc />
    internal class AuditableTypesBuilder : IAuditableTypesBuilder
    {
        public AuditableTypes Types { get; } = new();

        /// <inheritdoc />
        public IAuditableTypePropertiesBuilder<T> Add<T>(string keyPropertyName) where T : class
        {
            var type = typeof(T);

            Types.Add(type, keyPropertyName);

            return new AuditableTypePropertiesBuilder<T>(Types.Get(type));
        }

        /// <inheritdoc />
        public IAuditableTypePropertiesBuilder<T> Add<T>(Expression<Func<T, object>> keyFunc) where T : class
        {
            if (keyFunc.Body is not UnaryExpression { Operand: MemberExpression memberExpression })
            {
                throw new ArgumentException("Invalid type of key.");
            }

            var keyPropertyName = memberExpression.Member.Name;
            var type = typeof(T);
            Types.Add(type, keyPropertyName);
            
            return new AuditableTypePropertiesBuilder<T>(Types.Get(type));
        }
    }
}