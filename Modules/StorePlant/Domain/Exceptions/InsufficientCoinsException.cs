namespace PlantDomain.Exceptions
{
    public class InsufficientCoinsException:Exception
    {
        public InsufficientCoinsException(int required, int available)
       : base($"Insufficient coins. Required: {required}, Available: {available}. " +
              $"You need {required - available} more coins.")
        { }
    }
}
