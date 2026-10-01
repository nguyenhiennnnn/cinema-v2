using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(
            "Users",
            t => t.HasCheckConstraint("CK_Users_Gender", "\"Gender\" IN ('Male','Female','Other')")
        );
        builder.HasKey(x => x.UserId);
        builder.Property(x => x.FullName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(150).IsRequired();
        builder.HasIndex(x => x.Email).IsUnique();
        builder.Property(x => x.PasswordHash).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Phone).HasMaxLength(20);
        builder.Property(x => x.Avatar).HasMaxLength(255);
        builder.Property(x => x.GoogleId).HasMaxLength(64);
        builder
            .HasIndex(x => x.GoogleId)
            .IsUnique()
            .HasFilter("\"GoogleId\" IS NOT NULL")
            .HasDatabaseName("IX_Users_GoogleId");
        builder.Property(x => x.DateOfBirth).HasColumnType("date");
        builder.Property(x => x.Gender).HasConversion<string>().HasMaxLength(10);
        builder.Property(x => x.IsActive).HasDefaultValue(true);
        builder.Property(x => x.EmailConfirmed).HasDefaultValue(false);
        builder
            .Property(x => x.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("NOW()");
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamp without time zone");

        builder
            .HasOne(x => x.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.RoleId).HasDatabaseName("IX_Users_Role");
    }
}
