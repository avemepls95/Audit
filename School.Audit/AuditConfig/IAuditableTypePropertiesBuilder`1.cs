using School.Audit.Abstractions;

namespace School.Audit.AuditConfig
{
    public interface IAuditableTypePropertiesBuilder<T> where T : IAuditable
    {
        void AddProperties(params string[] propertyNames);
    }
}