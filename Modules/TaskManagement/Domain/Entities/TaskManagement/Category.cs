using SharedKernel;
using TaskDomain.Entities.TaskManagement.Events;
using TaskDomain.Entities.TaskManagement.ValueObjects;

namespace TaskDomain.Entities.TaskManagement
{
    public sealed class Category:BaseEntity
    {

        private readonly List<TaskCategory> _taskCategories = new();

        // Private constructor for EF Core
        private Category() { }

        // Private constructor for factory method
        private Category(Guid id, Guid userId, string name, TaskColor color) : base(id)
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
        public IReadOnlyCollection<TaskCategory> TaskCategories => _taskCategories.AsReadOnly();

        // ═══════════════════════════════════════════════════════════════
        // FACTORY METHOD
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Creates a new Category.
        /// </summary>
        public static Category Create(Guid userId, string name, string color)
        {
            ValidateName(name);
            var categoryColor = TaskColor.Create(color);

            var category = new Category(Guid.NewGuid(), userId, name, categoryColor);

            category.RaiseDomainEvent(new CategoryCreatedEvent(category.Id, category.UserId));

            return category;
        }

        // ═══════════════════════════════════════════════════════════════
        // DOMAIN METHODS
        // ═══════════════════════════════════════════════════════════════

        /// Updates category information.
        public void Update(string name, string color)
        {
            ValidateName(name);
            var categoryColor = TaskColor.Create(color);

            Name = name;
            Color = categoryColor;

            MarkAsUpdated();
        }

        /// Gets the count of tasks in this category.
        public int GetTaskCount()
        {
            return _taskCategories.Count;
        }

        // ═══════════════════════════════════════════════════════════════
        // VALIDATION
        // ═══════════════════════════════════════════════════════════════

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name cannot be empty", nameof(name));

            if (name.Length < 2)
                throw new ArgumentException("Category name must be at least 2 characters", nameof(name));

            if (name.Length > 50)
                throw new ArgumentException("Category name cannot exceed 50 characters", nameof(name));
        }
    }
}
