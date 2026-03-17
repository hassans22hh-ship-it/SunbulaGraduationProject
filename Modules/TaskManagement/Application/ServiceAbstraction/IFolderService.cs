using Application.TaskManagmentDTOS;

namespace Application.ServiceAbstraction
{
    public interface IFolderService
    {
        Task<FolderDto> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task<FolderDto> GetByIdWithTasksAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<FolderDto>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<FolderDto> CreateAsync(CreateFolderDto dto, Guid userId, CancellationToken cancellationToken = default);
        Task<FolderDto> UpdateAsync(Guid id, UpdateFolderDto dto, Guid userId, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    }
}
