using SharedKernel;

namespace Domain.Entities.ValueObjects
{
    public sealed record CoinBalanceChangedEvent(
    Guid UserId,
    int PreviousBalance,
    int NewBalance,
    int Change,
    string Reason) : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
