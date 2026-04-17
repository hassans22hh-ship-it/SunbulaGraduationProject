using System.Text.RegularExpressions;

namespace Domain.Entities.ValueObjects
{
    public sealed class Email : IEquatable<Email>
    {
        private static readonly Regex EmailRegex = new(
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                     RegexOptions.Compiled | RegexOptions.IgnoreCase
            );
        private Email(string value)
        {
            Value = value;
        }
        public string Value { get; }
        public static Email Create(string email) 
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));
            }
            var normalizedEmail = email.Trim().ToLowerInvariant();
            if(!EmailRegex.IsMatch(normalizedEmail))
            {
                throw new ArgumentException("Invalid email format.", nameof(email));
            }
            if(normalizedEmail.Length > 254)
            {
                throw new ArgumentException("Email cannot be longer than 254 characters.", nameof(email));
            }   
            return new Email(normalizedEmail);
        }
        public static implicit operator string(Email email) => email.Value;
     public   override  string ToString() => Value;
        public bool Equals(Email? other)
        {
            if(other is null) return false;
            if(ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }
        public override bool Equals(object? obj)
     =>obj is Email email && Equals(email);
        public static bool operator ==(Email? left, Email? right) =>
     left?.Equals(right) ?? right is null;
        public override int GetHashCode() => Value.GetHashCode();
        public static bool operator !=(Email? left, Email? right) => !(left == right);
    }
}
