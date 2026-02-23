using SharedKernel;

namespace TimeTrackingDomain.Events
{
    /// Raised when a user earns (or loses) coins from a session.
    /// Consumed by UserIdentity module to update coin balance.

    public sealed record CoinsEarnedEvent(
    Guid UserId,
    decimal CoinsAmount,
    Guid SessionId) : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
