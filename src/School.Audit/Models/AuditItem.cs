using System;

namespace School.Audit.Models
{
    /// <summary>
    /// Элемента аудита.
    /// </summary>
    public sealed class AuditItem
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Тип аудируемой объекта.
        /// </summary>
        public string TargetType { get; set; }

        /// <summary>
        /// Значение ключа аудируемого объекта.
        /// </summary>
        public string KeyPropertyValue { get; set; }

        /// <summary>
        /// Имя измененного свойства.
        /// </summary>
        public string ChangedPropertyName { get; set; }

        /// <summary>
        /// Старое значение измененного свойства.
        /// </summary>
        public string OldValue { get; set; }
    
        /// <summary>
        /// Новое значение измененного свойства.
        /// </summary>
        public string NewValue { get; set; }

        /// <inheritdoc cref="School.Audit.Models.OperationType"/>
        public OperationType OperationType { get; set; }

        /// <summary>
        /// Дата и время операции.
        /// </summary>
        public DateTimeOffset Date { get; set; }
    }
}
