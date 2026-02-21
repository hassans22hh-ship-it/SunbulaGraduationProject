using FinanceDomain.Enums;
using SharedKernel;

namespace FinanceDomain.Events
{
    /// <summary>
    /// Raised when a financial transaction is recorded.
    /// </summary>
    public  sealed record TransactionCreatedEvent(
    Guid TransactionId,
    Guid UserId,
    Guid WalletId,
    TransactionType Type,
    decimal Amount,
    string Currency) : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
