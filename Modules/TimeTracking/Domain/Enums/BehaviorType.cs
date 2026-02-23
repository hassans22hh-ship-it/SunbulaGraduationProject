namespace TimeTrackingDomain.Enums
{
    /// Behavior classification that determines coin earnings per hour.

    public enum BehaviorType
    {
        /// <summary>
        /// prayer, reading, exercise, productive work → +2 coins/hour
        /// </summary>
        Positive = 1,
        /// Sleep, eating, commute → +1 coin/hour
        Neutral = 2,
        /// <summary>
        /// Healthy entertainment, relaxation → +1 coin/hour
        /// </summary>
        Rest = 3,
        /// <summary>
        /// Procrastination, excessive social media → -1 coin/hour
        /// </summary>
        Negative = 4

    }
}
