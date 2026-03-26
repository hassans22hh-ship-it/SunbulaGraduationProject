using System.ComponentModel.DataAnnotations;

namespace Application.UserDTO
{
    public class DeleteAccountDto
    {
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
