using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlantDomain.Entities;

namespace PlantInfrastructure.Persistence.Data.Configurations
{
    public class UserPlantConfiguration:IEntityTypeConfiguration<UserPlant>
    {
        public void Configure(EntityTypeBuilder<UserPlant> builder)
        {
            builder.ToTable("UserPlants", schema: "store");

            builder.HasKey(up => up.Id);
            builder.Property(up => up.Id).ValueGeneratedNever();

            builder.Property(up => up.UserId).IsRequired();
            builder.Property(up => up.PlantId).IsRequired();

            builder.Property(up => up.CoinsSpent).IsRequired();
            builder.Property(up => up.PurchaseDate).IsRequired();

            builder.Property(up => up.CurrentStage)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(up => up.StageCoinsAccumulated)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(up => up.CreatedAt).IsRequired();
            builder.Property(up => up.UpdatedAt);
            builder.Property(up => up.IsDeleted).IsRequired().HasDefaultValue(false);

            builder.HasOne(up => up.Plant)
                .WithMany(p => p.UserPlants)
                .HasForeignKey(up => up.PlantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(up => up.GrowthHistories)
                .WithOne(gh => gh.UserPlant)
                .HasForeignKey(gh => gh.UserPlantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Business rule: user can own each plant only once
            builder.HasIndex(up => new { up.UserId, up.PlantId })
                .IsUnique()
                .HasDatabaseName("IX_UserPlants_UserId_PlantId");

            builder.HasIndex(up => up.UserId)
                .HasDatabaseName("IX_UserPlants_UserId");

            builder.HasIndex(up => up.PurchaseDate)
                .HasDatabaseName("IX_UserPlants_PurchaseDate");

            builder.Ignore(up => up.DomainEvents);
        }
    }
}
