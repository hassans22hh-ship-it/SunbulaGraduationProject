using Application.ServiceAbstraction;
using Application.TaskManagmentDTOS;
using Domain.Entities.TaskManagement;
using TaskDomain.Contracts;
using TaskDomain.Exceptions;

namespace TaskInfrastructure.Services
{
    public class FolderService : IFolderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FolderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<FolderDto> GetByIdAsync(Guid id,Guid UserId, CancellationToken cancellationToken = default)
        {
            var folder = await _unitOfWork.Folders.GetByIdAsync(id, cancellationToken);
            if (folder == null)
                throw new FolderNotFoundException(id);

            return MapToDto(folder);
        }

        public async Task<FolderDto> GetByIdWithTasksAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var folder = await _unitOfWork.Folders.GetByIdWithTasksAsync(id, cancellationToken);
            if (folder == null)
                throw new FolderNotFoundException(id);

            if (folder.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to access this folder");

            return MapToDtoWithTasks(folder);
        }

        public async Task<IEnumerable<FolderDto>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var folders = await _unitOfWork.Folders.GetByUserIdAsync(userId, cancellationToken);
            return folders.Select(MapToDto);
        }

        public async Task<FolderDto> CreateAsync(CreateFolderDto dto, Guid userId, CancellationToken cancellationToken = default)
        {
            var nameExists = await _unitOfWork.Folders.NameExistsAsync(userId, dto.Name, cancellationToken: cancellationToken);
            if (nameExists)
                throw new InvalidOperationException($"Folder with name '{dto.Name}' already exists");

            var folder = Folder.Create(userId, dto.Name, dto.Color);

            await _unitOfWork.Folders.AddAsync(folder, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDto(folder);
        }

        public async Task<FolderDto> UpdateAsync(Guid id, UpdateFolderDto dto, Guid userId, CancellationToken cancellationToken = default)
        {
            var folder = await _unitOfWork.Folders.GetByIdAsync(id, cancellationToken);
            if (folder == null)
                throw new FolderNotFoundException(id);

            if (folder.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to update this folder");

            if (folder.Name != dto.Name)
            {
                var nameExists = await _unitOfWork.Folders.NameExistsAsync(userId, dto.Name, id, cancellationToken);
                if (nameExists)
                    throw new InvalidOperationException($"Folder with name '{dto.Name}' already exists");
            }

            folder.Update(dto.Name, dto.Color);

            _unitOfWork.Folders.Update(folder);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDto(folder);
        }

        public async Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var folder = await _unitOfWork.Folders.GetByIdWithTasksAsync(id, cancellationToken);
            if (folder == null)
                throw new FolderNotFoundException(id);

            if (folder.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to delete this folder");

            // Business rule: Cannot delete folder with tasks
            if (!folder.CanBeDeleted())
                throw new FolderNotEmptyException(id, folder.GetTaskCount());

            _unitOfWork.Folders.Delete(folder);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private static FolderDto MapToDto(Folder folder)
        {
            return new FolderDto
            {
                Id = folder.Id,
                Name = folder.Name,
                Color = folder.Color.Value,
                CreatedAt = folder.CreatedAt,
                TaskCount = folder.GetTaskCount()
            };
        }

        private static FolderDto MapToDtoWithTasks(Folder folder)
        {
            return new FolderDto
            {
                Id = folder.Id,
                Name = folder.Name,
                Color = folder.Color.Value,
                CreatedAt = folder.CreatedAt,
                TaskCount = folder.GetTaskCount(),
                Tasks = folder.Tasks.Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Emoji = t.Emoji,
                    Color = t.Color.Value,
                    BehaviorType = t.BehaviorType,
                    FolderId = t.FolderId,
                    Status = t.Status,
                    IsArchived = t.IsArchived,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                }).ToList()
            };
        }
    }
}

