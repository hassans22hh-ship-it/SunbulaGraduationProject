
namespace Domain.Entities.StorePlant
{
    public class UserPlant
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid PlantId { get; private set; }

        public int CoinsSpent { get; private set; }
        public int GrowthStage { get; private set; }
        public DateTime PurchasedDate { get; private set; }

        public Plant Plant { get; private set; }

        private readonly List<GrowthHistory> _growthHistory = new();
        public IReadOnlyCollection<GrowthHistory> GrowthHistory => _growthHistory;

        private UserPlant() { }

        public UserPlant(Guid userId, Guid plantId, int coinsSpent)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            PlantId = plantId;
            CoinsSpent = coinsSpent;
            GrowthStage = 0;
            PurchasedDate = DateTime.UtcNow;
        }

        public void AdvanceGrowthStage(int stage)
        {
            if (stage <= GrowthStage)
                throw new InvalidOperationException("Stage must be greater than current.");

            GrowthStage = stage;
        }
    }
}
