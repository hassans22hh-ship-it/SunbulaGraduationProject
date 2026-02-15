using Domain.Entities.ValueOpjects;
using SharedKernel;

namespace Domain.Entities
{
    public class User:BaseEntity

    {
      private  User() { }
        public User(Guid id,Email email,string firstName, string lastName, string ?phoneNumber )
        {
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            IsActive = true;
            IsEmailConfirmed = false;
        }
        public Email Email { get; private set; } = null!;
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string? PhoneNumber { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsEmailConfirmed { get; private set; }
        public DateTime? LastLoginAt { get; private set; }

    }
}
