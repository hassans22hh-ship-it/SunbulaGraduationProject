using FinanceDomain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceInfrastructure.Persistence.Configurations
{
    public sealed class WalletConfiguration: IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.ToTable("Wallets", schema: "Finance");

            builder.HasKey(w => w.Id);
            builder.Property(w => w.Id).ValueGeneratedNever();

            builder.Property(w => w.UserId).IsRequired();

            builder.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(w => w.Type)
                .HasConversion<string>()
                .IsRequired();

            // ── Money value object (owned entity) ─────────────────────────────
            builder.OwnsOne(w => w.Balance, balance =>
            {
                balance.Property(m => m.Amount)
                    .HasColumnName("Balance")
                    .HasPrecision(18, 4)
                    .IsRequired();

                balance.Property(m => m.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            // Audit
            builder.Property(w => w.CreatedAt).IsRequired();
            builder.Property(w => w.UpdatedAt);
            builder.Property(w => w.IsDeleted).IsRequired().HasDefaultValue(false);

            // Relationships
            builder.HasMany(w => w.Transactions)
                .WithOne(t => t.Wallet)
                .HasForeignKey(t => t.WalletId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(w => w.UserId)
                .HasDatabaseName("IX_Wallets_UserId");

            builder.HasIndex(w => new { w.UserId, w.Name })
                .IsUnique()
                .HasFilter("[IsDeleted] = 0")
                .HasDatabaseName("IX_Wallets_UserId_Name");

            builder.Ignore(w => w.DomainEvents);
        }
    }
}
