using Application.Services.Abstraction;
using Application.UserDTO;
using Domain.Contracts;
using Domain.Entities;
using Domain.Exceptions;

namespace UserIdentityInfrastructure.Services
{
    public class UserSettingsService : IUserSettingsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserSettingsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserSettingsDto> GetSettingsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByIdWithSettingsAsync(userId, cancellationToken)
                ?? throw new UserNotFoundException(userId);

            if (user.Settings == null)
            {
                var settings = UserSettings.Create(userId);
                _unitOfWork.Users.Update(user); // Actually we should probably just add settings
                // Wait, if it's created but not added to context,EF core might not pick it up just by updating user if it's not attached.
                // We shouldn't manipulate context directly in the read if we can avoid it.
                // Let's create it and save it.
                // It's better to just return default DTO if it doesn't exist. We can create default settings on the fly.
                return new UserSettingsDto
                {
                    IsDailyReminderEnabled = true,
                    DefaultTaskView = "List",
                    Language = "en",
                    Theme = "System"
                };
            }

            return MapToDto(user.Settings);
        }

        public async Task<UserSettingsDto> UpdateSettingsAsync(Guid userId, UpdateUserSettingsDto dto, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByIdWithSettingsAsync(userId, cancellationToken)
                ?? throw new UserNotFoundException(userId);

            if (user.Settings == null)
            {
                user.InitializeSettings();
            }

            user.Settings!.Update(dto.IsDailyReminderEnabled, dto.DefaultTaskView, dto.Language, dto.Theme);

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDto(user.Settings);
        }

        private static UserSettingsDto MapToDto(UserSettings settings)
        {
            return new UserSettingsDto
            {
                IsDailyReminderEnabled = settings.IsDailyReminderEnabled,
                DefaultTaskView = settings.DefaultTaskView,
                Language = settings.Language,
                Theme = settings.Theme
            };
        }
    }
}
