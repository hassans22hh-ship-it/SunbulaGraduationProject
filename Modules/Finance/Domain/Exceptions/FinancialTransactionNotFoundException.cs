namespace FinanceDomain.Exceptions
{
    public class FinancialTransactionNotFoundException:Exception
    {
        public FinancialTransactionNotFoundException(Guid id)
       : base($"Financial transaction with ID '{id}' was not found.") { }
    }
}
