using System;
using System.Linq.Expressions;

namespace School.Audit.AuditConfig.Abstractions
{
    public interface IAuditableTypesBuilder
    {
        IAuditableTypePropertiesBuilder<T> Add<T>(string keyPropertyName) where T : class;
        IAuditableTypePropertiesBuilder<T> Add<T>(Expression<Func<T, object>> keyFunc) where T : class;
    }
}