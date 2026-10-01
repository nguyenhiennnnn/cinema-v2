using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema.Infrastructure.Persistence.Configurations;

public class MembershipTierConfiguration : IEntityTypeConfiguration<MembershipTier>
{
    public void Configure(EntityTypeBuilder<MembershipTier> builder)
    {
        builder.ToTable(
            "MembershipTiers",
            t =>
                t.HasCheckConstraint(
                    "CK_MembershipTiers_Discount",
                    "\"DiscountPercent\" BETWEEN 0 AND 100"
                )
        );
        builder.HasKey(x => x.TierId);
        builder.Property(x => x.TierName).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.TierName).IsUnique();
        builder.Property(x => x.MinPoints).HasDefaultValue(0);
        builder.Property(x => x.DiscountPercent).HasColumnType("decimal(5,2)").HasDefaultValue(0m);
    }
}
