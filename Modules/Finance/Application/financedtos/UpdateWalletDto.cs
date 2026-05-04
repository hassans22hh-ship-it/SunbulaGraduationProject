using FinanceDomain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FinanceApplication.financedtos
{
    public sealed record UpdateWalletDto
    {
        [Required(ErrorMessage = "Wallet name is required.")]
        [MinLength(2, ErrorMessage = "Name must be at least 2 characters.")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; init; } = string.Empty;

        [Required]
        public WalletType Type { get; init; }

        /// <summary>
        /// Optional: Direct balance adjustment.
        /// </summary>
        public decimal? Balance { get; init; }
    }
}
