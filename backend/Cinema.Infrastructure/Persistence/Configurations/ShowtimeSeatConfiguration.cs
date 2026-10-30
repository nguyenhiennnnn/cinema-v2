using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema.Infrastructure.Persistence.Configurations;

public class ShowtimeSeatConfiguration : IEntityTypeConfiguration<ShowtimeSeat>
{
    public void Configure(EntityTypeBuilder<ShowtimeSeat> builder)
    {
        builder.ToTable(
            "ShowtimeSeats",
            t =>
            {
                t.HasCheckConstraint("CK_ShowtimeSeats_Price", "\"Price\" >= 0");
                t.HasCheckConstraint(
                    "CK_ShowtimeSeats_Status",
                    "\"Status\" IN ('Available','Held','Booked')"
                );
            }
        );
        builder.HasKey(x => x.ShowtimeSeatId);
        builder
            .Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(Domain.Enums.ShowtimeSeatStatus.Available);
        builder.Property(x => x.HeldUntil).HasColumnType("timestamp without time zone");
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");

        builder
            .HasOne(x => x.Showtime)
            .WithMany(s => s.ShowtimeSeats)
            .HasForeignKey(x => x.ShowtimeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Seat)
            .WithMany(s => s.ShowtimeSeats)
            .HasForeignKey(x => x.SeatId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.HeldByUser)
            .WithMany()
            .HasForeignKey(x => x.HeldByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne<Booking>()
            .WithMany()
            .HasForeignKey(x => x.HeldByBookingId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasIndex(x => new { x.ShowtimeId, x.SeatId })
            .IsUnique()
            .HasDatabaseName("UQ_Showtime_Seat");
        builder
            .HasIndex(x => new { x.ShowtimeId, x.Status })
            .HasDatabaseName("IX_ShowtimeSeats_Status");
    }
}
