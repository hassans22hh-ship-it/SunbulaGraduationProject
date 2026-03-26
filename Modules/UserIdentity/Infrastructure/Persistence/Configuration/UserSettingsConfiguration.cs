using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UserIdentityInfrastructure.Persistence.Configuration
{
    public sealed class UserSettingsConfiguration : IEntityTypeConfiguration<UserSettings>
    {
        public void Configure(EntityTypeBuilder<UserSettings> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.UserId).IsRequired();
            builder.Property(s => s.DefaultTaskView).HasMaxLength(50).IsRequired();
            builder.Property(s => s.Language).HasMaxLength(20).IsRequired();
            builder.Property(s => s.Theme).HasMaxLength(50).IsRequired();

            builder.HasOne(s => s.User)
                   .WithOne(u => u.Settings)
                   .HasForeignKey<UserSettings>(s => s.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
