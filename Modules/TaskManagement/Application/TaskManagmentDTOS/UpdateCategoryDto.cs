using System.ComponentModel.DataAnnotations;

namespace Application.TaskManagmentDTOS
{
    public sealed record UpdateCategoryDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MinLength(2, ErrorMessage = "Name must be at least 2 characters")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; init; } = string.Empty;

        [Required(ErrorMessage = "Color is required")]
        [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Color must be a valid hex color")]
        public string Color { get; init; } = string.Empty;
    }
}
