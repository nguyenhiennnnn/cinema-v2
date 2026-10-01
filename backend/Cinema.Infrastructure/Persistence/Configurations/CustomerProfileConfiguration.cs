using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema.Infrastructure.Persistence.Configurations;

public class CustomerProfileConfiguration : IEntityTypeConfiguration<CustomerProfile>
{
    public void Configure(EntityTypeBuilder<CustomerProfile> builder)
    {
        builder.ToTable("CustomerProfiles");
        builder.HasKey(x => x.UserId);
        builder.Property(x => x.UserId).ValueGeneratedNever();
        builder.Property(x => x.Points).HasDefaultValue(0);
        builder.Property(x => x.TotalSpent).HasColumnType("decimal(18,2)").HasDefaultValue(0m);

        builder
            .HasOne(x => x.User)
            .WithOne(u => u.CustomerProfile)
            .HasForeignKey<CustomerProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Tier)
            .WithMany(t => t.CustomerProfiles)
            .HasForeignKey(x => x.TierId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.TierId).HasDatabaseName("IX_CustomerProfiles_Tier");
    }
}
