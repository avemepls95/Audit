using School.Audit.Models;

namespace School.Audit.Abstractions
{
    public interface IChangeTracker
    {
        bool IsAnyChanges();

        AuditItem[] GetChanges();
    }
}