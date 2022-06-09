using System;

namespace School.Audit
{
    public interface IAuditableTypesBuilder
    {
        public AuditableTypes Types { get; set; }

        IAuditableTypesBuilder Add<T>(params Func<T, object>[] getPropertyFuncs) where T : IAuditable;
    }
}