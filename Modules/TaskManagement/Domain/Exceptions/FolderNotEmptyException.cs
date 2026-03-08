

namespace TaskDomain.Exceptions
{
    public sealed class FolderNotEmptyException:Exception
    {
        public FolderNotEmptyException(Guid folderId, int taskCount)
    : base($"Cannot delete folder '{folderId}' because it contains {taskCount} task(s)")
        {
            FolderId = folderId;
            TaskCount = taskCount;
        }

        public Guid FolderId { get; }
        public int TaskCount { get; }
    }
}
