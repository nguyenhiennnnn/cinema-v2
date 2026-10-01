using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema.Infrastructure.Persistence.Configurations;

public class DirectorConfiguration : IEntityTypeConfiguration<Director>
{
    public void Configure(EntityTypeBuilder<Director> builder)
    {
        builder.ToTable("Directors");
        builder.HasKey(x => x.DirectorId);
        builder.Property(x => x.FullName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.AvatarUrl).HasMaxLength(255);
    }
}
