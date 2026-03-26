using Domain.Entities.TaskManagement;
using SharedKernel;
using TaskDomain.Entities.TaskManagement.Enums;

namespace TaskDomain.Contracts
{
    public interface ITaskRepository : IRepository<TaskItem>
    {
        Task<TaskItem?> GetByIdWithCategoriesAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<TaskItem?> GetByIdWithFolderAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<(IEnumerable<TaskItem> Items, int TotalCount)> GetByUserIdAsync(
            Guid userId,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default);

        Task<(IEnumerable<TaskItem> Items, int TotalCount)> GetActiveByUserIdAsync(
            Guid userId,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default);

        Task<(IEnumerable<TaskItem> Items, int TotalCount)> GetArchivedByUserIdAsync(
            Guid userId,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default);

        Task<(IEnumerable<TaskItem> Items, int TotalCount)> GetByFolderIdAsync(
            Guid folderId,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default);

        Task<(IEnumerable<TaskItem> Items, int TotalCount)> GetByCategoryIdAsync(
            Guid categoryId,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default);

        Task<(IEnumerable<TaskItem> Items, int TotalCount)> GetByBehaviorTypeAsync(
            Guid userId,
            BehaviorCategory behaviorType,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default);

        Task<bool> TitleExistsAsync(
            Guid userId,
            string title,
            Guid? excludeTaskId = null,
            CancellationToken cancellationToken = default);

        Task<(IEnumerable<TaskItem> Items, int TotalCount)> SearchByTitleAsync(
            Guid userId,
            string query,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<TaskItem>> GetRecentAsync(
            Guid userId,
            int count = 10,
            CancellationToken cancellationToken = default);

        Task<int> CountByFolderIdAsync(
            Guid folderId,
            CancellationToken cancellationToken = default);

        Task HardDeleteByUserIdAsync(Guid userId, CancellationToken ct = default);
    }
}