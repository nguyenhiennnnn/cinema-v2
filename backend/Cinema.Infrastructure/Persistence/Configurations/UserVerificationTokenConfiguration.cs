using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema.Infrastructure.Persistence.Configurations;

public class UserVerificationTokenConfiguration : IEntityTypeConfiguration<UserVerificationToken>
{
    public void Configure(EntityTypeBuilder<UserVerificationToken> builder)
    {
        builder.ToTable("UserVerificationTokens");
        builder.HasKey(x => x.TokenId);
        builder.Property(x => x.TokenHash).HasMaxLength(255).IsRequired();
        builder
            .HasIndex(x => x.TokenHash)
            .IsUnique()
            .HasDatabaseName("UQ_UserVerificationTokens_TokenHash");
        builder.Property(x => x.Purpose).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.ExpiresAt).HasColumnType("timestamp without time zone");
        builder.Property(x => x.Used).HasDefaultValue(false);
        builder
            .Property(x => x.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("NOW()");

        builder
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasIndex(x => new { x.UserId, x.Purpose })
            .HasDatabaseName("IX_UserVerificationTokens_User_Purpose");
    }
}
