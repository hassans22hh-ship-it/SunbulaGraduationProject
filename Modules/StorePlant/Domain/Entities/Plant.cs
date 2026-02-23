using PlantDomain.Enums;
using PlantDomain.Events;
using SharedKernel;

namespace PlantDomain.Entities
{
    /// Plant aggregate root.
    /// Represents a plant available for purchase in the virtual store.
    /// Manages plant metadata, pricing, seasonal availability, and level classification.
    
    public class Plant:BaseEntity
    {
        private readonly List<UserPlant> _userPlants = new();

        private Plant() { }

        private Plant(Guid id, string name, string botanicName, string description,
            string imageUrl, int price, PlantLevel level, string? decoration) : base(id)
        {
            Name = name;
            BotanicName = botanicName;
            Description = description;
            ImageUrl = imageUrl;
            Price = price;
            Level = level;
            Decoration = decoration;
            IsAvailable = true;
            IsSeasonal = false;
        }

        // ── Properties ──────────────────────────────────────────────────
        public string Name { get; private set; } = string.Empty;
        public string BotanicName { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string ImageUrl { get; private set; } = string.Empty;
        public int Price { get; private set; }
        public PlantLevel Level { get; private set; }
        public string? Decoration { get; private set; }
        public bool IsAvailable { get; private set; }
        public bool IsSeasonal { get; private set; }
        public DateTime? SeasonStart { get; private set; }
        public DateTime? SeasonEnd { get; private set; }

        public IReadOnlyCollection<UserPlant> UserPlants => _userPlants.AsReadOnly();

        // ── Factory Method ───────────────────────────────────────────────

        /// <summary>Creates a new Plant for the virtual store.</summary>
        public static Plant Create(string name, string botanicName, string description,
            string imageUrl, int price, PlantLevel level, string? decoration = null)
        {
            ValidateName(name);
            ValidateBotanicName(botanicName);
            ValidatePrice(price);
            ValidateImageUrl(imageUrl);

            var plant = new Plant(Guid.NewGuid(), name, botanicName, description,
                imageUrl, price, level, decoration);

            plant.RaiseDomainEvent(new PlantCreatedEvent(plant.Id));
            return plant;
        }

        // ── Domain Methods ───────────────────────────────────────────────

        /// <summary>Updates plant store information.</summary>
        public void Update(string name, string botanicName, string description,
            string imageUrl, int price, string? decoration)
        {
            ValidateName(name);
            ValidateBotanicName(botanicName);
            ValidatePrice(price);
            ValidateImageUrl(imageUrl);

            Name = name;
            BotanicName = botanicName;
            Description = description;
            ImageUrl = imageUrl;
            Price = price;
            Decoration = decoration;
            MarkAsUpdated();
        }

        /// <summary>Marks the plant as seasonal with a specific availability window.</summary>
        public void SetSeasonal(DateTime seasonStart, DateTime seasonEnd)
        {
            if (seasonEnd <= seasonStart)
                throw new ArgumentException("Season end must be after season start.");

            IsSeasonal = true;
            SeasonStart = seasonStart;
            SeasonEnd = seasonEnd;
            MarkAsUpdated();
        }

        /// <summary>Removes seasonal restriction from the plant.</summary>
        public void RemoveSeasonal()
        {
            IsSeasonal = false;
            SeasonStart = null;
            SeasonEnd = null;
            MarkAsUpdated();
        }

        /// <summary>Toggles the plant's visibility in the store.</summary>
        public void SetAvailability(bool isAvailable)
        {
            IsAvailable = isAvailable;
            MarkAsUpdated();
        }

        /// <summary>Returns true if the plant is currently purchasable.</summary>
        public bool IsCurrentlyAvailable()
        {
            if (!IsAvailable) return false;
            if (!IsSeasonal) return true;

            var now = DateTime.UtcNow;
            return now >= SeasonStart && now <= SeasonEnd;
        }

        // ── Private Validation ────────────────────────────────────────────

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Plant name cannot be empty.", nameof(name));
            if (name.Length > 100)
                throw new ArgumentException("Plant name cannot exceed 100 characters.", nameof(name));
        }

        private static void ValidateBotanicName(string botanicName)
        {
            if (string.IsNullOrWhiteSpace(botanicName))
                throw new ArgumentException("Botanic name cannot be empty.", nameof(botanicName));
            if (botanicName.Length > 150)
                throw new ArgumentException("Botanic name cannot exceed 150 characters.", nameof(botanicName));
        }

        private static void ValidatePrice(int price)
        {
            if (price <= 0)
                throw new ArgumentException("Price must be greater than zero.", nameof(price));
        }

        private static void ValidateImageUrl(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ArgumentException("Image URL cannot be empty.", nameof(imageUrl));
        }
    }
}
