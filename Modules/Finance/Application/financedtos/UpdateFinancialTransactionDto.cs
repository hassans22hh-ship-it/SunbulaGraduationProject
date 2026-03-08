using System.ComponentModel.DataAnnotations;

namespace FinanceApplication.financedtos
{
    public sealed record UpdateFinancialTransactionDto
    {
        public Guid? FinancialCategoryId { get; init; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; init; }

        [MaxLength(500)]
        public string? Description { get; init; }

        [Required]
        public DateTime TransactionDate { get; init; }
    }
}
