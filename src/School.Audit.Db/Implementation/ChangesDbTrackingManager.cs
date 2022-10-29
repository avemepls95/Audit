using Microsoft.EntityFrameworkCore;
using School.Audit.Abstractions;
using School.Audit.Db.External;
using School.Audit.Models;

namespace School.Audit.Db.Implementation
{
    /// <inheritdoc />
    internal class ChangesDbTrackingManager<TDbContext> : IChangesDbTrackingManager where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;
        private readonly IChangesProvider _changesProvider;

        public ChangesDbTrackingManager(TDbContext dbContext, IChangesProvider changesProvider)
        {
            _dbContext = dbContext;
            _changesProvider = changesProvider;
        }

        /// <inheritdoc />
        public void AddChanges()
        {
            var isAnyChanges = _changesProvider.IsAnyChanges();
            if (!isAnyChanges)
            {
                return;
            }
            
            var changes = _changesProvider.GetChanges();
            _dbContext.Set<AuditItem>().AddRange(changes);
        }
    }
}