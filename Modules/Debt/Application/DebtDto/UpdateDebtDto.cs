using System.ComponentModel.DataAnnotations;

namespace DebtApplication.Dtos
{
    /// DTO for updating a debt.

    public sealed record UpdateDebtDto
    {
        [Required(ErrorMessage = "Creditor name is required")]
        [MinLength(2, ErrorMessage = "Creditor name must be at least 2 characters")]
        [MaxLength(100, ErrorMessage = "Creditor name cannot exceed 100 characters")]
        public string CreditorName { get; init; } = string.Empty;

        [Required(ErrorMessage = "Due date is required")]
        public DateTime DueDate { get; init; }

        [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; init; }
    }
}

