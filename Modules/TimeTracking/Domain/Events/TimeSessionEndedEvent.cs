using SharedKernel;

namespace TimeTrackingDomain.Events
{
    public class TimeSessionEndedEvent(
    Guid SessionId,
    Guid UserId,
    decimal CoinsEarned,
    int DurationMinutes) : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
