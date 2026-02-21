namespace TaskDomain.Entities.TaskManagement
{
    public class TaskCategory : SharedKernel.BaseEntity
    {
        public Guid TaskId { get; private set; }
        public Guid CategoryId { get; private set; }

        public virtual Domain.Entities.TaskManagement.Task Task { get; private set; } = null!;
        public virtual Category Category { get; private set; } = null!;

        private TaskCategory() { } // EF Core

        private TaskCategory(Guid taskId, Guid categoryId) : base(Guid.NewGuid())
        {
            TaskId = taskId;
            CategoryId = categoryId;
        }

        public static TaskCategory Create(Guid taskId, Guid categoryId)
        {
            return new TaskCategory(taskId, categoryId);
        }
    }
}
