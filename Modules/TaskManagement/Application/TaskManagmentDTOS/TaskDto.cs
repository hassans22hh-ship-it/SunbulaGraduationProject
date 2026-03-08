
using TaskDomain.Entities.TaskManagement.Enums;

namespace Application.TaskManagmentDTOS
{
    public sealed record TaskDto
    {
        public required Guid Id { get; init; }
        public required string Title { get; init; }
        public string? Emoji { get; init; }
        public required string Color { get; init; }
        public required BehaviorCategory BehaviorType { get; init; }
        public Guid? FolderId { get; init; }
        public required TaskDomain.Entities.TaskManagement.Enums.TaskStatus Status { get; init; }
        public required bool IsArchived { get; init; }
        public required DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public List<CategoryDto> Categories { get; init; } = new();
        public FolderDto? Folder { get; init; }
    }
}
