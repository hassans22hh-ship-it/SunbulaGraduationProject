using Application.UserDTO;

namespace Application.Services.Abstraction
{
    public interface IAuthenticationService
    {
        Task<AuthREsponseDto> RegisterAsync (RegisterDto registerDto,CancellationToken cancellationToken=default);
        Task<AuthREsponseDto> LoginAsync (LoginDto loginDto,string ? deviceInfo=null,CancellationToken cancellationToken=default);
            Task<AuthREsponseDto> RefreshTokenAsync (string refreshToken,string ? deviceInfo=null,CancellationToken cancellationToken=default);
        Task LogoutAsync(Guid UserId,string? refreshToken=null,CancellationToken cancellationToken = default); 
        Task <UserDto> GetUserProfileAsync (Guid userId,CancellationToken cancellationToken=default);
        Task<UserDto> UpdateProfileAsync (Guid userId, UpdateProfileDto updateProfileDto,CancellationToken cancellationToken=default);
        Task ConfirmEmailAsync(string token, CancellationToken cancellationToken = default);
        Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto, CancellationToken cancellationToken = default);
        Task DeleteAccountAsync(Guid userId, DeleteAccountDto dto, CancellationToken cancellationToken = default);
        Task<UserDto> ResetCoinsAsync(Guid userId, ResetCoinsDto dto, CancellationToken cancellationToken = default);
    }
}
