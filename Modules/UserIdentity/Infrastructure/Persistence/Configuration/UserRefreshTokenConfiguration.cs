using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration
{
    public class UserRefreshTokenConfiguration : IEntityTypeConfiguration<UserRefreshToken>
    {
        public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
        {
            // Table
            builder.ToTable("UserRefreshTokens", schema: "Identity");

            // Primary Key
            builder.HasKey(rt => rt.Id);
            builder.Property(rt => rt.Id).ValueGeneratedNever();

            // Properties
            builder.Property(rt => rt.UserId).IsRequired();
            builder.Property(rt => rt.Token).IsRequired().HasMaxLength(512);
            builder.Property(rt => rt.ExpiresAt).IsRequired();
            builder.Property(rt => rt.DeviceInfo).HasMaxLength(500);
            builder.Property(rt => rt.IsRevoked).IsRequired().HasDefaultValue(false);
            builder.Property(rt => rt.RevokedAt);
            builder.Property(rt => rt.CreatedAt).IsRequired();
            builder.Property(rt => rt.UpdatedAt);
            builder.Property(rt => rt.IsDeleted).IsRequired().HasDefaultValue(false);

            // Indexes
            builder.HasIndex(rt => rt.Token)
                .IsUnique()
                .HasDatabaseName("IX_UserRefreshTokens_Token");

            builder.HasIndex(rt => rt.UserId)
                .HasDatabaseName("IX_UserRefreshTokens_UserId");

            builder.HasIndex(rt => rt.ExpiresAt)
                .HasDatabaseName("IX_UserRefreshTokens_ExpiresAt");

            builder.HasIndex(rt => rt.IsRevoked)
                .HasDatabaseName("IX_UserRefreshTokens_IsRevoked");

            builder.HasIndex(rt => new { rt.UserId, rt.IsRevoked, rt.ExpiresAt })
                .HasDatabaseName("IX_UserRefreshTokens_Composite");

            // Ignore computed properties
            builder.Ignore(rt => rt.IsExpired);
            builder.Ignore(rt => rt.IsActive);

            // Ignore domain events
            builder.Ignore(rt => rt.DomainEvents);
        }
    }
}
