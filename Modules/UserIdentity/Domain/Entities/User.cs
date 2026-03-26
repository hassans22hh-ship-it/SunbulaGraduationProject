using Domain.Entities.ValueOpjects;
using SharedKernel;

namespace Domain.Entities
{
    public class User : BaseEntity

    {
        private List<UserRefreshToken> _refreshTokens = new();
        private string _email = string.Empty; // Backing field for EF Core persistence
        // Validation
        private static void ValidateName(string name, string paramName)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"{paramName} cannot be empty", paramName);
            }

            if (name.Length < 2)
            {
                throw new ArgumentException($"{paramName} must be at least 2 characters", paramName);
            }

            if (name.Length > 50)
            {
                throw new ArgumentException($"{paramName} cannot exceed 50 characters", paramName);
            }
        }
        private User() { }

        private User(Guid id, Email email, string firstName, string passwordHash, string lastName, string? phoneNumber)
            : base(id)
        {
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            IsActive = true;
            IsEmailConfirmed = false;
            CoinBalance = 0;
            ConsecutiveStreakDays = 0;
            PasswordHash = passwordHash;
        }

        public Email Email
        {
            get => Email.Create(_email);
            private set => _email = value.Value;
        }
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string? PhoneNumber { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsEmailConfirmed { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public string PasswordHash { get; private set; } = string.Empty;

        // Gamification properties
        public int CoinBalance { get; private set; }
        public int ConsecutiveStreakDays { get; private set; }
        public DateTime? LastStreakDate { get; private set; }
        public string AwardedMilestones { get; private set; } = string.Empty;
        // Navigation properties
        public IReadOnlyCollection<UserRefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
        //Factory method 
        public static User Create(Email email, string firstName, string lastName, string passwordHash, string? phoneNumber)
        {

            ValidateName(firstName, nameof(firstName));
            ValidateName(lastName, nameof(lastName));

            var user = new User(Guid.NewGuid(), email, firstName, passwordHash, lastName, phoneNumber);

            user.RaiseDomainEvent(new UserRegisteredEvent(user.Id, user.Email.Value));

            return user;
        }
        // Domain methods
        public void UpdateProfile(string firstName, string lastName, string? phoneNumber)
        {
            ValidateName(firstName, nameof(firstName));
            ValidateName(lastName, nameof(lastName));

            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            MarkAsUpdated();
        }
        public void ConfirmEmail()
        {
            if (IsEmailConfirmed)
            {
                throw new InvalidOperationException("Email is already confirmed");
            }

            IsEmailConfirmed = true;
            MarkAsUpdated();
        }
        public void UpdatePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("Password hash cannot be empty", nameof(newPasswordHash));

            PasswordHash = newPasswordHash;
            MarkAsUpdated();
        }
        public void Deactivate()
        {
            if (!IsActive)
            {
                throw new InvalidOperationException("User is already deactivated");
            }

            IsActive = false;
            MarkAsUpdated();
        }

        public void Activate()
        {
            if (IsActive)
            {
                throw new InvalidOperationException("User is already active");
            }

            IsActive = true;
            MarkAsUpdated();
        }
        public void RecordLogin()
        {
            LastLoginAt = DateTime.UtcNow;
            MarkAsUpdated();
            RaiseDomainEvent(new UserLoggedInEvent(Id, Email.Value, LastLoginAt.Value));
        }
        public UserRefreshToken AddRefreshToken(string token, DateTime expiresAt, string? deviceInfo = null)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("Token cannot be empty", nameof(token));
            }

            if (expiresAt <= DateTime.UtcNow)
            {
                throw new ArgumentException("Expiration date must be in the future", nameof(expiresAt));
            }

            var refreshToken = UserRefreshToken.Create(Id, token, expiresAt, deviceInfo);
            _refreshTokens.Add(refreshToken);

            return refreshToken;
        }
        public void RevokeRefreshToken(Guid refreshTokenId)
        {
            var token = _refreshTokens.FirstOrDefault(t => t.Id == refreshTokenId);
            if (token == null)
            {
                throw new InvalidOperationException("Refresh token not found");
            }
            token.Revoke();
        }
        public void RevokeAllRefreshTokens()
        {
            foreach (var token in _refreshTokens.Where(rt => rt.IsActive))
            {
                token.Revoke();
            }
        }

        public string GetFullName() => $"{FirstName} {LastName}";
        public void AddCoins(int amount, string reason)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive", nameof(amount));

            var previousBalance = CoinBalance;
            CoinBalance += amount;
            MarkAsUpdated();

            RaiseDomainEvent(new CoinBalanceChangedEvent(Id, previousBalance, CoinBalance, amount, reason));
        }

        public void AwardStreakMilestone(int milestoneDays, int coins)
        {
            var milestoneStr = milestoneDays.ToString();
            var awarded = AwardedMilestones.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (awarded.Contains(milestoneStr)) return; // Already awarded

            AwardedMilestones = string.IsNullOrEmpty(AwardedMilestones) 
                ? milestoneStr 
                : $"{AwardedMilestones},{milestoneStr}";
                
            AddCoins(coins, $"Streak Bonus: {milestoneDays} days");
        }

        public void SpendCoins(int amount, string reason)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive", nameof(amount));

            if (CoinBalance < amount)
                throw new InvalidOperationException($"Insufficient coin balance. Current: {CoinBalance}, Required: {amount}");

            var previousBalance = CoinBalance;
            CoinBalance -= amount;
            MarkAsUpdated();

            RaiseDomainEvent(new CoinBalanceChangedEvent(Id, previousBalance, CoinBalance, -amount, reason));
        }
    }
}
