namespace TimeTrackingDomain.Exceptions
{
    public sealed class TimeSessionNotFoundException: Exception
    {
        public TimeSessionNotFoundException(Guid sessionId)
        : base($"Time session with ID '{sessionId}' was not found.") { }

    }
}
