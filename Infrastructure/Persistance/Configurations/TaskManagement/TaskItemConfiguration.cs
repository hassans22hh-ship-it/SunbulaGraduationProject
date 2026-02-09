using Domain.Entities.TaskManagement;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Persistance.Configurations.TaskManagement
{
    public class TaskItemConfiguration:IEntityTypeConfiguration<TaskItem>
    {
        public void Configure(EntityTypeBuilder<TaskItem> builder)
        {
            builder.ToTable("Tasks");

            builder.HasKey(t =>t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Color)
                .HasMaxLength(7);

            builder.Property(t => t.Behavior)
                .IsRequired();

            builder.HasOne(t => t.Folder)
                .WithMany(f => f.Tasks)
                .HasForeignKey(t => t.FolderId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(t => new { t.UserId, t.Name });
        }

    }
}
