using System.Threading;
using System.Threading.Tasks;
using School.Audit.Abstractions;

namespace School.Audit
{
    internal sealed class UnitOfWork : IUnitOfWork
    {
        private readonly IChangesProvider _changesProvider;
        private readonly ISaveChangesCommand _saveChangesCommand;

        public UnitOfWork(
            IChangesProvider changesProvider,
            ISaveChangesCommand saveChangesCommand)
        {
            _changesProvider = changesProvider;
            _saveChangesCommand = saveChangesCommand;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            var isAnyChanges = _changesProvider.IsAnyChanges();
            if (!isAnyChanges)
            {
                return;
            }

            var changes = _changesProvider.GetChanges();
            
            await _saveChangesCommand.ExecuteAsync(changes, cancellationToken);
        }
    }
}