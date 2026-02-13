using System.Text.RegularExpressions;

namespace Domain.Entities.ValueOpjects
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

        public bool Equals(Email? other)
        {
            throw new NotImplementedException();
        }
    }
}
