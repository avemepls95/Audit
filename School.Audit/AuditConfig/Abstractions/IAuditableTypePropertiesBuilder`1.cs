namespace School.Audit.AuditConfig.Abstractions
{
    public interface IAuditableTypePropertiesBuilder<T> where T : class
    {
        void AddProperties(params string[] propertyNames);
    }
}