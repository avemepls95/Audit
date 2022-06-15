namespace School.Audit.AuditConfig.Abstractions
{
    public interface IAuditableTypesBuilder
    {
        IAuditableTypePropertiesBuilder<T> Add<T>(string keyPropertyName) where T : class;
    }
}