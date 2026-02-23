namespace PlantDomain.Enums
{
    /// Growth stages a plant passes through based on accumulated coins activity.
    /// Every 10,000 additional coins advances the plant one stage.
    public enum GrowthStage
    {
        Seed = 0,
        Seedling = 1,
        SmallPlant = 2,
        LargePlant = 3
    }
}
