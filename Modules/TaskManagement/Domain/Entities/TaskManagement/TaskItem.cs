using SharedKernel;
using TaskDomain.Entities.TaskManagement;
using TaskDomain.Entities.TaskManagement.Enums;
using TaskDomain.Entities.TaskManagement.Events;
using TaskDomain.Entities.TaskManagement.ValueObjects;
using TaskStatus = TaskDomain.Entities.TaskManagement.Enums.TaskStatus;

namespace Domain.Entities.TaskManagement
{
    public class TaskItem:BaseEntity
    {
        private readonly List<TaskCategory> _taskCategories = new();

        // Private constructor for EF Core
        private TaskItem() { }

        // Private constructor for factory method
        private TaskItem(
            Guid id,
            Guid userId,
            string title,
            string? emoji,
            TaskColor color,
            BehaviorCategory behaviorType,
            Guid? folderId) : base(id)
        {
            UserId = userId;
            Title = title;
            Emoji = emoji;
            Color = color;
            BehaviorType = behaviorType;
            FolderId = folderId;
            Status = TaskStatus.Active;
            IsArchived = false;
        }

        // Properties - ALL with private set
        public Guid UserId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string? Emoji { get; private set; }
        public TaskColor Color { get; private set; } = null!;
        public BehaviorCategory BehaviorType { get; private set; }
        public Guid? FolderId { get; private set; }
        public TaskStatus Status { get; private set; }
        public bool IsArchived { get; private set; }

        // Navigation properties (within same module only)
        public Folder? Folder { get; private set; }
        public IReadOnlyCollection<TaskCategory> TaskCategories => _taskCategories.AsReadOnly();

        // ═══════════════════════════════════════════════════════════════
        // FACTORY METHOD
        // ═══════════════════════════════════════════════════════════════

        /// Creates a new Task.
        public static TaskItem Create(
            Guid userId,
            string title,
            string? emoji,
            string color,
            BehaviorCategory behaviorType,
            Guid? folderId = null)
        {
            // Validate parameters
            ValidateTitle(title);
            var taskColor = TaskColor.Create(color);

            // Create instance
            var task = new TaskItem(Guid.NewGuid(), userId, title, emoji, taskColor, behaviorType, folderId);

            // Raise domain event
            task.RaiseDomainEvent(new TaskCreatedEvent(task.Id, task.UserId, task.BehaviorType));

            return task;
        }

        // ═══════════════════════════════════════════════════════════════
        // DOMAIN METHODS (Business Logic)
        // ═══════════════════════════════════════════════════════════════

        /// Updates task information.
        public void Update(string title, string? emoji, string color, BehaviorCategory behaviorType, Guid? folderId)
        {
            ValidateTitle(title);
            var taskColor = TaskColor.Create(color);

            var behaviorChanged = BehaviorType != behaviorType;

            Title = title;
            Emoji = emoji;
            Color = taskColor;
            BehaviorType = behaviorType;
            FolderId = folderId;

            MarkAsUpdated();

            if (behaviorChanged)
            {
                RaiseDomainEvent(new BehaviorTypeChangedEvent(Id, UserId, behaviorType));
            }
        }

        /// Adds a category to the task.
        public void AddCategory(Guid categoryId)
        {
            if (_taskCategories.Any(tc => tc.CategoryId == categoryId))
                return;

            var taskCategory = TaskCategory.Create(Id, categoryId);
            _taskCategories.Add(taskCategory);
        }

        /// Removes a category from the task.
        public void RemoveCategory(Guid categoryId)
        {
            var taskCategory = _taskCategories.FirstOrDefault(tc => tc.CategoryId == categoryId);
            if (taskCategory != null)
            {
                _taskCategories.Remove(taskCategory);
            }
        }

        /// Archives the task.
        public void Archive()
        {
            if (IsArchived)
                throw new InvalidOperationException("Task is already archived");

            IsArchived = true;
            Status = TaskStatus.Archived;
            MarkAsUpdated();

            RaiseDomainEvent(new TaskArchivedEvent(Id, UserId));
        }

        /// <summary>
        /// Unarchives the task.
        /// </summary>
        public void Unarchive()
        {
            if (!IsArchived)
                throw new InvalidOperationException("Task is not archived");

            IsArchived = false;
            Status = TaskStatus.Active;
            MarkAsUpdated();
        }

        /// Marks task as completed.
        public void Complete()
        {
            if (Status == TaskStatus.Completed)
                throw new InvalidOperationException("Task is already completed");

            Status = TaskStatus.Completed;
            MarkAsUpdated();

            RaiseDomainEvent(new TaskCompletedEvent(Id, UserId));
        }

        /// <summary>
        /// Moves task to a different folder.
        /// </summary>
        public void MoveToFolder(Guid? newFolderId)
        {
            FolderId = newFolderId;
            MarkAsUpdated();
        }

        /// <summary>
        /// Gets coin rate per hour based on behavior type.
        /// </summary>
        public int GetCoinRatePerHour()
        {
            return BehaviorType switch
            {
                BehaviorCategory.Positive => 2,
                BehaviorCategory.Neutral => 1,
                BehaviorCategory.Rest => 1,
                BehaviorCategory.Negative => -1,
                _ => 0
            };
        }

        /// <summary>
        /// Creates a duplicate of this task.
        /// </summary>
        public TaskItem Duplicate()
        {
            var newTitle = $"{Title} - Copy";
            if (newTitle.Length > 200) 
            {
                newTitle = newTitle.Substring(0, 200);
            }

            var duplicate = new TaskItem(
                Guid.NewGuid(), 
                UserId, 
                newTitle, 
                Emoji, 
                Color, 
                BehaviorType, 
                FolderId);

            foreach (var category in _taskCategories)
            {
                duplicate.AddCategory(category.CategoryId);
            }

            duplicate.RaiseDomainEvent(new TaskCreatedEvent(duplicate.Id, duplicate.UserId, duplicate.BehaviorType));

            return duplicate;
        }

        // ═══════════════════════════════════════════════════════════════
        // VALIDATION (Private Methods)
        // ═══════════════════════════════════════════════════════════════

        private static void ValidateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Task title cannot be empty", nameof(title));

            if (title.Length < 2)
                throw new ArgumentException("Task title must be at least 2 characters", nameof(title));

            if (title.Length > 200)
                throw new ArgumentException("Task title cannot exceed 200 characters", nameof(title));
        }

    }
}
