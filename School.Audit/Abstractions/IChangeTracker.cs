using School.Audit.Models;

namespace School.Audit.Abstractions
{
    /// <summary>
    /// Auditable entities change tracker.
    /// </summary>
    public interface IChangeTracker
    {
        /// <summary>
        /// Check whether any changes on auditable entities.
        /// </summary>
        bool IsAnyChanges();

        /// <summary>
        /// Returns changes in auditable entities.
        /// </summary>
        /// <returns>Array of <see cref="AuditItem"/></returns>
        AuditItem[] GetChanges();
    }
}