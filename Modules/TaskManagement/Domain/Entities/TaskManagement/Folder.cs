using SharedKernel;
using TaskDomain.Entities.TaskManagement.Events;
using TaskDomain.Entities.TaskManagement.ValueObjects;

namespace Domain.Entities.TaskManagement
{
    public class Folder:BaseEntity
    {

        private readonly List<TaskItem> _tasks = new();

        // Private constructor for EF Core
        private Folder() { }

        // Private constructor for factory method
        private Folder(Guid id, Guid userId, string name, TaskColor color) : base(id)
        {
            UserId = userId;
            Name = name;
            Color = color;
        }

        // Properties
        public Guid UserId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public TaskColor Color { get; private set; } = null!;

        // Navigation properties
        public IReadOnlyCollection<TaskItem> Tasks => _tasks.AsReadOnly();

        // ═══════════════════════════════════════════════════════════════
        // FACTORY METHOD
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Creates a new Folder.
        /// </summary>
        public static Folder Create(Guid userId, string name, string color)
        {
            ValidateName(name);
            var folderColor = TaskColor.Create(color);

            var folder = new Folder(Guid.NewGuid(), userId, name, folderColor);

            folder.RaiseDomainEvent(new FolderCreatedEvent(folder.Id, folder.UserId));

            return folder;
        }

        // ═══════════════════════════════════════════════════════════════
        // DOMAIN METHODS
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Updates folder information.
        /// </summary>
        public void Update(string name, string color)
        {
            ValidateName(name);
            var folderColor = TaskColor.Create(color);

            Name = name;
            Color = folderColor;

            MarkAsUpdated();
        }

        /// <summary>
        /// Checks if folder can be deleted.
        /// Business rule: Cannot delete folder with tasks.
        /// </summary>
        public bool CanBeDeleted()
        {
            return !_tasks.Any();
        }

        /// <summary>
        /// Gets the count of tasks in this folder.
        /// </summary>
        public int GetTaskCount()
        {
            return _tasks.Count;
        }

        // ═══════════════════════════════════════════════════════════════
        // VALIDATION
        // ═══════════════════════════════════════════════════════════════

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Folder name cannot be empty", nameof(name));

            if (name.Length < 2)
                throw new ArgumentException("Folder name must be at least 2 characters", nameof(name));

            if (name.Length > 100)
                throw new ArgumentException("Folder name cannot exceed 100 characters", nameof(name));
        }

    }
}
