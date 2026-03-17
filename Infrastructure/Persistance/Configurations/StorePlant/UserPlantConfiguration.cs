

using Domain.Entities.StorePlant;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations.StorePlant
{
    public class UserPlantConfiguration:IEntityTypeConfiguration<UserPlant>
    {
        public void Configure(EntityTypeBuilder<UserPlant> builder)
        {
            builder.ToTable("UserPlants");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.PlantId).IsRequired();

            builder.Property(x => x.CoinsSpent).IsRequired();
            builder.Property(x => x.GrowthStage).IsRequired();

            builder.Property(x => x.PurchasedDate)
                .IsRequired();

            // Relationship: Plant 1 -> Many UserPlants
            builder.HasOne(x => x.Plant)
                .WithMany(p => p.UserPlants)
                .HasForeignKey(x => x.PlantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.UserId, x.PlantId });
            builder.HasIndex(x => x.UserId);
        }
    }
}
