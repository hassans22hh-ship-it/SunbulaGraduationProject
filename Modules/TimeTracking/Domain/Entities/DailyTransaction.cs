using SharedKernel;

namespace TimeTrackingDomain.Entities
{
    public class DailyTransaction : BaseEntity
    {
        private DailyTransaction() { }

        private DailyTransaction(Guid id, Guid userId, DateOnly date) : base(id)
        {
            UserId = userId;
            Date = date;
            TotalMinutes = 0;
            TotalCoins = 0;
            SessionCount = 0;
        }

        // ═══════════════════════════════════════════════════════════════
        // PROPERTIES
        // ═══════════════════════════════════════════════════════════════

        public Guid UserId { get; private set; }
        public DateOnly Date { get; private set; }
        public int TotalMinutes { get; private set; }
        public int TotalCoins { get; private set; }
        public int SessionCount { get; private set; }

        // ═══════════════════════════════════════════════════════════════
        // FACTORY
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Creates a new daily transaction record for the given user and date.
        /// </summary>
        public static DailyTransaction Create(Guid userId, DateOnly date)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));

            return new DailyTransaction(Guid.NewGuid(), userId, date);
        }

        // ═══════════════════════════════════════════════════════════════
        // DOMAIN METHODS
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Adds a completed session's data to this daily summary.
        /// </summary>
        public void AddSession(int durationMinutes, int coinsEarned)
        {
            if (durationMinutes < 0)
                throw new ArgumentException("Duration cannot be negative.", nameof(durationMinutes));

            TotalMinutes += durationMinutes;
            TotalCoins += coinsEarned;
            SessionCount++;

            MarkAsUpdated();
        }

        /// <summary>
        /// Removes a session's contribution from this daily summary.
        /// Called when a session is deleted.
        /// </summary>
        public void RemoveSession(int durationMinutes, int coinsEarned)
        {
            TotalMinutes = Math.Max(0, TotalMinutes - durationMinutes);
            TotalCoins -= coinsEarned;
            SessionCount = Math.Max(0, SessionCount - 1);

            MarkAsUpdated();
        }

        /// <summary>
        /// Replaces old session data with updated session data.
        /// </summary>
        public void UpdateSession(int oldDurationMinutes, int oldCoins, int newDurationMinutes, int newCoins)
        {
            RemoveSession(oldDurationMinutes, oldCoins);
            AddSession(newDurationMinutes, newCoins);
        }

        /// <summary>
        /// Checks if this day qualifies for streak (20+ hours tracked).
        /// </summary>
        public bool QualifiesForStreak() => TotalMinutes >= 1200; // 30 minutes minimum
    }
}
