using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DebtDomain.ValueObjects;

namespace DebtInfrastructure.Persistenece.Configurations
{
    public sealed class DebtConfiguration : IEntityTypeConfiguration<DebtDomain.Entities.Debt>
    {
        public void Configure(EntityTypeBuilder<DebtDomain.Entities.Debt> builder)
        {
            // Table
            builder.ToTable("Debts", schema: "debt");

            // Primary Key
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedNever();

            // Properties
            builder.Property(e => e.CreditorName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.UserId)
                .IsRequired();

            // Mapping via HasConversion (Robust for same-type multiple properties)
            builder.Property(e => e.Amount)
                .HasConversion(v => v.Value, v => Money.Create(v))
                .HasColumnName("Amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(e => e.RemainingAmount)
                .HasConversion(v => v.Value, v => Money.Create(v))
                .HasColumnName("RemainingAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(e => e.DueDate)
                .IsRequired();

            builder.Property(e => e.IsPaid)
                .IsRequired()
                .HasDefaultValue(false);

            // Enum
            builder.Property(e => e.DebtType)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(e => e.Notes)
                .HasMaxLength(500);

            // Audit fields
            builder.Property(e => e.CreatedAt).IsRequired();
            builder.Property(e => e.UpdatedAt);
            builder.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);

            // Relationships
            builder.HasMany(e => e.Payments)
                .WithOne(p => p.Debt)
                .HasForeignKey(p => p.DebtId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(e => e.UserId)
                .HasDatabaseName("IX_Debts_UserId");

            builder.HasIndex(e => e.IsPaid)
                .HasDatabaseName("IX_Debts_IsPaid");

            builder.HasIndex(e => e.DueDate)
                .HasDatabaseName("IX_Debts_DueDate");

            builder.HasIndex(e => new { e.UserId, e.DebtType })
                .HasDatabaseName("IX_Debts_UserId_DebtType");

            builder.HasIndex(e => new { e.UserId, e.IsPaid, e.DueDate })
                .HasDatabaseName("IX_Debts_UserId_IsPaid_DueDate");

            // Ignore domain events
            builder.Ignore(e => e.DomainEvents);

            // Ignore computed property
            builder.Ignore(e => e.IsOverdue);
        }
    }
}
