namespace Application.UserDTO
{
    public class UserDto
    {
        public Guid UserId { get; init; }
        public string Email { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string FullName { get; init; } = string.Empty;
        public string? Phone { get; init; }
        public bool IsActive { get; init; }
        public bool IsEmailConfirmed { get; init; }
        public int CoinBalance { get; init; }
        public int ConsecutiveStreakDays { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? LastLoginAt { get; init; }
        public string Role { get; init; } = string.Empty;
    }
}
