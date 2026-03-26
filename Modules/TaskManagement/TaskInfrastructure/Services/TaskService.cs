using Application.ServiceAbstraction;
using Application.TaskManagmentDTOS;
using Domain.Entities.TaskManagement;
using TaskDomain.Contracts;
using TaskDomain.Entities.TaskManagement.Enums;
using TaskDomain.Exceptions;

namespace TaskInfrastructure.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TaskService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TaskDto> GetByIdAsync(Guid id,Guid UserId, CancellationToken cancellationToken = default)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id, cancellationToken);
            if (task == null)
                throw new TaskNotFoundException(id);

            return MapToDto(task);
        }

        public async Task<TaskDto> GetByIdWithDetailsAsync(Guid id,Guid UserId, CancellationToken cancellationToken = default)
        {
            var task = await _unitOfWork.Tasks.GetByIdWithCategoriesAsync(id, cancellationToken);
            if (task == null)
                throw new TaskNotFoundException(id);

            return MapToDtoWithDetails(task);
        }

        public async Task<PagedResultDto<TaskDto>> GetAllByUserIdAsync(Guid userId, PaginationParams pagination, CancellationToken cancellationToken = default)
        {
            var (tasks, totalCount) = await _unitOfWork.Tasks.GetByUserIdAsync(userId, pagination.PageNumber, pagination.PageSize, cancellationToken);
            return new PagedResultDto<TaskDto>
            {
                Items = tasks.Select(MapToDto),
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        public async Task<PagedResultDto<TaskDto>> GetActiveByUserIdAsync(Guid userId, PaginationParams pagination, CancellationToken cancellationToken = default)
        {
            var (tasks, totalCount) = await _unitOfWork.Tasks.GetActiveByUserIdAsync(userId, pagination.PageNumber, pagination.PageSize, cancellationToken);
            return new PagedResultDto<TaskDto>
            {
                Items = tasks.Select(MapToDto),
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        public async Task<PagedResultDto<TaskDto>> GetArchivedByUserIdAsync(Guid userId, PaginationParams pagination, CancellationToken cancellationToken = default)
        {
            var (tasks, totalCount) = await _unitOfWork.Tasks.GetArchivedByUserIdAsync(userId, pagination.PageNumber, pagination.PageSize, cancellationToken);
            return new PagedResultDto<TaskDto>
            {
                Items = tasks.Select(MapToDto),
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        public async Task<PagedResultDto<TaskDto>> GetByFolderIdAsync(Guid folderId, Guid userId, PaginationParams pagination, CancellationToken cancellationToken = default)
        {
            // Verify folder belongs to user
            var folder = await _unitOfWork.Folders.GetByIdAsync(folderId, cancellationToken);
            if (folder == null)
                throw new FolderNotFoundException(folderId);

            if (folder.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to access this folder");

            var (tasks, totalCount) = await _unitOfWork.Tasks.GetByFolderIdAsync(folderId, pagination.PageNumber, pagination.PageSize, cancellationToken);
            return new PagedResultDto<TaskDto>
            {
                Items = tasks.Select(MapToDto),
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        public async Task<PagedResultDto<TaskDto>> GetByCategoryIdAsync(Guid categoryId, Guid userId, PaginationParams pagination, CancellationToken cancellationToken = default)
        {
            // Verify category belongs to user
            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId, cancellationToken);
            if (category == null)
                throw new CategoryNotFoundException(categoryId);

            if (category.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to access this category");

            var (tasks, totalCount) = await _unitOfWork.Tasks.GetByCategoryIdAsync(categoryId, pagination.PageNumber, pagination.PageSize, cancellationToken);
            return new PagedResultDto<TaskDto>
            {
                Items = tasks.Select(MapToDto),
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        public async Task<PagedResultDto<TaskDto>> GetByBehaviorTypeAsync(
            Guid userId,
            BehaviorCategory behaviorType,
            PaginationParams pagination,
            CancellationToken cancellationToken = default)
        {
            var (tasks, totalCount) = await _unitOfWork.Tasks.GetByBehaviorTypeAsync(userId, behaviorType, pagination.PageNumber, pagination.PageSize, cancellationToken);
            return new PagedResultDto<TaskDto>
            {
                Items = tasks.Select(MapToDto),
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        public async Task<PagedResultDto<TaskDto>> SearchAsync(string query, Guid userId, PaginationParams pagination, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new PagedResultDto<TaskDto> { PageNumber = pagination.PageNumber, PageSize = pagination.PageSize };

            var (tasks, totalCount) = await _unitOfWork.Tasks.SearchByTitleAsync(userId, query, pagination.PageNumber, pagination.PageSize, cancellationToken);
            return new PagedResultDto<TaskDto>
            {
                Items = tasks.Select(MapToDto),
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        public async Task<IEnumerable<TaskDto>> GetRecentAsync(Guid userId, int count = 10, CancellationToken cancellationToken = default)
        {
            var tasks = await _unitOfWork.Tasks.GetRecentAsync(userId, count, cancellationToken);
            return tasks.Select(MapToDto);
        }

        public async Task<TaskDto> CreateAsync(CreateTaskDto dto, Guid userId, CancellationToken cancellationToken = default)
        {
            // Check if title exists
            var titleExists = await _unitOfWork.Tasks.TitleExistsAsync(userId, dto.Title, cancellationToken: cancellationToken);
            if (titleExists)
                throw new InvalidOperationException($"Task with title '{dto.Title}' already exists");

            // Verify folder if provided
            if (dto.FolderId.HasValue)
            {
                var folder = await _unitOfWork.Folders.GetByIdAsync(dto.FolderId.Value, cancellationToken);
                if (folder == null)
                    throw new FolderNotFoundException(dto.FolderId.Value);

                if (folder.UserId != userId)
                    throw new UnauthorizedAccessException("You don't have permission to use this folder");
            }

            // Create using domain factory
            var task = TaskItem.Create(
                userId,
                dto.Title,
                dto.Emoji,
                dto.Color,
                dto.BehaviorType,
                dto.FolderId);

            // Add categories if provided
            foreach (var categoryId in dto.CategoryIds)
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(categoryId, cancellationToken);
                if (category == null)
                    throw new CategoryNotFoundException(categoryId);

                if (category.UserId != userId)
                    throw new UnauthorizedAccessException("You don't have permission to use this category");

                task.AddCategory(categoryId);
            }

            await _unitOfWork.Tasks.AddAsync(task, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDto(task);
        }

        public async Task<TaskDto> DuplicateAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var task = await _unitOfWork.Tasks.GetByIdWithCategoriesAsync(id, cancellationToken);
            if (task == null)
                throw new TaskNotFoundException(id);

            if (task.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to duplicate this task");

            var duplicate = task.Duplicate();

            // Ensure title uniqueness
            var baseTitle = duplicate.Title;
            var finalTitle = baseTitle;
            var counter = 1;
            while (await _unitOfWork.Tasks.TitleExistsAsync(userId, finalTitle, cancellationToken: cancellationToken))
            {
                finalTitle = $"{baseTitle} ({counter++})";
                if (finalTitle.Length > 200) finalTitle = finalTitle.Substring(0, 200);
                // Note: using reflection or Update to change title if needed, 
                // but Duplicate() uses private constructor. We can just Update it.
            }
            
            if (finalTitle != baseTitle)
            {
                duplicate.Update(finalTitle, duplicate.Emoji, duplicate.Color.Value, duplicate.BehaviorType, duplicate.FolderId);
            }

            await _unitOfWork.Tasks.AddAsync(duplicate, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDtoWithDetails(duplicate);
        }

        public async Task<TaskDto> UpdateAsync(Guid id, UpdateTaskDto dto, Guid userId, CancellationToken cancellationToken = default)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id, cancellationToken);
            if (task == null)
                throw new TaskNotFoundException(id);

            if (task.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to update this task");

            // Check if new title conflicts with existing
            if (task.Title != dto.Title)
            {
                var titleExists = await _unitOfWork.Tasks.TitleExistsAsync(userId, dto.Title, id, cancellationToken);
                if (titleExists)
                    throw new InvalidOperationException($"Task with title '{dto.Title}' already exists");
            }

            // Verify folder if provided
            if (dto.FolderId.HasValue)
            {
                var folder = await _unitOfWork.Folders.GetByIdAsync(dto.FolderId.Value, cancellationToken);
                if (folder == null)
                    throw new FolderNotFoundException(dto.FolderId.Value);

                if (folder.UserId != userId)
                    throw new UnauthorizedAccessException("You don't have permission to use this folder");
            }

            task.Update(dto.Title, dto.Emoji, dto.Color, dto.BehaviorType, dto.FolderId);

            _unitOfWork.Tasks.Update(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDto(task);
        }

        public async Task ArchiveAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id, cancellationToken);
            if (task == null)
                throw new TaskNotFoundException(id);

            if (task.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to archive this task");

            task.Archive();

            _unitOfWork.Tasks.Update(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task UnarchiveAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id, cancellationToken);
            if (task == null)
                throw new TaskNotFoundException(id);

            if (task.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to unarchive this task");

            task.Unarchive();

            _unitOfWork.Tasks.Update(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task CompleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id, cancellationToken);
            if (task == null)
                throw new TaskNotFoundException(id);

            if (task.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to complete this task");

            task.Complete();

            _unitOfWork.Tasks.Update(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id, cancellationToken);
            if (task == null)
                throw new TaskNotFoundException(id);

            if (task.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to delete this task");

            _unitOfWork.Tasks.Delete(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task AddCategoryAsync(Guid taskId, Guid categoryId, Guid userId, CancellationToken cancellationToken = default)
        {
            var task = await _unitOfWork.Tasks.GetByIdWithCategoriesAsync(taskId, cancellationToken);
            if (task == null)
                throw new TaskNotFoundException(taskId);

            if (task.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to modify this task");

            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId, cancellationToken);
            if (category == null)
                throw new CategoryNotFoundException(categoryId);

            if (category.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to use this category");

            task.AddCategory(categoryId);

            _unitOfWork.Tasks.Update(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveCategoryAsync(Guid taskId, Guid categoryId, Guid userId, CancellationToken cancellationToken = default)
        {
            var task = await _unitOfWork.Tasks.GetByIdWithCategoriesAsync(taskId, cancellationToken);
            if (task == null)
                throw new TaskNotFoundException(taskId);

            if (task.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to modify this task");

            task.RemoveCategory(categoryId);

            _unitOfWork.Tasks.Update(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteUserDataAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            // Delete tasks first (they may refer to folders/categories)
            await _unitOfWork.Tasks.HardDeleteByUserIdAsync(userId, cancellationToken);
            await _unitOfWork.Folders.HardDeleteByUserIdAsync(userId, cancellationToken);
            await _unitOfWork.Categories.HardDeleteByUserIdAsync(userId, cancellationToken);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private static TaskDto MapToDto(TaskItem task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Emoji = task.Emoji,
                Color = task.Color.Value,
                BehaviorType = task.BehaviorType,
                FolderId = task.FolderId,
                Status = task.Status,
                IsArchived = task.IsArchived,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            };
        }
        private static TaskDto MapToDtoWithDetails(TaskItem task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Emoji = task.Emoji,
                Color = task.Color.Value,
                BehaviorType = task.BehaviorType,
                FolderId = task.FolderId,
                Status = task.Status,
                IsArchived = task.IsArchived,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                Categories = task.TaskCategories.Select(tc => new CategoryDto
                {
                    Id = tc.Category.Id,
                    Name = tc.Category.Name,
                    Color = tc.Category.Color.Value,
                    CreatedAt = tc.Category.CreatedAt
                }).ToList()
            };
        }
    }
}
