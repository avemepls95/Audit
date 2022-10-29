using System;

namespace School.Audit.AuditConfig
{
    /// <summary>
    /// Резолвер для типа аудируемого объекта.
    /// </summary>
    internal static class PropertyTypeAllowResolver
    {
        public const string ErrorMessage = "Auditable property type should be serializable.";
        
        /// <summary>
        /// Разрешено ли аудировать указанный тип.
        /// </summary>
        public static bool IsValid(Type type)
        {
            return type.IsSerializable;
        }
    }
}