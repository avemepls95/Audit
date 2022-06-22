using System;
using System.Linq.Expressions;

namespace School.Audit.AuditConfig.Abstractions
{
    /// <summary>
    /// Билдер, добавляющий в аудит сущность с указанием ее идентификатора.
    /// </summary>
    public interface IAuditableTypesBuilder
    {
        /// <summary>
        /// Добавить сущность в аудит.
        /// </summary>
        /// <param name="keyPropertyName">Имя свойства-ключа.</param>
        /// <typeparam name="T">Тип аудируемой сущности.</typeparam>
        /// <returns>Билдер аудируемых свойств.</returns>
        IAuditableTypePropertiesBuilder<T> Add<T>(string keyPropertyName) where T : class;

        /// <summary>
        /// Добавить сущность в аудит.
        /// </summary>
        /// <param name="keyFunc">Выражение, определяющее свойство-клю.ч</param>
        /// <typeparam name="T">Тип аудируемой сущности.</typeparam>
        /// <returns>Билдер аудируемых свойств.</returns>
        IAuditableTypePropertiesBuilder<T> Add<T>(Expression<Func<T, object>> keyFunc) where T : class;
    }
}