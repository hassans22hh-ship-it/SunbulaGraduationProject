namespace TimeTrackingDomain.ValueObjects
{
    /// Represents a validated time range with start and end times.
    /// Ensures end time is after start time and prevents invalid states.
   
    public sealed class TimeRange: IEquatable<TimeRange>
    {

        private TimeRange(DateTime startTime, DateTime endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        public DateTime StartTime { get; }
        public DateTime EndTime { get; }

        /// <summary>
        /// Duration of the time range in minutes.
        /// </summary>
        public int DurationMinutes => (int)(EndTime - StartTime).TotalMinutes;

        /// <summary>
        /// Duration of the time range in hours (decimal).
        /// </summary>
        public double DurationHours => (EndTime - StartTime).TotalHours;

        /// <summary>
        /// Creates a validated TimeRange.
        /// </summary>
        public static TimeRange Create(DateTime startTime, DateTime endTime)
        {
            if (startTime >= endTime)
                throw new ArgumentException("End time must be after start time.");

            if ((endTime - startTime).TotalHours > 24)
                throw new ArgumentException("A single session cannot exceed 24 hours.");

            return new TimeRange(startTime.ToUniversalTime(), endTime.ToUniversalTime());
        }

        /// <summary>
        /// Creates a TimeRange from start time only (for active sessions).
        /// </summary>
        public static TimeRange CreateOpen(DateTime startTime)
        {
            if (startTime > DateTime.UtcNow.AddMinutes(1))
                throw new ArgumentException("Start time cannot be in the future.");

            return new TimeRange(startTime.ToUniversalTime(), startTime.ToUniversalTime().AddSeconds(1));
        }

        /// <summary>
        /// Checks if this range overlaps with another range.
        /// </summary>
        public bool OverlapsWith(TimeRange other)
        {
            return StartTime < other.EndTime && EndTime > other.StartTime;
        }

        public bool Equals(TimeRange? other)
        {
            if (other is null) return false;
            return StartTime == other.StartTime && EndTime == other.EndTime;
        }

        public override bool Equals(object? obj) => obj is TimeRange tr && Equals(tr);
        public override int GetHashCode() => HashCode.Combine(StartTime, EndTime);
        public override string ToString() => $"{StartTime:HH:mm} - {EndTime:HH:mm}";

        public static bool operator ==(TimeRange? left, TimeRange? right) => left?.Equals(right) ?? right is null;
        public static bool operator !=(TimeRange? left, TimeRange? right) => !(left == right);
    }
}

