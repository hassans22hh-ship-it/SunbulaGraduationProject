using System.ComponentModel.DataAnnotations;

namespace Application.UserDTO
{
    public sealed record UpdateProfileDto
    {
        [Required(ErrorMessage = "First name is required")]
        [MinLength(2)]
        [MaxLength(50)]
        public string FirstName { get; init; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [MinLength(2)]
        [MaxLength(50)]
        public string LastName { get; init; } = string.Empty;

        [Phone]
        public string? Phone { get; init; }
    }
}
