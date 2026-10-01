using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema.Infrastructure.Persistence.Configurations;

public class SeatConfiguration : IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> builder)
    {
        builder.ToTable(
            "Seats",
            t => t.HasCheckConstraint("CK_Seats_SeatNumber", "\"SeatNumber\" > 0")
        );
        builder.HasKey(x => x.SeatId);
        builder.Property(x => x.SeatRow).HasMaxLength(5).IsRequired();
        builder.Property(x => x.IsActive).HasDefaultValue(true);

        builder
            .HasOne(x => x.Room)
            .WithMany(r => r.Seats)
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.SeatType)
            .WithMany(st => st.Seats)
            .HasForeignKey(x => x.SeatTypeId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.SeatTypeId).HasDatabaseName("IX_Seats_SeatType");

        builder
            .HasIndex(x => new
            {
                x.RoomId,
                x.SeatRow,
                x.SeatNumber,
            })
            .IsUnique()
            .HasDatabaseName("UQ_Room_Seat");
    }
}
