namespace FinanceDomain.Exceptions
{
    public class FinancialCategoryNotFoundException: Exception
    {

        public FinancialCategoryNotFoundException(Guid id)
            : base($"Financial category with ID '{id}' was not found.") { }
    }
}
