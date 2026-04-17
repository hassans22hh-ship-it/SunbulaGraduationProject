using System.ComponentModel.DataAnnotations;

namespace Application.UserDTO
{
    public sealed  record RegisterDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; init; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must contain uppercase, lowercase, digit, and special character")]
        public string Password { get; init; } = string.Empty;

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; init; } = string.Empty;

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
