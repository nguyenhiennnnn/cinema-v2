using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema.Infrastructure.Persistence.Configurations;

public class ActorConfiguration : IEntityTypeConfiguration<Actor>
{
    public void Configure(EntityTypeBuilder<Actor> builder)
    {
        builder.ToTable("Actors");
        builder.HasKey(x => x.ActorId);
        builder.Property(x => x.FullName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.AvatarUrl).HasMaxLength(255);
    }
}
