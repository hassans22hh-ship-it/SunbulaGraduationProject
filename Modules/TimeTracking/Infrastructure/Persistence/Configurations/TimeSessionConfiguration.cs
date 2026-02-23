using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTrackingDomain.Entities;

namespace TimeTrackingInfrastructure.Persistence.Configurations
{
    public sealed class TimeSessionConfiguration : IEntityTypeConfiguration<TimeSession>
    {
        public void Configure(EntityTypeBuilder<TimeSession> builder)
        {
            builder.ToTable("TimeSessions", schema: "tracking");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.UserId).IsRequired();
            builder.Property(e => e.TaskId).IsRequired();

            builder.Property(e => e.StartTime).IsRequired();
            builder.Property(e => e.EndTime);

            builder.Property(e => e.DurationMinutes)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(e => e.CoinsEarned)
                .IsRequired()
                .HasPrecision(10, 2)
                .HasDefaultValue(0m);

            builder.Property(e => e.BehaviorType)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.ManuallyAdded)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.Notes)
                .HasMaxLength(500);

            // Audit
            builder.Property(e => e.CreatedAt).IsRequired();
            builder.Property(e => e.UpdatedAt);
            builder.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);

            // Indexes
            builder.HasIndex(e => e.UserId)
                .HasDatabaseName("IX_TimeSessions_UserId");

            builder.HasIndex(e => e.TaskId)
                .HasDatabaseName("IX_TimeSessions_TaskId");

            builder.HasIndex(e => new { e.UserId, e.IsActive })
                .HasDatabaseName("IX_TimeSessions_UserId_IsActive");

            builder.HasIndex(e => new { e.UserId, e.StartTime })
                .HasDatabaseName("IX_TimeSessions_UserId_StartTime");

            builder.HasIndex(e => e.StartTime)
                .HasDatabaseName("IX_TimeSessions_StartTime");

            builder.Ignore(e => e.DomainEvents);
        }
    }
}
