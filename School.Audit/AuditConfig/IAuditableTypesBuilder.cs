using School.Audit.Abstractions;

namespace School.Audit.AuditConfig
{
    public interface IAuditableTypesBuilder
    {
        IAuditableTypePropertiesBuilder<T> Add<T>(string keyPropertyName) where T : IAuditable;
    }
}