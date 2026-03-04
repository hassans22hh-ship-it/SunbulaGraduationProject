using System;
using System.Collections.Generic;
using System.Text;

namespace TaskDomain.Contracts
{
    public interface IUnitOfWork:IDisposable
    {
        ITaskRepository Tasks { get; }
        ICategoryRepository Categories { get; }
        IFolderRepository Folders { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
