namespace FinanceApplication.financedtos
{
    public sealed record FinanceSummaryDto
    {
        public required decimal TotalBalance { get; init; }
        public required string Currency { get; init; }
        public required decimal TotalIncome { get; init; }
        public required decimal TotalExpenses { get; init; }
        public required int WalletCount { get; init; }
        public required int TransactionCount { get; init; }
    }
}
