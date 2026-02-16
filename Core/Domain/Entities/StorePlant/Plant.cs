
namespace Domain.Entities.StorePlant
{
    public class Plant
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string ImageUrl { get; private set; }
        public string GrowthName { get; private set; }

        public int Level { get; private set; }
        public int Points { get; private set; }

        private readonly List<UserPlant> _userPlants = new();
        public IReadOnlyCollection<UserPlant> UserPlants => _userPlants;

        private Plant() { } // EF

        public Plant(string name, string description, string imageUrl, string growthName, int level, int points)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            ImageUrl = imageUrl;
            GrowthName = growthName;
            Level = level;
            Points = points;
        }
    }
}
