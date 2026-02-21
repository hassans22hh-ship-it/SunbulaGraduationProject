namespace DebtDomain.Exceptions
{
    public sealed class DebtAlreadyPaidException:Exception
    {
        public DebtAlreadyPaidException(Guid debtId)
        : base($"Debt with ID '{debtId}' is already fully paid")
        {
            DebtId = debtId;
        }

        public Guid DebtId { get; }
    }
}
