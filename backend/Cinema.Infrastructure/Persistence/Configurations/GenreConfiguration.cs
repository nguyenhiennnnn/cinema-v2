using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema.Infrastructure.Persistence.Configurations;

public class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.ToTable("Genres");
        builder.HasKey(x => x.GenreId);
        builder.Property(x => x.Name).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();
    }
}
