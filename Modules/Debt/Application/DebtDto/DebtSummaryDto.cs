namespace DebtApplication.Dtos
{
    /// Summary of user's debt statistics.

    public sealed record DebtSummaryDto
    {
        public required decimal TotalPayable { get; init; }
        public required decimal TotalReceivable { get; init; }
        public required decimal TotalRemainingPayable { get; init; }
        public required decimal TotalRemainingReceivable { get; init; }
        public required int TotalDebtsCount { get; init; }
        public required int UnpaidDebtsCount { get; init; }
        public required int OverdueDebtsCount { get; init; }
    }
}

