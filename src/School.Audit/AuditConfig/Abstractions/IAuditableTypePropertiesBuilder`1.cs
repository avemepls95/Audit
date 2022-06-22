using System;
using System.Linq.Expressions;

namespace School.Audit.AuditConfig.Abstractions
{
    /// <summary>
    /// Билдер, добавляющий свойства сущности в аудит.
    /// </summary>
    /// <typeparam name="T">Тип аудируемой сущности</typeparam>
    /// <remarks>
    /// Поддерживаются только те типы, которые реализуют интерфейс <see cref="ISerializable"/>
    /// </remarks>
    public interface IAuditableTypePropertiesBuilder<T> where T : class
    {
        /// <summary>
        /// Добавить свойства в аудит по их именам.
        /// </summary>
        /// <param name="propertyNames">Имена аудируемых сущностей.</param>
        void AddProperties(params string[] propertyNames);

        /// <summary>
        /// Добавить свойство в аудит.
        /// </summary>
        IAuditableTypePropertiesBuilder<T> AddProperty<TProperty>(Expression<Func<T, TProperty>> propertyFunc);

        /// <summary>
        /// Добавить свойства в аудит.
        /// </summary>
        IAuditableTypePropertiesBuilder<T> AddProperties(params Expression<Func<T, object>>[] propertyFunctions);

        /// <summary>
        /// Добавить все свойства сущности в аудит.
        /// </summary>
        void AddAllProperties();
    }
}