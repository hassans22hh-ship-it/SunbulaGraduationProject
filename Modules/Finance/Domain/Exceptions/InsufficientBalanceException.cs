namespace FinanceDomain.Exceptions
{
    public class InsufficientBalanceException: Exception
    {

        public InsufficientBalanceException(string walletName, decimal current, decimal required)
            : base($"Wallet '{walletName}' has insufficient balance. Current: {current:F2}, Required: {required:F2}.") { }

    }

}
