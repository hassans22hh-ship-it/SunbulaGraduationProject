namespace TimeTrackingApplication.TimeDtos
{
    public sealed record DailyTransactionDto
    {
        public required Guid Id { get; init; }
        public required Guid UserId { get; init; }
        public required DateOnly Date { get; init; }
        public required int TotalMinutes { get; init; }
        public required decimal TotalCoins { get; init; }
        public required int SessionCount { get; init; }
        public required bool QualifiesForStreak { get; init; }

        // Computed
        public string FormattedTotalTime => $"{TotalMinutes / 60}h {TotalMinutes % 60}m";
        public int UntrackedMinutes => Math.Max(0, 1440 - TotalMinutes); // 24*60 = 1440
    }
}

