namespace TimeTrackingDomain.Exceptions
{
    public class ActiveSessionExistsException:Exception
    {
        public ActiveSessionExistsException(Guid activeSessionId)
      : base($"An active session (ID: {activeSessionId}) already exists. " +
             "Stop the current session before starting a new one.")
        { }
    }
}
