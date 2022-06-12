using System.Threading;
using System.Threading.Tasks;
using School.Audit.Abstractions;

namespace School.Audit
{
    internal sealed class UnitOfWork : IUnitOfWork
    {
        private readonly IChangeTracker _changeTracker;
        private readonly ISaveChangesCommand _saveChangesCommand;

        public UnitOfWork(
            IChangeTracker changeTracker,
            ISaveChangesCommand saveChangesCommand)
        {
            _changeTracker = changeTracker;
            _saveChangesCommand = saveChangesCommand;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            var isAnyChanges = _changeTracker.IsAnyChanges();
            if (!isAnyChanges)
            {
                return;
            }

            var changes = _changeTracker.GetChanges();
            
            await _saveChangesCommand.ExecuteAsync(changes, cancellationToken);
        }
    }
}