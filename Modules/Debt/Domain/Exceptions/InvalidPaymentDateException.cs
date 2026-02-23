namespace DebtDomain.Exceptions
{
    public sealed class InvalidPaymentDateException:Exception
    {

        public InvalidPaymentDateException(string message) : base(message)
        {
        }
    }
}
