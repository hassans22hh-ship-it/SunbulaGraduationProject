using MediatR;
using SharedKernel;
using System;

namespace SharedKernel
{
    /// <summary>
    /// Integration event published when a user earns (or loses) coins from a session.
    /// Consumed by UserIdentity module to update the user's total coin balance.
    /// </summary>
    public record CoinsEarnedEvent(
        Guid UserId,
        int CoinsAmount,
        Guid SessionId) : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
