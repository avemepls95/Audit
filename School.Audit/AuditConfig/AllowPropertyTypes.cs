using System;

namespace School.Audit.AuditConfig
{
    internal static class AllowPropertyTypes
    {
        public const string ErrorMessage = "Auditable property type should be serializable.";
        
        public static bool IsValid(Type type)
        {
            return type.IsSerializable;
        }
    }
}