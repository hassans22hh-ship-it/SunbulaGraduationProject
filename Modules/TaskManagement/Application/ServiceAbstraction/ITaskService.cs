using Application.TaskManagmentDTOS;
using TaskDomain.Entities.TaskManagement.Enums;

namespace Application.ServiceAbstraction
{
    public interface ITaskService
    {
        Task<TaskDto> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task<TaskDto> GetByIdWithDetailsAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task<PagedResultDto<TaskDto>> GetAllByUserIdAsync(Guid userId, PaginationParams pagination, CancellationToken cancellationToken = default);
        Task<PagedResultDto<TaskDto>> GetActiveByUserIdAsync(Guid userId, PaginationParams pagination, CancellationToken cancellationToken = default);
        Task<PagedResultDto<TaskDto>> GetArchivedByUserIdAsync(Guid userId, PaginationParams pagination, CancellationToken cancellationToken = default);
        Task<PagedResultDto<TaskDto>> GetByFolderIdAsync(Guid folderId, Guid userId, PaginationParams pagination, CancellationToken cancellationToken = default);
        Task<PagedResultDto<TaskDto>> GetByCategoryIdAsync(Guid categoryId, Guid userId, PaginationParams pagination, CancellationToken cancellationToken = default);
        Task<PagedResultDto<TaskDto>> GetByBehaviorTypeAsync(Guid userId, BehaviorCategory behaviorType, PaginationParams pagination, CancellationToken cancellationToken = default);
        Task<PagedResultDto<TaskDto>> SearchAsync(string query, Guid userId, PaginationParams pagination, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskDto>> GetRecentAsync(Guid userId, int count = 10, CancellationToken cancellationToken = default);
        Task<TaskDto> CreateAsync(CreateTaskDto dto, Guid userId, CancellationToken cancellationToken = default);
        Task<TaskDto> DuplicateAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task<TaskDto> UpdateAsync(Guid id, UpdateTaskDto dto, Guid userId, CancellationToken cancellationToken = default);
        Task ArchiveAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task UnarchiveAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task CompleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task ReactivateAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task AddCategoryAsync(Guid taskId, Guid categoryId, Guid userId, CancellationToken cancellationToken = default);
        Task RemoveCategoryAsync(Guid taskId, Guid categoryId, Guid userId, CancellationToken cancellationToken = default);
        Task DeleteUserDataAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
