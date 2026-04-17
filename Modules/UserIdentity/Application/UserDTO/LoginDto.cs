using System.ComponentModel.DataAnnotations;

namespace Application.UserDTO
{
    public sealed record LoginDto
    {

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; init; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; init; } = string.Empty;
    }
}
