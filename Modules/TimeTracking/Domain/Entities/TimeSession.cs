using SharedKernel;
using TimeTrackingDomain.Enums;
using TimeTrackingDomain.Events;
using TimeTrackingDomain.ValueObjects;

namespace TimeTrackingDomain.Entities
{
    /// Represents a time tracking session for a specific task.
    /// A session records the time a user spent on a task,
    /// calculates the duration, and determines coins earned based on behavior type.
    public class TimeSession:BaseEntity
    {
        // Private constructor for EF Core
        private TimeSession() { }

        private TimeSession(
            Guid id,
            Guid userId,
            Guid taskId,
            DateTime startTime,
            BehaviorType behaviorType,
            bool manuallyAdded,
            string? notes) : base(id)
        {
            UserId = userId;
            TaskId = taskId;
            StartTime = startTime;
            BehaviorType = behaviorType;
            ManuallyAdded = manuallyAdded;
            Notes = notes;
            IsActive = !manuallyAdded;
            EndTime = null;
            DurationMinutes = 0;
            CoinsEarned = 0;
            PausedAt = null;
            TotalPausedDuration = TimeSpan.Zero;
        }

        // ═══════════════════════════════════════════════════════════════
        // PROPERTIES
        // ═══════════════════════════════════════════════════════════════

        /// <summary>Cross-module reference to User (no navigation property)</summary>
        public Guid UserId { get; private set; }

        /// <summary>Cross-module reference to Task (no navigation property)</summary>
        public Guid TaskId { get; private set; }

        public DateTime StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }
        public int DurationMinutes { get; private set; }
        public int CoinsEarned { get; private set; }
        public BehaviorType BehaviorType { get; private set; }
        public bool IsActive { get; private set; }
        public bool ManuallyAdded { get; private set; }
        public string? Notes { get; private set; }
        public DateTime? PausedAt { get; private set; }
        public TimeSpan TotalPausedDuration { get; private set; }

        // ═══════════════════════════════════════════════════════════════
        // FACTORY METHODS
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Starts a new live tracking session.
        /// </summary>
        public static TimeSession Start(Guid userId, Guid taskId, BehaviorType behaviorType, string? notes = null)
        {
            ValidateUserId(userId);
            ValidateTaskId(taskId);

            var session = new TimeSession(
                id: Guid.NewGuid(),
                userId: userId,
                taskId: taskId,
                startTime: DateTime.UtcNow,
                behaviorType: behaviorType,
                manuallyAdded: false,
                notes: notes);

            session.RaiseDomainEvent(new TimeSessionStartedEvent(session.Id, session.UserId, session.TaskId));
            return session;
        }

        /// <summary>
        /// Creates a manually added session (without live timer).
        /// </summary>
        public static TimeSession CreateManual(
            Guid userId,
            Guid taskId,
            DateTime startTime,
            DateTime endTime,
            BehaviorType behaviorType,
            string? notes = null)
        {
            ValidateUserId(userId);
            ValidateTaskId(taskId);

            var timeRange = TimeRange.Create(startTime, endTime);
            var duration = Duration.FromTimeRange(timeRange.StartTime, timeRange.EndTime);
            var coins = duration.CalculateCoins(behaviorType);

            var session = new TimeSession(
                id: Guid.NewGuid(),
                userId: userId,
                taskId: taskId,
                startTime: timeRange.StartTime,
                behaviorType: behaviorType,
                manuallyAdded: true,
                notes: notes);

            session.EndTime = timeRange.EndTime;
            session.DurationMinutes = (int)Math.Round(duration.TotalMinutes, MidpointRounding.AwayFromZero);
            session.CoinsEarned = coins;
            session.IsActive = false;
            session.PausedAt = null;
            session.TotalPausedDuration = TimeSpan.Zero;

            session.RaiseDomainEvent(new TimeSessionEndedEvent(session.Id, session.UserId, coins, session.DurationMinutes));
            
            if (coins != 0)
                session.RaiseDomainEvent(new CoinsEarnedEvent(userId, coins, session.Id));

            return session;
        }

