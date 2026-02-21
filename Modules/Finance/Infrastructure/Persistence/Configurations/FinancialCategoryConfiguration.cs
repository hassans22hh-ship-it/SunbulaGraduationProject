using FinanceDomain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceInfrastructure.Persistence.Configurations
{
    public class FinancialCategoryConfiguration : IEntityTypeConfiguration<FinancialCategory>
    {
        public void Configure(EntityTypeBuilder<FinancialCategory> builder)
        {
            builder.ToTable("FinancialCategories", schema: "Finance");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedNever();

            builder.Property(c => c.UserId).IsRequired();

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.CreatedAt).IsRequired();
            builder.Property(c => c.UpdatedAt);
            builder.Property(c => c.IsDeleted).IsRequired().HasDefaultValue(false);

            builder.HasMany(c => c.Transactions)
                .WithOne(t => t.FinancialCategory)
                .HasForeignKey(t => t.FinancialCategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(c => c.UserId)
                .HasDatabaseName("IX_FinancialCategories_UserId");

            builder.HasIndex(c => new { c.UserId, c.Name })
                .IsUnique()
                .HasDatabaseName("IX_FinancialCategories_UserId_Name");

            builder.Ignore(c => c.DomainEvents);
        }
    }
}
