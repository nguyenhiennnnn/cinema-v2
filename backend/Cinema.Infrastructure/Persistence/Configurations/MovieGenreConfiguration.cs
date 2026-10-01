using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema.Infrastructure.Persistence.Configurations;

public class MovieGenreConfiguration : IEntityTypeConfiguration<MovieGenre>
{
    public void Configure(EntityTypeBuilder<MovieGenre> builder)
    {
        builder.ToTable("MovieGenres");
        builder.HasKey(x => new { x.MovieId, x.GenreId });

        builder
            .HasOne(x => x.Movie)
            .WithMany(m => m.MovieGenres)
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Genre)
            .WithMany(g => g.MovieGenres)
            .HasForeignKey(x => x.GenreId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
