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

        Task<IEnumerable<TaskItem>> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<TaskItem>> GetActiveByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<TaskItem>> GetArchivedByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<TaskItem>> GetByFolderIdAsync(
            Guid folderId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<TaskItem>> GetByCategoryIdAsync(
            Guid categoryId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<TaskItem>> GetByBehaviorTypeAsync(
            Guid userId,
            BehaviorCategory behaviorType,
            CancellationToken cancellationToken = default);

        Task<bool> TitleExistsAsync(
            Guid userId,
            string title,
            Guid? excludeTaskId = null,
            CancellationToken cancellationToken = default);

        Task<int> CountByFolderIdAsync(
            Guid folderId,
            CancellationToken cancellationToken = default);

        Task HardDeleteByUserIdAsync(Guid userId, CancellationToken ct = default);
    }
}