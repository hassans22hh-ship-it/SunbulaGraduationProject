using SharedKernel;

namespace FinanceDomain.Events
{
    ///Raised when a new wallet is created
    public sealed record WalletCreatedEvent(
    Guid WalletId,
    Guid UserId,
    decimal OpeningBalance,
    string Currency) : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

    }
}
