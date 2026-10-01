using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema.Infrastructure.Persistence.Configurations;

public class BookingTicketConfiguration : IEntityTypeConfiguration<BookingTicket>
{
    public void Configure(EntityTypeBuilder<BookingTicket> builder)
    {
        builder.ToTable(
            "BookingTickets",
            t => t.HasCheckConstraint("CK_BookingTickets_Price", "\"Price\" >= 0")
        );
        builder.HasKey(x => x.TicketId);
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
        builder.Property(x => x.QrCode).HasMaxLength(255);
        builder.Property(x => x.IsCheckedIn).HasDefaultValue(false);
        builder.Property(x => x.CheckedInAt).HasColumnType("timestamp without time zone");

        builder
            .HasOne(x => x.Booking)
            .WithMany(bk => bk.Tickets)
            .HasForeignKey(x => x.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.ShowtimeSeat)
            .WithOne(ss => ss.BookingTicket)
            .HasForeignKey<BookingTicket>(x => x.ShowtimeSeatId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.ShowtimeSeatId).IsUnique();
    }
}
