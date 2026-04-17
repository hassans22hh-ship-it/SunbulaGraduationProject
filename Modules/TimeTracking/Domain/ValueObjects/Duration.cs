namespace TimeTrackingDomain.ValueObjects
{
    /// Represents a duration with helpers for display and coin calculation.

    public class Duration: IEquatable<Duration>
    {

        private Duration(double totalMinutes)
        {
            TotalMinutes = totalMinutes;
        }

        public double TotalMinutes { get; }
        public int Hours => (int)(TotalMinutes / 60);
        public int Minutes => (int)(TotalMinutes % 60);
        public double TotalHours => TotalMinutes / 60.0;

        /// <summary>
        /// Creates a Duration from total minutes.
        /// </summary>
        public static Duration FromMinutes(double totalMinutes)
        {
            if (totalMinutes < 0)
                throw new ArgumentException("Duration cannot be negative.", nameof(totalMinutes));

            if (totalMinutes > 1440)
                throw new ArgumentException("Duration cannot exceed 24 hours (1440 minutes).", nameof(totalMinutes));

            return new Duration(totalMinutes);
        }

        /// <summary>
        /// Creates a Duration from a TimeRange.
        /// </summary>
        public static Duration FromTimeRange(DateTime startTime, DateTime endTime)
        {
            var minutes = (endTime - startTime).TotalMinutes;
            return FromMinutes(Math.Max(0.0, minutes));
        }

        /// <summary>
        /// Calculates coins earned based on behavior type.
        /// </summary>
        public int CalculateCoins(Enums.BehaviorType behaviorType)
        {
            var coinFactor = behaviorType switch
            {
                Enums.BehaviorType.Positive => 2.0,
                Enums.BehaviorType.Neutral => 1.0,
                Enums.BehaviorType.Rest => 1.0,
                Enums.BehaviorType.Negative => -1.0,
                _ => 0.0
            };

            return (int)Math.Round(TotalHours * coinFactor, MidpointRounding.AwayFromZero);
        }

        public static implicit operator double(Duration d) => d.TotalMinutes;

        public bool Equals(Duration? other)
        {
            if (other is null) return false;
            return Math.Abs(TotalMinutes - other.TotalMinutes) < 0.001;
        }

        public override bool Equals(object? obj) => obj is Duration d && Equals(d);
        public override int GetHashCode() => TotalMinutes.GetHashCode();
        public override string ToString() => $"{Hours}h {Minutes}m";

        public static bool operator ==(Duration? left, Duration? right) => left?.Equals(right) ?? right is null;
        public static bool operator !=(Duration? left, Duration? right) => !(left == right);
    }
}

