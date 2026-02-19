using SharedKernel;

namespace Domain.Entities.TaskManagement
{
    public class Folder:BaseEntity
    {

        public string Name { get; private set; } = null!;

        public string Color { get; private set; } = "#FFFFFF";

        public Guid UserId { get; private set; }

        // Navigation
        public ICollection<Task> Tasks { get; private set; } = new List<Task>();

        private Folder() { }

        public Folder(Guid userId, string name, string color)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Name = name;
            Color = color;
        }

        public void Rename(string name)
        {
            Name = name;
        }
    }
}
