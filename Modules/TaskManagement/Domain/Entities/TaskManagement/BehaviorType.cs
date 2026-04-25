namespace Domain.Entities.TaskManagement
{
    public enum BehaviorType
    {
        Positive = 1,
        Neutral = 2,
        Rest = 3,      // was 4 — now aligned with TimeTracking enum
        Negative = 4   // was 3 — now aligned with TimeTracking enum
    }
}
