using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema.Infrastructure.Persistence.Configurations;

public class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.ToTable("Languages");
        builder.HasKey(x => x.LanguageId);
        builder.Property(x => x.Name).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();
    }
}
