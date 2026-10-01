using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema.Infrastructure.Persistence.Configurations;

public class ShowtimeConfiguration : IEntityTypeConfiguration<Showtime>
{
    public void Configure(EntityTypeBuilder<Showtime> builder)
    {
        builder.ToTable(
            "Showtimes",
            t =>
            {
                t.HasCheckConstraint("CK_Showtime_Time", "\"EndTime\" > \"StartTime\"");
                t.HasCheckConstraint("CK_Showtimes_BasePrice", "\"BasePrice\" >= 0");
                t.HasCheckConstraint(
                    "CK_Showtimes_Status",
                    "\"Status\" IN ('Scheduled','Cancelled','Completed')"
                );
            }
        );
        builder.HasKey(x => x.ShowtimeId);
        builder.Property(x => x.StartTime).HasColumnType("timestamp without time zone");
        builder.Property(x => x.EndTime).HasColumnType("timestamp without time zone");
        builder.Property(x => x.BasePrice).HasColumnType("decimal(18,2)");
        builder
            .Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(Domain.Enums.ShowtimeStatus.Scheduled);
        builder
            .Property(x => x.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("NOW()");

        builder
            .HasOne(x => x.Movie)
            .WithMany(m => m.Showtimes)
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.Room)
            .WithMany(r => r.Showtimes)
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => new
            {
                x.RoomId,
                x.StartTime,
                x.EndTime,
            })
            .HasDatabaseName("IX_Showtime_Room_Time");
        builder.HasIndex(x => new { x.MovieId, x.StartTime }).HasDatabaseName("IX_Showtime_Movie");
    }
}
