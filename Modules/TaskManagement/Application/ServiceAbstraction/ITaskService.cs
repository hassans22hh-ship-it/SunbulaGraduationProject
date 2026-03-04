using Application.TaskManagmentDTOS;
using TaskDomain.Entities.TaskManagement.Enums;

namespace Application.ServiceAbstraction
{
    public interface ITaskService
    {
        Task<TaskDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<TaskDto> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskDto>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskDto>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskDto>> GetArchivedByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskDto>> GetByFolderIdAsync(Guid folderId, Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskDto>> GetByCategoryIdAsync(Guid categoryId, Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskDto>> GetByBehaviorTypeAsync(Guid userId, BehaviorCategory behaviorType, CancellationToken cancellationToken = default);
        Task<TaskDto> CreateAsync(CreateTaskDto dto, Guid userId, CancellationToken cancellationToken = default);
        Task<TaskDto> UpdateAsync(Guid id, UpdateTaskDto dto, Guid userId, CancellationToken cancellationToken = default);
        Task ArchiveAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task UnarchiveAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task CompleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task AddCategoryAsync(Guid taskId, Guid categoryId, Guid userId, CancellationToken cancellationToken = default);
        Task RemoveCategoryAsync(Guid taskId, Guid categoryId, Guid userId, CancellationToken cancellationToken = default);
    }
}
