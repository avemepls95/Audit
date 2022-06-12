using School.Audit.Abstractions;

namespace School.Audit.AuditConfig.Abstractions
{
    public interface IAuditableTypePropertiesBuilder<T> where T : IAuditable
    {
        void AddProperties(params string[] propertyNames);
    }
}