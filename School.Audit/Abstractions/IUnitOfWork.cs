using System.Threading;
using System.Threading.Tasks;

namespace School.Audit.Abstractions
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}