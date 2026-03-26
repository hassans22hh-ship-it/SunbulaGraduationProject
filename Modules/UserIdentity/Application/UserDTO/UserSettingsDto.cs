namespace Application.UserDTO
{
    public class UserSettingsDto
    {
        public bool IsDailyReminderEnabled { get; set; }
        public string DefaultTaskView { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Theme { get; set; } = string.Empty;
    }

    public class UpdateUserSettingsDto
    {
        public bool IsDailyReminderEnabled { get; set; }
        public string DefaultTaskView { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Theme { get; set; } = string.Empty;
    }
}
