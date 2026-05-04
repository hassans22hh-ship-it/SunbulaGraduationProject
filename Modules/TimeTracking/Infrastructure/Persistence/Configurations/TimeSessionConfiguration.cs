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
                .HasDefaultValue(0).HasConversion<long>();

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

            // PausedAt and TotalPausedDuration are persisted (columns added in 20260326230212_AddPauseFields)
            builder.Property(e => e.PausedAt);
            builder.Property(e => e.TotalPausedDuration)
                .HasConversion(
                    v => v.Ticks,          // TimeSpan → long (write)
                    v => TimeSpan.FromTicks(v)) // long → TimeSpan (read)
                .HasColumnType("bigint")
                .HasDefaultValue(TimeSpan.Zero);

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

            // BR-02: Filtered unique index — prevents duplicate active session for same (User, Task)
            // Applied in SQL directly (EF filtered index with WHERE clause)
            // See migration: AddUniqueActiveSessionIndex

            builder.Ignore(e => e.DomainEvents);
        }
    }
}
