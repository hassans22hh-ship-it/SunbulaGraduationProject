using FinanceDomain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FinanceApplication.financedtos
{
    public sealed record CreateWalletDto
    {
        [Required(ErrorMessage = "Wallet name is required.")]
        [MinLength(2, ErrorMessage = "Name must be at least 2 characters.")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; init; } = string.Empty;

        [Required(ErrorMessage = "Wallet type is required.")]
        public WalletType Type { get; init; }

        [Required(ErrorMessage = "Currency is required.")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be a 3-letter ISO code.")]
        public string Currency { get; init; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Opening balance cannot be negative.")]
        public decimal OpeningBalance { get; init; } = 0;
    }
}
