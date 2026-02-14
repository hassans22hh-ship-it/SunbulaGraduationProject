
using Domain.Entities.StorePlant;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations.StorePlant
{
    public class PlantConfiguration:IEntityTypeConfiguration<Plant>
    {
        public void Configure(EntityTypeBuilder<Plant> builder)
        {
            builder.ToTable("Plants");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.ImageUrl)
                .HasMaxLength(500);

            builder.Property(x => x.GrowthName)
                .HasMaxLength(150);

            builder.Property(x => x.Level).IsRequired();
            builder.Property(x => x.Points).IsRequired();

            builder.HasIndex(x => x.Name).IsUnique();
        }
    }
}
