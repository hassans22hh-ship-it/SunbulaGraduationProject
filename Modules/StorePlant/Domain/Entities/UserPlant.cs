using PlantDomain.Enums;
using PlantDomain.Events;
using SharedKernel;

namespace PlantDomain.Entities
{
    /// UserPlant entity — represents a plant owned by a user after purchase.
    /// Tracks purchase metadata, current growth stage, and accumulated growth coins.
    /// Cross-module reference: UserId is a Guid (no navigation to UserIdentity module).
   
    public class UserPlant:BaseEntity
    {
        private readonly List<GrowthHistory> _growthHistories = new();

        private UserPlant() { }

        private UserPlant(Guid id, Guid userId, Guid plantId, int coinsSpent) : base(id)
        {
            UserId = userId;
            PlantId = plantId;
            CoinsSpent = coinsSpent;
            PurchaseDate = DateTime.UtcNow;
            CurrentStage = GrowthStage.Seed;
            StageCoinsAccumulated = 0;
        }

        // ── Properties ──────────────────────────────────────────────────
        public Guid UserId { get; private set; }
        public Guid PlantId { get; private set; }
        public int CoinsSpent { get; private set; }
        public DateTime PurchaseDate { get; private set; }
        public GrowthStage CurrentStage { get; private set; }
        public int StageCoinsAccumulated { get; private set; }

        // Navigation (within same module)
        public Plant Plant { get; private set; } = null!;
        public IReadOnlyCollection<GrowthHistory> GrowthHistories => _growthHistories.AsReadOnly();

        // ── Factory Method ───────────────────────────────────────────────

        /// <summary>
        /// Creates a new UserPlant record upon successful store purchase.
        /// Called by UserPlantService after coin deduction is confirmed.
        /// </summary>
        public static UserPlant Create(Guid userId, Guid plantId, int coinsSpent)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));
            if (plantId == Guid.Empty)
                throw new ArgumentException("PlantId cannot be empty.", nameof(plantId));
            if (coinsSpent <= 0)
                throw new ArgumentException("Coins spent must be positive.", nameof(coinsSpent));

            var userPlant = new UserPlant(Guid.NewGuid(), userId, plantId, coinsSpent);
            userPlant.RaiseDomainEvent(new PlantPurchasedEvent(userPlant.Id, userId, plantId, coinsSpent));
            return userPlant;
        }

        // ── Domain Methods ────────────────────────────────────────────────

        /// <summary>
        /// Adds growth coins and advances the growth stage if threshold is reached.
        /// Business rule: every 10,000 additional coins → plant grows one stage.
        /// </summary>
        public void AddGrowthCoins(int coins)
        {
            if (coins <= 0)
                throw new ArgumentException("Growth coins must be positive.", nameof(coins));
            if (CurrentStage == GrowthStage.LargePlant)
                return; // Already at max stage

            StageCoinsAccumulated += coins;

            const int CoinsPerStage = 10_000;
            if (StageCoinsAccumulated >= CoinsPerStage)
            {
                StageCoinsAccumulated -= CoinsPerStage;
                AdvanceStage();
            }

            MarkAsUpdated();
        }

        // ── Private Helpers ───────────────────────────────────────────────

        private void AdvanceStage()
        {
            var previousStage = CurrentStage;
            CurrentStage = CurrentStage switch
            {
                GrowthStage.Seed => GrowthStage.Seedling,
                GrowthStage.Seedling => GrowthStage.SmallPlant,
                GrowthStage.SmallPlant => GrowthStage.LargePlant,
                _ => CurrentStage
            };

            if (CurrentStage != previousStage)
            {
                var history = GrowthHistory.Create(Id, CurrentStage);
                _growthHistories.Add(history);
                RaiseDomainEvent(new PlantGrownEvent(Id, UserId, CurrentStage));
            }
        }
    }
}
