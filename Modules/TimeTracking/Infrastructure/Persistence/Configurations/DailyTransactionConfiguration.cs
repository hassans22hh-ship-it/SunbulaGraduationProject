using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTrackingDomain.Entities;

namespace TimeTrackingInfrastructure.Persistence.Configurations
{
    public sealed class DailyTransactionConfiguration: IEntityTypeConfiguration<DailyTransaction>
    {
        public void Configure(EntityTypeBuilder<DailyTransaction> builder)
        {
            builder.ToTable("DailyTransactions", schema: "tracking");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.UserId).IsRequired();

            builder.Property(e => e.Date)
                .IsRequired()
                .HasConversion(
                    d => d.ToDateTime(TimeOnly.MinValue),
                    dt => DateOnly.FromDateTime(dt));

            builder.Property(e => e.TotalMinutes)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(e => e.TotalCoins)
                .IsRequired()
                .HasPrecision(10, 2)
                .HasDefaultValue(0m);

            builder.Property(e => e.SessionCount)
                .IsRequired()
                .HasDefaultValue(0);

            // Audit
            builder.Property(e => e.CreatedAt).IsRequired();
            builder.Property(e => e.UpdatedAt);
            builder.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);

            // Unique: one record per user per day
            builder.HasIndex(e => new { e.UserId, e.Date })
                .IsUnique()
                .HasDatabaseName("IX_DailyTransactions_UserId_Date");

            builder.HasIndex(e => e.UserId)
                .HasDatabaseName("IX_DailyTransactions_UserId");

            builder.Ignore(e => e.DomainEvents);
        }
    }
}
