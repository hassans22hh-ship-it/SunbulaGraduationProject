namespace DebtDomain.ValueObjects
{
    public class Money:IEquatable<Money>
    {
        private Money() { }
        public Money(decimal value)
        {
            Value = value;
        }

        public decimal Value { get; }

        /// Factory method with validation.
        public static Money Create(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));

            if (amount > 999999999.99m)
                throw new ArgumentException("Amount is too large", nameof(amount));

            // Round to 2 decimal places
            var rounded = Math.Round(amount, 2);

            return new Money(rounded);
        }

        /// <summary>
        /// Creates zero amount.
        /// </summary>
        public static Money Zero => new(0);

        // Implicit conversion
        public static implicit operator decimal(Money money) => money.Value;

        // Arithmetic operators
        public static Money operator +(Money left, Money right) =>
            Create(left.Value + right.Value);

        public static Money operator -(Money left, Money right) =>
            Create(left.Value - right.Value);

        // IEquatable implementation
        public bool Equals(Money? other)
        {
            if (other is null) return false;
            return Value == other.Value;
        }

        public override bool Equals(object? obj) => obj is Money money && Equals(money);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value.ToString("F2");

        public static bool operator ==(Money? left, Money? right) =>
            left?.Equals(right) ?? right is null;
        public static bool operator !=(Money? left, Money? right) => !(left == right);
    }
}
