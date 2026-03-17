using SharedKernel;

namespace TimeTrackingDomain.Events
{
    public sealed record  TimeSessionStartedEvent(
    Guid SessionId,
    Guid UserId,
    Guid TaskId) : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
