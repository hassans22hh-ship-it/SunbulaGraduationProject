namespace TimeTrackingApplication.TimeDtos
{
    ///Combined daily summary including sessions and transaction totals.

    public sealed record DailySummaryDto
    {
        public required DateOnly Date { get; init; }
        public required int TotalMinutes { get; init; }
        public required decimal TotalCoins { get; init; }
        public required int SessionCount { get; init; }
        public required int UntrackedMinutes { get; init; }
        public required int CurrentStreak { get; init; }
        public required bool QualifiesForStreak { get; init; }
        public required IEnumerable<TimeSessionDto> Sessions { get; init; }
    }
}

