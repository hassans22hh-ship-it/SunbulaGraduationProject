using FinanceDomain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FinanceApplication.financedtos
{
    public sealed record CreateFinancialTransactionDto
    {
        [Required(ErrorMessage = "Wallet is required.")]
        public Guid WalletId { get; init; }

        /// <summary>Required only for Transfer type.</summary>
        public Guid? DestinationWalletId { get; init; }

        public Guid? FinancialCategoryId { get; init; }

        [Required(ErrorMessage = "Transaction type is required.")]
        public TransactionType Type { get; init; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; init; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; init; }

        public DateTime? TransactionDate { get; init; }
    }
}
