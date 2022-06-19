using System;
using System.Linq.Expressions;

namespace School.Audit.AuditConfig.Abstractions
{
    public interface IAuditableTypePropertiesBuilder<T> where T : class
    {
        void AddProperties(params string[] propertyNames);

        IAuditableTypePropertiesBuilder<T> AddProperty<TProperty>(Expression<Func<T, TProperty>> propertyFunc);

        IAuditableTypePropertiesBuilder<T> AddProperties(params Expression<Func<T, object>>[] propertyFunctions);

        void AddAllProperties();
    }
}