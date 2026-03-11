namespace TaskDomain.Exceptions
{
    public sealed class TaskNotFoundException : Exception
    {
        public TaskNotFoundException(Guid taskId) : base($"Task with ID '{taskId}' was not found")
        {
            TaskId = taskId;
        }

        public Guid TaskId { get; }
    }
}
