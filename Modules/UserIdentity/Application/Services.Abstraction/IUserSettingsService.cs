using Application.UserDTO;

namespace Application.Services.Abstraction
{
    public interface IUserSettingsService
    {
        Task<UserSettingsDto> GetSettingsAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<UserSettingsDto> UpdateSettingsAsync(Guid userId, UpdateUserSettingsDto dto, CancellationToken cancellationToken = default);
    }
}
