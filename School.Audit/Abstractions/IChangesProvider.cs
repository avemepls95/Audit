using School.Audit.Models;

namespace School.Audit.Abstractions
{
    /// <summary>
    /// Трекер изменений.
    /// </summary>
    public interface IChangesProvider
    {
        /// <summary>
        /// Проверяет наличие изменений.
        /// </summary>
        bool IsAnyChanges();

        /// <summary>
        /// Возвращает все отслеживаемые изменения.
        /// </summary>
        /// <returns>Список изменений <see cref="AuditItem"/></returns>
        AuditItem[] GetChanges();
    }
}