        // ═══════════════════════════════════════════════════════════════
        // DOMAIN METHODS
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Stops the active session and calculates earned coins.
        /// </summary>
        public void Stop()
        {
            if (!IsActive)
                throw new InvalidOperationException("Cannot stop a session that is not active.");

            if (PausedAt.HasValue)
            {
                TotalPausedDuration += DateTime.UtcNow - PausedAt.Value;
                PausedAt = null;
            }

            var endTime = DateTime.UtcNow;
            var actualDurationSpan = (endTime - StartTime) - TotalPausedDuration;
            var duration = Duration.FromTimeRange(StartTime, StartTime.Add(actualDurationSpan));
            var coins = duration.CalculateCoins(BehaviorType);

            EndTime = endTime;
            DurationMinutes = (int)Math.Round(duration.TotalMinutes, MidpointRounding.AwayFromZero);
            CoinsEarned = coins;
            IsActive = false;

            MarkAsUpdated();
            RaiseDomainEvent(new TimeSessionEndedEvent(Id, UserId, coins, DurationMinutes));

            if (coins != 0)
            {
                // Event handled directly via Integration Service in the application layer
            }
        }

        public void Pause()
        {
            if (!IsActive)
                throw new InvalidOperationException("Cannot pause a session that is not active.");

            if (PausedAt.HasValue)
                throw new InvalidOperationException("Session is already paused.");

            PausedAt = DateTime.UtcNow;
            
            MarkAsUpdated();
            RaiseDomainEvent(new TimeSessionPausedEvent(Id, UserId));
        }

        public void Resume()
        {
            if (!IsActive)
                throw new InvalidOperationException("Cannot resume a session that is not active.");

            if (!PausedAt.HasValue)
                throw new InvalidOperationException("Session is not paused.");

            var pausedTime = DateTime.UtcNow - PausedAt.Value;
            TotalPausedDuration += pausedTime;
            PausedAt = null;

            MarkAsUpdated();
        }

        /// <summary>
        /// Updates session details (time range, notes, behavior type).
        /// Only allowed for completed sessions.
        /// </summary>
        public void Update(DateTime startTime, DateTime endTime, BehaviorType behaviorType, string? notes)
        {
            if (IsActive)
                throw new InvalidOperationException("Cannot update an active session. Stop it first.");

            var previousCoins = CoinsEarned;
            
            var timeRange = TimeRange.Create(startTime, endTime);
            var duration = Duration.FromTimeRange(timeRange.StartTime, timeRange.EndTime);
            var coins = duration.CalculateCoins(behaviorType);

            StartTime = timeRange.StartTime;
            EndTime = timeRange.EndTime;
            BehaviorType = behaviorType;
            DurationMinutes = (int)Math.Round(duration.TotalMinutes, MidpointRounding.AwayFromZero);
            CoinsEarned = coins;
            Notes = notes;

            MarkAsUpdated();

            var coinDifference = CoinsEarned - previousCoins;
            if (coinDifference != 0)
            {
                // Event handled directly via Integration Service in the application layer
            }
        }

        /// <summary>
        /// Updates only the notes for this session.
        /// </summary>
        public void UpdateNotes(string? notes)
        {
            Notes = notes;
            MarkAsUpdated();
        }

        /// <summary>
        /// Restores session from browser state after reconnect.
        /// Called when user returns after closing the browser.
        /// </summary>
        public void RecoverFromDisconnect(DateTime recoveryTime)
        {
            if (!IsActive)
                throw new InvalidOperationException("Session is not active, cannot recover.");

            if (PausedAt.HasValue)
            {
                TotalPausedDuration += recoveryTime > PausedAt.Value ? recoveryTime - PausedAt.Value : TimeSpan.Zero;
                PausedAt = null;
            }

            // Cap at 24 hours max
            var maxEnd = StartTime.AddHours(24);
            var endTime = recoveryTime > maxEnd ? maxEnd : recoveryTime;
            var actualDurationSpan = (endTime - StartTime) - TotalPausedDuration;
            var duration = Duration.FromTimeRange(StartTime, StartTime.Add(actualDurationSpan));
            var coins = duration.CalculateCoins(BehaviorType);

            EndTime = endTime;
            DurationMinutes = (int)Math.Round(duration.TotalMinutes, MidpointRounding.AwayFromZero);
            CoinsEarned = coins;
            IsActive = false;

            MarkAsUpdated();
            RaiseDomainEvent(new TimeSessionEndedEvent(Id, UserId, coins, DurationMinutes));

            if (coins != 0)
                RaiseDomainEvent(new CoinsEarnedEvent(UserId, coins, Id));
        }

        // ═══════════════════════════════════════════════════════════════
        // PRIVATE VALIDATION
        // ═══════════════════════════════════════════════════════════════

        private static void ValidateUserId(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        }

        private static void ValidateTaskId(Guid taskId)
        {
            if (taskId == Guid.Empty)
                throw new ArgumentException("TaskId cannot be empty.", nameof(taskId));
        }
    }
}
