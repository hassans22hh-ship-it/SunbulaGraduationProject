using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskDomain.Entities.TaskManagement;

namespace TaskInfrastructure.Persistence.Configurations
{
    internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.OwnsOne(c => c.Color, color =>
            {
                color.Property(c => c.Value).HasColumnName("Color");
            });
        }
    }
}
