namespace FinanceDomain.Exceptions
{
    public class WalletNotFoundException:Exception
    {
        public WalletNotFoundException(Guid id)
       : base($"Wallet with ID '{id}' was not found.") { }
    }
}
