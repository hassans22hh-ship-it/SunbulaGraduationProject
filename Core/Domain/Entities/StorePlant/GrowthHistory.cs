
namespace Domain.Entities.StorePlant
{
    public class GrowthHistory
    {
        public Guid Id { get; private set; }
        public Guid UserPlantId { get; private set; }

        public int Stage { get; private set; }
        public DateTime GrowthDate { get; private set; }
        public DateTime AchievedAt { get; private set; }

        public UserPlant UserPlant { get; private set; }

        private GrowthHistory() { }

        public GrowthHistory(Guid userPlantId, int stage)
        {
            Id = Guid.NewGuid();
            UserPlantId = userPlantId;
            Stage = stage;
            GrowthDate = DateTime.UtcNow;
            AchievedAt = DateTime.UtcNow;
        }
    }
}
