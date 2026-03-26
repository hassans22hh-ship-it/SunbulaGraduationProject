

using DebtDomain.Entities;
using DebtDomain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DebtInfrastructure.Persistenece.Configurations
{
    public sealed  class DebtPaymentConfiguration: IEntityTypeConfiguration<DebtPayment>
    {
        public void Configure(EntityTypeBuilder<DebtPayment> builder)
        {
            // Table
            builder.ToTable("DebtPayments", schema: "debt");

            // Primary Key
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedNever();

            // Properties
            builder.Property(e => e.DebtId)
                .IsRequired();

            // Mapping via HasConversion
            builder.Property(e => e.Amount)
                .HasConversion(v => v.Value, v => Money.Create(v))
                .HasColumnName("Amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            


            builder.Property(e => e.PaymentDate)
                .IsRequired();

            builder.Property(e => e.Notes)
                .HasMaxLength(500);

            // Audit fields
            builder.Property(e => e.CreatedAt).IsRequired();
            builder.Property(e => e.UpdatedAt);
            builder.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);

            // Indexes
            builder.HasIndex(e => e.DebtId)
                .HasDatabaseName("IX_DebtPayments_DebtId");

            builder.HasIndex(e => e.PaymentDate)
                .HasDatabaseName("IX_DebtPayments_PaymentDate");

            // Ignore domain events
            builder.Ignore(e => e.DomainEvents);
        }
    }
}
