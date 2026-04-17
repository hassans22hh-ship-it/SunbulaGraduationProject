namespace Domain.Exceptions
{
    public sealed  class ValidationException:Exception
    {
        public ValidationException(IDictionary<string, string[]>errors) : base("One or more validation errors occurred")
        {
            Errors= errors;

        }
        public ValidationException(string propertyName, string errorMessage)
      : base("Validation error occurred")
        {
            Errors = new Dictionary<string, string[]>
        {
            { propertyName, new[] { errorMessage } }
        };
        }
        public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>();
    }
}
