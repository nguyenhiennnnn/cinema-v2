using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema.Infrastructure.Persistence.Configurations;

public class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.ToTable(
            "Movies",
            t =>
            {
                t.HasCheckConstraint("CK_Movies_Duration", "\"DurationMinutes\" > 0");
                t.HasCheckConstraint(
                    "CK_Movies_Status",
                    "\"Status\" IN ('ComingSoon','NowShowing','Ended')"
                );
                t.HasCheckConstraint("CK_Movies_AverageRating", "\"AverageRating\" BETWEEN 0 AND 5");
            }
        );
        builder.HasKey(x => x.MovieId);
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.OriginalTitle).HasMaxLength(200);
        builder.Property(x => x.Description);
        builder.Property(x => x.ReleaseDate).HasColumnType("date");
        builder.Property(x => x.Subtitle).HasMaxLength(50);
        builder.Property(x => x.PosterUrl).HasMaxLength(255);
        builder.Property(x => x.TrailerUrl).HasMaxLength(255);
        builder
            .Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(Domain.Enums.MovieStatus.ComingSoon);
        builder.Property(x => x.AverageRating).HasColumnType("decimal(3,2)").HasDefaultValue(0m);
        builder
            .Property(x => x.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("NOW()");

        builder
            .HasOne(x => x.Director)
            .WithMany(d => d.Movies)
            .HasForeignKey(x => x.DirectorId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.DirectorId).HasDatabaseName("IX_Movies_Director");

        builder
            .HasOne(x => x.Language)
            .WithMany(l => l.Movies)
            .HasForeignKey(x => x.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.LanguageId).HasDatabaseName("IX_Movies_Language");

        builder
            .HasOne(x => x.AgeRating)
            .WithMany(a => a.Movies)
            .HasForeignKey(x => x.AgeRatingId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.AgeRatingId).HasDatabaseName("IX_Movies_AgeRating");

        builder.HasIndex(x => x.Status).HasDatabaseName("IX_Movies_Status");
        builder.HasIndex(x => x.ReleaseDate).HasDatabaseName("IX_Movies_ReleaseDate");
    }
}
