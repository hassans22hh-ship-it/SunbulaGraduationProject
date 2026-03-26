using DebtDomain.Enums;
using SharedKernel;

namespace DebtDomain.Contracts

{
    /// Repository for Debt aggregate.
    /// Contains custom queries - NO Specification Pattern.
    public interface IDebtRepository: IRepository<Entities.Debt>
    {
        /// <summary>
        /// Gets a debt with all its payments.
        /// </summary>
        Task<Entities.Debt?> GetByIdWithPaymentsAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all debts for a user.
        /// </summary>
        Task<IEnumerable<Entities.Debt>> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all unpaid debts for a user.
        /// </summary>
        Task<IEnumerable<Entities.Debt>> GetUnpaidByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all overdue debts for a user.
        /// </summary>
        Task<IEnumerable<Entities.Debt>> GetOverdueByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets debts by type (Payable or Receivable).
        /// </summary>
        Task<IEnumerable<Entities.Debt>> GetByTypeAsync(
            Guid userId,
            DebtType debtType,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets total debt amount for a user by type.
        /// </summary>
        Task<decimal> GetTotalDebtAmountAsync(
            Guid userId,
            DebtType debtType,
            bool unpaidOnly = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets total remaining amount for a user.
        /// </summary>
        Task<decimal> GetTotalRemainingAmountAsync(
            Guid userId,
            DebtType debtType,
            CancellationToken cancellationToken = default);

        Task HardDeleteByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
