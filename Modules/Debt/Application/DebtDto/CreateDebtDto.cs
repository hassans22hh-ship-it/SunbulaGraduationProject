using DebtDomain.Enums;
using System.ComponentModel.DataAnnotations;

namespace DebtApplication.Dtos
{
    /// DTO for creating a new debt.

    public sealed record CreateDebtDto
    {
        [Required(ErrorMessage = "Creditor name is required")]
        [MinLength(2, ErrorMessage = "Creditor name must be at least 2 characters")]
        [MaxLength(100, ErrorMessage = "Creditor name cannot exceed 100 characters")]
        public string CreditorName { get; init; } = string.Empty;

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, 999999999.99, ErrorMessage = "Amount must be between 0.01 and 999,999,999.99")]
        public decimal Amount { get; init; }

        [Required(ErrorMessage = "Debt type is required")]
        public DebtType DebtType { get; init; }

        [Required(ErrorMessage = "Due date is required")]
        public DateTime DueDate { get; init; }

        [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; init; }
    }
}

