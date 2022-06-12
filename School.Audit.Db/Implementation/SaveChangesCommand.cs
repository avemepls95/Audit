using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using School.Audit.Abstractions;
using School.Audit.Models;

namespace School.Audit.Db.Implementation
{
    internal class SaveChangesCommand<TDbContext> : ISaveChangesCommand where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;

        public SaveChangesCommand(TDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task ExecuteAsync(AuditItem[] auditItems, CancellationToken cancellationToken)
        {
            _dbContext.Set<AuditItem>().AddRange(auditItems);
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}