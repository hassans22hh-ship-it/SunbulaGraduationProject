using SharedKernel;

namespace TimeTrackingDomain.Events
{
    public class TimeSessionPausedEvent(
        Guid sessionId,
        Guid userId) : IDomainEvent
    {
        public Guid SessionId { get; } = sessionId;
        public Guid UserId { get; } = userId;
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
