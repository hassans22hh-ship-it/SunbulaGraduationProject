using PlantDomain.Enums;
using SharedKernel;

namespace PlantDomain.Entities
{
    public class GrowthHistory:BaseEntity
    {
        private GrowthHistory() { }

        private GrowthHistory(Guid id, Guid userPlantId, GrowthStage stage) : base(id)
        {
            UserPlantId = userPlantId;
            Stage = stage;
            AchievementId = Guid.NewGuid(); // Unique identifier for this growth event
            GrowthDate = DateTime.UtcNow;
        }

        public Guid UserPlantId { get; private set; }
        public GrowthStage Stage { get; private set; }
        public Guid AchievementId { get; private set; }
        public DateTime GrowthDate { get; private set; }

        // Navigation (within same module)
        public UserPlant UserPlant { get; private set; } = null!;

        /// <summary>Creates a new GrowthHistory record. Called internally by UserPlant.</summary>
        internal static GrowthHistory Create(Guid userPlantId, GrowthStage stage)
        {
            if (userPlantId == Guid.Empty)
                throw new ArgumentException("UserPlantId cannot be empty.", nameof(userPlantId));

            return new GrowthHistory(Guid.NewGuid(), userPlantId, stage);
        }
    }
}
