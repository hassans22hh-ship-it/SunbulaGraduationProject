using System.ComponentModel.DataAnnotations;
using TaskDomain.Entities.TaskManagement.Enums;
namespace Application.TaskManagmentDTOS
{
    public sealed record UpdateTaskDto
    {
        [Required(ErrorMessage = "Title is required")]
        [MinLength(2, ErrorMessage = "Title must be at least 2 characters")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; init; } = string.Empty;

        [MaxLength(10, ErrorMessage = "Emoji cannot exceed 10 characters")]
        public string? Emoji { get; init; }

        [Required(ErrorMessage = "Color is required")]
        [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Color must be a valid hex color")]
        public string Color { get; init; } = string.Empty;

        [Required(ErrorMessage = "Behavior type is required")]
        public BehaviorCategory BehaviorType { get; init; }

        public Guid? FolderId { get; init; }
    }
}
