using FinanceDomain.Enums;
using FinanceDomain.Events;
using SharedKernel;

namespace FinanceDomain.Entities
{
    public sealed class FinancialTransaction: BaseEntity
    {
        private FinancialTransaction() { }

        private FinancialTransaction(
            Guid id,
            Guid userId,
            Guid walletId,
            Guid? destinationWalletId,
            Guid? financialCategoryId,
            TransactionType type,
            decimal amount,
            string currency,
            string? description,
            DateTime transactionDate)
            : base(id)
        {
            UserId = userId;
            WalletId = walletId;
            DestinationWalletId = destinationWalletId;
            FinancialCategoryId = financialCategoryId;
            Type = type;
            Amount = amount;
            Currency = currency;
            Description = description;
            TransactionDate = transactionDate;
        }

        // ─── Properties ─────────────────────────────────────────────────────────

        /// <summary>Owner user ID.</summary>
        public Guid UserId { get; private set; }

        /// <summary>Source wallet ID.</summary>
        public Guid WalletId { get; private set; }

        /// <summary>Destination wallet ID (only for Transfer type).</summary>
        public Guid? DestinationWalletId { get; private set; }

        /// <summary>Optional financial category ID.</summary>
        public Guid? FinancialCategoryId { get; private set; }

        /// <summary>Transaction type: Income | Expense | Transfer.</summary>
        public TransactionType Type { get; private set; }

        /// <summary>Transaction amount (always positive; direction determined by Type).</summary>
        public decimal Amount { get; private set; }

        /// <summary>Currency code (e.g., "SAR", "USD").</summary>
        public string Currency { get; private set; } = string.Empty;

        /// <summary>Optional user-provided note.</summary>
        public string? Description { get; private set; }

        /// <summary>Date and time the transaction occurred.</summary>
        public DateTime TransactionDate { get; private set; }

        // Navigation properties (within Finance module only)
        public Wallet? Wallet { get; private set; }
        public FinancialCategory? FinancialCategory { get; private set; }

        // ─── Factory ────────────────────────────────────────────────────────────

        /// <summary>Creates a new financial transaction.</summary>
        public static FinancialTransaction Create(
            Guid userId,
            Guid walletId,
            Guid? destinationWalletId,
            Guid? financialCategoryId,
            TransactionType type,
            decimal amount,
            string currency,
            string? description,
            DateTime? transactionDate = null)
        {
            ValidateAmount(amount);

            if (type == TransactionType.Transfer && destinationWalletId is null)
                throw new ArgumentException("Transfer transactions require a destination wallet.", nameof(destinationWalletId));

            if (type == TransactionType.Transfer && destinationWalletId == walletId)
                throw new ArgumentException("Source and destination wallets must be different.", nameof(destinationWalletId));

            var transaction = new FinancialTransaction(
                Guid.NewGuid(),
                userId,
                walletId,
                type == TransactionType.Transfer ? destinationWalletId : null,
                financialCategoryId,
                type,
                amount,
                currency.ToUpperInvariant(),
                description?.Trim(),
                transactionDate?.ToUniversalTime() ?? DateTime.UtcNow);

            transaction.RaiseDomainEvent(new TransactionCreatedEvent(
                transaction.Id, userId, walletId, type, amount, currency));

            return transaction;
        }

        // ─── Domain Methods ─────────────────────────────────────────────────────

        /// <summary>Updates editable fields of this transaction.</summary>
        public void Update(
            Guid? financialCategoryId,
            decimal amount,
            string? description,
            DateTime transactionDate)
        {
            ValidateAmount(amount);

            FinancialCategoryId = financialCategoryId;
            Amount = amount;
            Description = description?.Trim();
            TransactionDate = transactionDate.ToUniversalTime();

            MarkAsUpdated();
        }

        // ─── Validation ─────────────────────────────────────────────────────────

        private static void ValidateAmount(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Transaction amount must be greater than zero.", nameof(amount));
        }
    }
}
