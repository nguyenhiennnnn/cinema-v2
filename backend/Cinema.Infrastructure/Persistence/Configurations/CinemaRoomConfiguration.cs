using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema.Infrastructure.Persistence.Configurations;

public class CinemaRoomConfiguration : IEntityTypeConfiguration<CinemaRoom>
{
    public void Configure(EntityTypeBuilder<CinemaRoom> builder)
    {
        builder.ToTable(
            "CinemaRooms",
            t => t.HasCheckConstraint("CK_CinemaRooms_TotalSeats", "\"TotalSeats\" >= 0")
        );
        builder.HasKey(x => x.RoomId);
        builder.Property(x => x.RoomName).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.RoomName).IsUnique();
        builder.Property(x => x.IsActive).HasDefaultValue(true);
        builder.Property(x => x.TotalSeats).HasDefaultValue(0);

        builder
            .HasOne(x => x.ScreenType)
            .WithMany(s => s.Rooms)
            .HasForeignKey(x => x.ScreenTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
