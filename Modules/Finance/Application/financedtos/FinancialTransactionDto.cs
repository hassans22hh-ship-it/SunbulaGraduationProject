using FinanceDomain.Enums;

namespace FinanceApplication.financedtos
{
    public sealed record FinancialTransactionDto
    {
        public required Guid Id { get; init; }
        public required Guid WalletId { get; init; }
        public string? WalletName { get; init; }
        public Guid? DestinationWalletId { get; init; }
        public string? DestinationWalletName { get; init; }
        public Guid? FinancialCategoryId { get; init; }
        public string? CategoryName { get; init; }
        public string? CategoryIcon { get; init; }
        public required TransactionType Type { get; init; }
        public required decimal Amount { get; init; }
        public required string Currency { get; init; }
        public string? Description { get; init; }
        public required DateTime TransactionDate { get; init; }
        public required DateTime CreatedAt { get; init; }
    }
}
