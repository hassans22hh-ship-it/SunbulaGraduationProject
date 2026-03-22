namespace DebtDomain.Exceptions
{
    public class DebtNotFoundException: Exception
    {
        public DebtNotFoundException(Guid debtId)
        : base($"Debt with ID '{debtId}' was not found")
        {
            DebtId = debtId;
        }

        public Guid DebtId { get; }
    }
}

