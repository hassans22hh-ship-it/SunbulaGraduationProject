using FinanceDomain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceInfrastructure.Persistence.Configurations
{
    public class FinancialTransactionConfiguration: IEntityTypeConfiguration<FinancialTransaction>
    {
        public void Configure(EntityTypeBuilder<FinancialTransaction> builder)
        {
            builder.ToTable("FinancialTransactions", schema: "Finance");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).ValueGeneratedNever();

            builder.Property(t => t.UserId).IsRequired();
            builder.Property(t => t.WalletId).IsRequired();
            builder.Property(t => t.DestinationWalletId);
            builder.Property(t => t.FinancialCategoryId);

            builder.Property(t => t.Type)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(t => t.Amount)
                .HasPrecision(18, 4)
                .IsRequired();

            builder.Property(t => t.Currency)
                .HasMaxLength(3)
                .IsRequired();

            builder.Property(t => t.Description)
                .HasMaxLength(500);

            builder.Property(t => t.TransactionDate).IsRequired();

            builder.Property(t => t.CreatedAt).IsRequired();
            builder.Property(t => t.UpdatedAt);
            builder.Property(t => t.IsDeleted).IsRequired().HasDefaultValue(false);

            // Wallet relationship (source)
            builder.HasOne(t => t.Wallet)
                .WithMany(w => w.Transactions)
                .HasForeignKey(t => t.WalletId)
                .OnDelete(DeleteBehavior.Restrict);

            // Category relationship
            builder.HasOne(t => t.FinancialCategory)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.FinancialCategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            builder.HasIndex(t => t.UserId)
                .HasDatabaseName("IX_FinancialTransactions_UserId");

            builder.HasIndex(t => t.WalletId)
                .HasDatabaseName("IX_FinancialTransactions_WalletId");

            builder.HasIndex(t => t.TransactionDate)
                .HasDatabaseName("IX_FinancialTransactions_Date");

            builder.HasIndex(t => t.FinancialCategoryId)
                .HasDatabaseName("IX_FinancialTransactions_CategoryId");

            builder.Ignore(t => t.DomainEvents);
        }
    }

}
