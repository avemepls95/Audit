using System.Threading;
using System.Threading.Tasks;
using School.Audit.Models;

namespace School.Audit.Abstractions
{
    public interface ISaveChangesCommand
    {
        Task ExecuteAsync(AuditItem[] auditItems, CancellationToken cancellationToken = default);
    }
}