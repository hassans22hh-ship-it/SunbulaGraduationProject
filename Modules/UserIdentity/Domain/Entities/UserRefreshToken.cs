using SharedKernel;

namespace Domain.Entities
{
    public sealed class UserRefreshToken:BaseEntity
    {
        // Private constructor for EF Core
        private UserRefreshToken() { }

        private UserRefreshToken(Guid id, Guid userId, string token, DateTime expiresAt, string? deviceInfo)
            : base(id)
        {
            UserId = userId;
            Token = token;
            ExpiresAt = expiresAt;
            DeviceInfo = deviceInfo;
            IsRevoked = false;
        }

        public Guid UserId { get; private set; }
        public string Token { get; private set; } = string.Empty;
        public DateTime ExpiresAt { get; private set; }
        public string? DeviceInfo { get; private set; }
        public bool IsRevoked { get; private set; }
        public DateTime? RevokedAt { get; private set; }

        // Navigation property
        public User User { get; private set; } = null!;

        // Computed property
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsActive => !IsRevoked && !IsExpired;

        // Factory method
        public static UserRefreshToken Create(Guid userId, string token, DateTime expiresAt, string? deviceInfo = null)
        {
            return new UserRefreshToken(Guid.NewGuid(), userId, token, expiresAt, deviceInfo);
        }

        // Domain methods
        public void Revoke()
        {
            if (IsRevoked)
            {
                throw new InvalidOperationException("Token is already revoked");
            }

            IsRevoked = true;
            RevokedAt = DateTime.UtcNow;
            MarkAsUpdated();
        }

        public void Validate()
        {
            if (IsRevoked)
            {
                throw new InvalidOperationException("Token has been revoked");
            }

            if (IsExpired)
            {
                throw new InvalidOperationException("Token has expired");
            }
        }
    }
}
