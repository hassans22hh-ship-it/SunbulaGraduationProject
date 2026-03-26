using SharedKernel;

namespace Domain.Entities
{
    public class UserSettings : BaseEntity
    {
        private UserSettings() { } // For EF Core

        private UserSettings(Guid id, Guid userId) : base(id)
        {
            UserId = userId;
            IsDailyReminderEnabled = true;
            DefaultTaskView = "List";
            Language = "en";
            Theme = "System";
        }

        public Guid UserId { get; private set; }
        public bool IsDailyReminderEnabled { get; private set; }
        public string DefaultTaskView { get; private set; } = "List";
        public string Language { get; private set; } = "en";
        public string Theme { get; private set; } = "System";

        // Navigation property
        public User? User { get; private set; }

        public static UserSettings Create(Guid userId)
        {
            return new UserSettings(Guid.NewGuid(), userId);
        }

        public void Update(bool isDailyReminderEnabled, string defaultTaskView, string language, string theme)
        {
            if (string.IsNullOrWhiteSpace(defaultTaskView))
                throw new ArgumentException("DefaultTaskView cannot be empty", nameof(defaultTaskView));
            if (string.IsNullOrWhiteSpace(language))
                throw new ArgumentException("Language cannot be empty", nameof(language));
            if (string.IsNullOrWhiteSpace(theme))
                throw new ArgumentException("Theme cannot be empty", nameof(theme));

            IsDailyReminderEnabled = isDailyReminderEnabled;
            DefaultTaskView = defaultTaskView;
            Language = language;
            Theme = theme;

            MarkAsUpdated();
        }
    }
}
