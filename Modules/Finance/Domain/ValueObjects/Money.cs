namespace FinanceDomain.ValueObjects
{

    /// Represents a monetary amount with its currency.
    /// Ensures amount is non-negative and currency is valid ISO 4217 code.
    public sealed class Money:IEquatable<Money>
    {

        private static readonly HashSet<string> SupportedCurrencies =
            new(StringComparer.OrdinalIgnoreCase)
            {
            "SAR", "USD", "EUR", "GBP", "EGP", "AED", "KWD", "QAR", "BHD", "OMR", "JOD"
            };

        private Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency.ToUpperInvariant();
        }

        /// <summary>The numeric amount (can be negative for balance tracking).</summary>
        public decimal Amount { get; }

        /// <summary>ISO 4217 currency code (e.g., "SAR", "USD").</summary>
        public string Currency { get; }

        // ─── Factory ────────────────────────────────────────────────────────────

        /// <summary>Creates a Money value object.</summary>
        /// <param name="amount">The monetary amount.</param>
        /// <param name="currency">ISO 4217 currency code.</param>
        public static Money Create(decimal amount, string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency cannot be empty.", nameof(currency));

            if (!SupportedCurrencies.Contains(currency))
                throw new ArgumentException(
                    $"Currency '{currency}' is not supported. Supported: {string.Join(", ", SupportedCurrencies)}",
                    nameof(currency));

            return new Money(amount, currency);
        }

        // ─── Arithmetic helpers ──────────────────────────────────────────────────

        /// <summary>Returns a new Money with the given amount added.</summary>
        public Money Add(decimal amount) => new(Amount + amount, Currency);

        /// <summary>Returns a new Money with the given amount subtracted.</summary>
        public Money Subtract(decimal amount) => new(Amount - amount, Currency);

        // ─── Equality ───────────────────────────────────────────────────────────

        public bool Equals(Money? other)
        {
            if (other is null) return false;
            return Amount == other.Amount && Currency == other.Currency;
        }

        public override bool Equals(object? obj) => obj is Money m && Equals(m);
        public override int GetHashCode() => HashCode.Combine(Amount, Currency);
        public override string ToString() => $"{Amount:F2} {Currency}";

        public static bool operator ==(Money? left, Money? right) => left?.Equals(right) ?? right is null;
        public static bool operator !=(Money? left, Money? right) => !(left == right);
    }
}
