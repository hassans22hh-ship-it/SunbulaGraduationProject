using Domain.Entities.TaskManagement;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace TaskDomain.Contracts
{
    public interface IFolderRepository:IRepository<Folder>
    {
        Task<Folder?> GetByIdWithTasksAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Folder>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Folder?> GetByNameAsync(Guid userId, string name, CancellationToken cancellationToken = default);
        Task<bool> NameExistsAsync(Guid userId, string name, Guid? excludeFolderId = null, CancellationToken cancellationToken = default);
        Task<int> GetTaskCountAsync(Guid folderId, CancellationToken cancellationToken = default);
    }
}
