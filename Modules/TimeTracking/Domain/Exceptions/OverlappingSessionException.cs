namespace TimeTrackingDomain.Exceptions
{
    public sealed class OverlappingSessionException:Exception
    {
        public OverlappingSessionException(DateTime startTime, DateTime endTime)
    : base($"A session already exists that overlaps with {startTime:HH:mm} - {endTime:HH:mm}. " +
           "Sessions cannot overlap.")
        { }
    }
}
