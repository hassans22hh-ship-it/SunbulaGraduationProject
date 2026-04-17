namespace Application.UserDTO
{
    public sealed  record AuthREsponseDto
    {
        public required string AccessToken { get; init; }
        public required string RefreshToken { get; init; }
        public required DateTime ExpiresAt { get; init; }
        public required UserDto User { get; init; }
    }
}
