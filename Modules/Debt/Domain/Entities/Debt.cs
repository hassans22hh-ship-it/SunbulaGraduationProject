using DebtDomain.Enums;
using DebtDomain.Events;
using DebtDomain.Exceptions;
using DebtDomain.ValueObjects;
using SharedKernel;

namespace DebtDomain.Entities
{
    public class Debt:BaseEntity
    {
        private readonly List<DebtPayment> _payments = new();

        // Private constructor for EF Core
        private Debt() { }

        // Private constructor for factory method
        private Debt(
            Guid id,
            Guid userId,
            string creditorName,
            Money amount,
            DebtType debtType,
            DateTime dueDate,
            string? notes) : base(id)
        {
            UserId = userId;
            CreditorName = creditorName;
            Amount = amount;
            RemainingAmount = amount;
            DebtType = debtType;
            DueDate = dueDate;
            Notes = notes;
            IsPaid = false;
        }

        // Properties - ALL with private set
        public Guid UserId { get; private set; }

        /// <summary>
        /// Name of the creditor (person/organization owed to or owing from)
        /// </summary>
        public string CreditorName { get; private set; } = string.Empty;

        /// <summary>
        /// Original debt amount
        /// </summary>
        public Money Amount { get; private set; } = null!;

        /// <summary>
        /// Remaining amount to be paid
        /// </summary>
        public Money RemainingAmount { get; private set; } = null!;

        /// <summary>
        /// Due date for payment
        /// </summary>
        public DateTime DueDate { get; private set; }

        /// <summary>
        /// Whether the debt is fully paid
        /// </summary>
        public bool IsPaid { get; private set; }

        /// <summary>
        /// Type of debt (Payable or Receivable)
        public DebtType DebtType { get; private set; }

        /// Optional notes about the debt
        public string? Notes { get; private set; }

        // Navigation properties
        public IReadOnlyCollection<DebtPayment> Payments => _payments.AsReadOnly();

        // Computed property
        public bool IsOverdue => !IsPaid && DueDate < DateTime.UtcNow;

        // ═══════════════════════════════════════════════════════════════
        // FACTORY METHOD
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Creates a new Debt.
        /// </summary>
        public static Debt Create(
            Guid userId,
            string creditorName,
            decimal amount,
            DebtType debtType,
            DateTime dueDate,
            string? notes = null)
        {
            // Validate parameters
            ValidateCreditorName(creditorName);
            ValidateDueDate(dueDate);

            // Create value object
            var money = Money.Create(amount);

            // Create instance
            var debt = new Debt(
                Guid.NewGuid(),
                userId,
                creditorName,
                money,
                debtType,
                dueDate,
                notes);

            // Raise domain event
            debt.RaiseDomainEvent(new DebtCreatedEvent(
                debt.Id,
                debt.UserId,
                debt.Amount.Value,
                debt.DebtType));

            return debt;
        }

        // ═══════════════════════════════════════════════════════════════
        // DOMAIN METHODS (Business Logic)
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Updates debt information.
        /// </summary>
        public void Update(
            string creditorName,
            DateTime dueDate,
            string? notes = null)
        {
            if (IsPaid)
                throw new DebtAlreadyPaidException(Id);

            ValidateCreditorName(creditorName);
            ValidateDueDate(dueDate);

            CreditorName = creditorName;
            DueDate = dueDate;
            Notes = notes;

            MarkAsUpdated();
        }

        /// <summary>
        /// Records a payment against this debt.
        /// </summary>
        public DebtPayment RecordPayment(
            decimal paymentAmount,
            DateTime paymentDate,
            string? notes = null)
        {
            // Business rule validations
            if (IsPaid)
                throw new DebtAlreadyPaidException(Id);

            if (paymentDate > DateTime.UtcNow)
                throw new InvalidPaymentDateException("Payment date cannot be in the future");

            var payment = Money.Create(paymentAmount);

            if (payment.Value > RemainingAmount.Value)
                throw new PaymentExceedsRemainingAmountException(
                    payment.Value,
                    RemainingAmount.Value);

            // Create payment using its factory
            var debtPayment = DebtPayment.Create(
                Id,
                paymentAmount,
                paymentDate,
                notes);

            // Add to collection
            _payments.Add(debtPayment);

            // Update remaining amount
            RemainingAmount = Money.Create(RemainingAmount.Value - payment.Value);

            // Check if fully paid
            if (RemainingAmount.Value == 0)
            {
                IsPaid = true;
                RaiseDomainEvent(new DebtFullyPaidEvent(Id, UserId));
            }

            // Raise payment recorded event
            RaiseDomainEvent(new PaymentRecordedEvent(
                Id,
                debtPayment.Id,
                payment.Value,
                RemainingAmount.Value));

            MarkAsUpdated();

            return debtPayment;
        }

        /// <summary>
        /// Marks the entire debt as paid (without recording individual payment).
        /// </summary>
        public void MarkAsPaid()
        {
            if (IsPaid)
                return; // Already paid

            IsPaid = true;
            RemainingAmount = Money.Create(0);

            RaiseDomainEvent(new DebtFullyPaidEvent(Id, UserId));
            MarkAsUpdated();
        }

        /// <summary>
        /// Reopens a paid debt (in case of mistake).
        /// </summary>
        public void Reopen()
        {
            if (!IsPaid)
                throw new InvalidOperationException("Debt is not paid, cannot reopen");

            if (_payments.Any())
            {
                // Recalculate remaining amount based on payments
                var totalPaid = _payments.Sum(p => p.Amount.Value);
                RemainingAmount = Money.Create(Amount.Value - totalPaid);
            }
            else
            {
                RemainingAmount = Amount;
            }

            IsPaid = false;
            MarkAsUpdated();
        }

        // ═══════════════════════════════════════════════════════════════
        // VALIDATION (Private Methods)
        // ═══════════════════════════════════════════════════════════════

        private static void ValidateCreditorName(string creditorName)
        {
            if (string.IsNullOrWhiteSpace(creditorName))
                throw new ArgumentException("Creditor name cannot be empty", nameof(creditorName));

            if (creditorName.Length < 2)
                throw new ArgumentException(
                    "Creditor name must be at least 2 characters",
                    nameof(creditorName));

            if (creditorName.Length > 100)
                throw new ArgumentException(
                    "Creditor name cannot exceed 100 characters",
                    nameof(creditorName));
        }

        private static void ValidateDueDate(DateTime dueDate)
        {
            if (dueDate < DateTime.UtcNow.Date)
                throw new ArgumentException(
                    "Due date cannot be in the past",
                    nameof(dueDate));
        }
    }
}
