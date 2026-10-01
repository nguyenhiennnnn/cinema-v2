using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(x => x.RefreshTokenId);
        builder.Property(x => x.TokenHash).HasMaxLength(255).IsRequired();
        builder.HasIndex(x => x.TokenHash).IsUnique().HasDatabaseName("UQ_RefreshTokens_TokenHash");
        builder.Property(x => x.ExpiresAt).HasColumnType("timestamp without time zone");
        builder.Property(x => x.IsRevoked).HasDefaultValue(false);
        builder
            .Property(x => x.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("NOW()");
        builder.Property(x => x.RevokedAt).HasColumnType("timestamp without time zone");

        builder
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => x.UserId).HasDatabaseName("IX_RefreshTokens_User");

        builder
            .HasOne(x => x.ReplacedByToken)
            .WithMany()
            .HasForeignKey(x => x.ReplacedByTokenId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
