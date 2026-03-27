namespace Domain.Exceptions
{
    public sealed  class UnauthorizedException:Exception
    {
        public UnauthorizedException(string message) : base(message)
        {
        }
        public UnauthorizedException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public UnauthorizedException() : base("Invalid credentials provided") { }

    }
}
