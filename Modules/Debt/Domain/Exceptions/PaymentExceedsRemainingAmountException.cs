namespace DebtDomain.Exceptions
{
    public sealed  class PaymentExceedsRemainingAmountException:Exception
    {
        public PaymentExceedsRemainingAmountException(decimal paymentAmount, decimal remainingAmount)
        : base($"Payment amount ({paymentAmount:F2}) exceeds remaining debt amount ({remainingAmount:F2})")
        {
            PaymentAmount = paymentAmount;
            RemainingAmount = remainingAmount;
        }

        public decimal PaymentAmount { get; }
        public decimal RemainingAmount { get; }
    }
}
