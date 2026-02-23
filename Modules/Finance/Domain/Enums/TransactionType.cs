namespace FinanceDomain.Enums
{
    /// Represents the type of a financial transaction.

    public enum TransactionType
    {

        /// Money received.
        Income = 0,

        /// Money spent.
        Expense = 1,

        /// Money moved between wallets.
        Transfer = 2
    }
}
