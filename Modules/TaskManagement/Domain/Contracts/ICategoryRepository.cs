using SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;
using TaskDomain.Entities.TaskManagement;

namespace TaskDomain.Contracts
{
    public interface ICategoryRepository:IRepository<Category>
    {
        Task<Category?> GetByIdWithTasksAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Category>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task HardDeleteByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<Category?> GetByNameAsync(Guid userId, string name, CancellationToken cancellationToken = default);
        Task<bool> NameExistsAsync(Guid userId, string name, Guid? excludeCategoryId = null, CancellationToken cancellationToken = default);
    }
}
