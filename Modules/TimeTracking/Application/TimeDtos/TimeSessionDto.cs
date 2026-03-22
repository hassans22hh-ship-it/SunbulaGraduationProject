using TimeTrackingDomain.Enums;

namespace TimeTrackingApplication.TimeDtos
{
    ///Response DTO for a completed or active time session.

    public sealed record TimeSessionDto
    {
        public required Guid Id { get; init; }
        public required Guid UserId { get; init; }
        public required Guid TaskId { get; init; }
        public required DateTime StartTime { get; init; }
        public DateTime? EndTime { get; init; }
        public required int DurationMinutes { get; init; }
        public required decimal CoinsEarned { get; init; }
        public required BehaviorType BehaviorType { get; init; }
        public required string BehaviorTypeName { get; init; }
        public required bool IsActive { get; init; }
        public required bool ManuallyAdded { get; init; }
        public string? Notes { get; init; }
        public required DateTime CreatedAt { get; init; }

        // Computed for display
        public string FormattedDuration => $"{DurationMinutes / 60}h {DurationMinutes % 60}m";
    }
}

