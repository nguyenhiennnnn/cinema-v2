using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema.Infrastructure.Persistence.Configurations;

public class ComboConfiguration : IEntityTypeConfiguration<Combo>
{
    public void Configure(EntityTypeBuilder<Combo> builder)
    {
        builder.ToTable("Combos", t => t.HasCheckConstraint("CK_Combos_Price", "\"Price\" >= 0"));
        builder.HasKey(x => x.ComboId);
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
        builder.Property(x => x.ImageUrl).HasMaxLength(255);
        builder.Property(x => x.IsActive).HasDefaultValue(true);
    }
}
