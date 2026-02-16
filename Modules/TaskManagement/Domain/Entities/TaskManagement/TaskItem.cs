namespace Domain.Entities.TaskManagement
{
    public class TaskItem
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = null!;

        public string? Emoji { get; private set; }

        public string Color { get; private set; } = "#FFFFFF";

        public BehaviorType Behavior { get; private set; }

        public bool IsArchived { get; private set; }

        public Guid UserId { get; private set; }

        public Guid? FolderId { get; private set; }

        // Navigation
        public Folder? Folder { get; private set; }

        //public ICollection<TaskCategory> TaskCategories { get; private set; }
        //    = new List<TaskCategory>();

        private TaskItem() { }

        public TaskItem(Guid userId,
                        string name,
                        BehaviorType behavior,
                        string color,
                        string? emoji = null,
                        Guid? folderId = null)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Name = name;
            Behavior = behavior;
            Color = color;
            Emoji = emoji;
            FolderId = folderId;
        }

        public void Archive() => IsArchived = true;

        public void Restore() => IsArchived = false;

        public void ChangeBehavior(BehaviorType behavior)
            => Behavior = behavior;

    }
}
