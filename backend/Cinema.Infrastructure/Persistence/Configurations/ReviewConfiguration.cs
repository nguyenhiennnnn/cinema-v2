using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema.Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable(
            "Reviews",
            t => t.HasCheckConstraint("CK_Reviews_Rating", "\"Rating\" BETWEEN 1 AND 5")
        );
        builder.HasKey(x => x.ReviewId);
        builder.Property(x => x.Comment);
        builder
            .Property(x => x.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("NOW()");

        builder
            .HasOne(x => x.Movie)
            .WithMany(m => m.Reviews)
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => x.MovieId).HasDatabaseName("IX_Reviews_Movie");

        builder
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => new { x.UserId, x.MovieId })
            .IsUnique()
            .HasDatabaseName("UQ_User_Movie_Review");
    }
}
