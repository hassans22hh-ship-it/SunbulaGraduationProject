
using System.ComponentModel.DataAnnotations;

namespace Application.UserDTO
{
    public sealed record  RefreshTokenDto
    {
        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; init; } = string.Empty;

    }
}
