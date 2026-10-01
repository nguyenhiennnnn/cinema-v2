using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema.Infrastructure.Persistence.Configurations;

public class ScreenTypeConfiguration : IEntityTypeConfiguration<ScreenType>
{
    public void Configure(EntityTypeBuilder<ScreenType> builder)
    {
        builder.ToTable("ScreenTypes");
        builder.HasKey(x => x.ScreenTypeId);
        builder.Property(x => x.Name).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();
    }
}
