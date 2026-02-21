using DebtDomain.ValueObjects;
using SharedKernel;

namespace DebtDomain.Entities
{
    /// DebtPayment entity.
    /// Represents a single payment made against a debt.
    public class DebtPayment: BaseEntity
    {

        // Private constructor for EF Core
        private DebtPayment() { }

        // Private constructor for factory method
        private DebtPayment(
            Guid id,
            Guid debtId,
            Money amount,
            DateTime paymentDate,
            string? notes) : base(id)
        {
            DebtId = debtId;
            Amount = amount;
            PaymentDate = paymentDate;
            Notes = notes;
        }

        // Properties
        public Guid DebtId { get; private set; }
        public Money Amount { get; private set; } = null!;
        public DateTime PaymentDate { get; private set; }
        public string? Notes { get; private set; }

        // Navigation property
        public Debt Debt { get; private set; } = null!;

        // ═══════════════════════════════════════════════════════════════
        // FACTORY METHOD
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Creates a new DebtPayment.
        /// </summary>
        public static DebtPayment Create(
            Guid debtId,
            decimal amount,
            DateTime paymentDate,
            string? notes = null)
        {
            var money = Money.Create(amount);

            var payment = new DebtPayment(
                Guid.NewGuid(),
                debtId,
                money,
                paymentDate,
                notes);

            return payment;
        }

        /// <summary>
        /// Updates payment information.
        /// </summary>
        public void Update(decimal amount, DateTime paymentDate, string? notes = null)
        {
            Amount = Money.Create(amount);
            PaymentDate = paymentDate;
            Notes = notes;
            MarkAsUpdated();
        }

    }
}
