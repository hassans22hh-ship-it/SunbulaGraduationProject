using Domain.Entities.TaskManagement;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations.TaskManagement
{
    public class FolderConfiguration:IEntityTypeConfiguration<Folder>
    {
        public void Configure(EntityTypeBuilder<Folder> builder)
        {
            builder.ToTable("Folders");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(60);

            builder.Property(f => f.Color)
                .HasMaxLength(7);

            builder.HasIndex(f => new { f.UserId, f.Name })
                .IsUnique();
        }
    }
}
