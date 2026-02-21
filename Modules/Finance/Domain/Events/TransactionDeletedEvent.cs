using FinanceDomain.Enums;
using SharedKernel;

namespace FinanceDomain.Events
{
    /// <summary>
    /// Raised when a financial transaction is deleted (to reverse balance).
    /// </summary>
    public class TransactionDeletedEvent(
    Guid TransactionId,
    Guid UserId,
    Guid WalletId,
    TransactionType Type,
    decimal Amount) : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
