using FinanceDomain.Enums;
using FinanceDomain.Events;
using FinanceDomain.ValueObjects;
using SharedKernel;

namespace FinanceDomain.Entities
{
    /// Wallet aggregate root.
    /// Represents a user's financial wallet (cash, bank, or card).
    /// Owns the balance and validates all financial movements.
    public class Wallet:BaseEntity
    {
        private readonly List<FinancialTransaction> _transactions = new();

        // ─── Private constructors ───────────────────────────────────────────────

        private Wallet() { }

        private Wallet(Guid id, Guid userId, string name, WalletType type, Money balance)
            : base(id)
        {
            UserId = userId;
            Name = name;
            Type = type;
            Balance = balance;
        }

        // ─── Properties ─────────────────────────────────────────────────────────

        /// <summary>Owner of the wallet (cross-module reference by ID only).</summary>
        public Guid UserId { get; private set; }

        /// <summary>Display name (e.g., "Al Ahli Bank", "Cash").</summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>Wallet type: Cash | BankAccount | Card.</summary>
        public WalletType Type { get; private set; }

        /// <summary>Current balance (amount + currency as value object).</summary>
        public Money Balance { get; private set; } = null!;

        /// <summary>Transactions belonging to this wallet (read-only projection).</summary>
        public IReadOnlyCollection<FinancialTransaction> Transactions => _transactions.AsReadOnly();

        // ─── Factory ────────────────────────────────────────────────────────────

        /// <summary>
        /// Creates a new Wallet with an opening balance.
        /// </summary>
        public static Wallet Create(
            Guid userId,
            string name,
            WalletType walletType,
            decimal openingBalance,
            string currency)
        {
            ValidateName(name);

            var balance = Money.Create(openingBalance, currency);
            var wallet = new Wallet(Guid.NewGuid(), userId, name, walletType, balance);

            wallet.RaiseDomainEvent(new WalletCreatedEvent(wallet.Id, userId, openingBalance, currency));

            return wallet;
        }

        // ─── Domain Methods ─────────────────────────────────────────────────────

        /// <summary>Updates wallet name and/or type.</summary>
        public void Update(string name, WalletType type)
        {
            ValidateName(name);
            Name = name;
            Type = type;
            MarkAsUpdated();
        }

        /// <summary>
        /// Applies a transaction amount to this wallet's balance.
        /// Income → adds; Expense → subtracts; Transfer handled at service level.
        /// </summary>
        public void ApplyTransaction(decimal amount, TransactionType transactionType)
        {
            Balance = transactionType switch
            {
                TransactionType.Income => Balance.Add(amount),
                TransactionType.Expense => Balance.Subtract(amount),
                TransactionType.Transfer => Balance.Subtract(amount),
                _ => throw new InvalidOperationException($"Unknown transaction type: {transactionType}")
            };
            MarkAsUpdated();
        }

        /// <summary>
        /// Reverses a previously applied transaction (used when deleting/undoing).
        /// </summary>
        public void ReverseTransaction(decimal amount, TransactionType transactionType)
        {
            Balance = transactionType switch
            {
                TransactionType.Income => Balance.Subtract(amount),
                TransactionType.Expense => Balance.Add(amount),
                TransactionType.Transfer => Balance.Add(amount),
                _ => throw new InvalidOperationException($"Unknown transaction type: {transactionType}")
            };
            MarkAsUpdated();
        }

        /// <summary>Credits this wallet when it is the destination of a transfer.</summary>
        public void ReceiveTransfer(decimal amount)
        {
            Balance = Balance.Add(amount);
            MarkAsUpdated();
        }

        // ─── Validation ─────────────────────────────────────────────────────────

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Wallet name cannot be empty.", nameof(name));

            if (name.Length < 2)
                throw new ArgumentException("Wallet name must be at least 2 characters.", nameof(name));

            if (name.Length > 100)
                throw new ArgumentException("Wallet name cannot exceed 100 characters.", nameof(name));
        }
    }
}
