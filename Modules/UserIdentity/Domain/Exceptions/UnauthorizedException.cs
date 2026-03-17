namespace Domain.Exceptions
{
    public sealed  class UnauthorizedException:Exception
    {
        public UnauthorizedException(string message) : base(message)
        {
        }
        public UnauthorizedException() : base("Invalid credentials provided") { }

    }
}
