using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema.Infrastructure.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable(
            "Bookings",
            t =>
            {
                t.HasCheckConstraint("CK_Bookings_TotalAmount", "\"TotalAmount\" >= 0");
                t.HasCheckConstraint("CK_Bookings_DiscountAmount", "\"DiscountAmount\" >= 0");
                t.HasCheckConstraint("CK_Bookings_FinalAmount", "\"FinalAmount\" >= 0");
                t.HasCheckConstraint("CK_Bookings_Channel", "\"Channel\" IN ('Online','Counter')");
                t.HasCheckConstraint(
                    "CK_Bookings_Status",
                    "\"Status\" IN ('Pending','Confirmed','Cancelled','Expired')"
                );
                t.HasCheckConstraint(
                    "CK_Booking_Channel_Employee",
                    "(\"Channel\" = 'Counter' AND \"CreatedByEmployeeId\" IS NOT NULL) OR "
                        + "(\"Channel\" = 'Online' AND \"CreatedByEmployeeId\" IS NULL)"
                );
                t.HasCheckConstraint(
                    "CK_Booking_Customer_Identity",
                    "\"UserId\" IS NOT NULL OR \"CustomerName\" IS NOT NULL"
                );
            }
        );
        builder.HasKey(x => x.BookingId);
        builder.Property(x => x.BookingCode).HasMaxLength(20).IsRequired();
        builder.HasIndex(x => x.BookingCode).IsUnique();
        builder
            .Property(x => x.Channel)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(Domain.Enums.BookingChannel.Online);
        builder.Property(x => x.CustomerName).HasMaxLength(100);
        builder.Property(x => x.CustomerPhone).HasMaxLength(20);
        builder.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.DiscountAmount).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
        builder.Property(x => x.FinalAmount).HasColumnType("decimal(18,2)");
        builder
            .Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(Domain.Enums.BookingStatus.Pending);
        builder
            .Property(x => x.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("NOW()");
        builder.Property(x => x.ExpiresAt).HasColumnType("timestamp without time zone");

        builder
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasIndex(x => new { x.UserId, x.CreatedAt })
            .HasDatabaseName("IX_Bookings_User")
            .IsDescending(false, true);

        builder
            .HasOne(x => x.Showtime)
            .WithMany(s => s.Bookings)
            .HasForeignKey(x => x.ShowtimeId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.ShowtimeId).HasDatabaseName("IX_Bookings_Showtime");

        builder
            .HasOne(x => x.CreatedByEmployee)
            .WithMany(e => e.CounterBookings)
            .HasForeignKey(x => x.CreatedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.CreatedByEmployeeId).HasDatabaseName("IX_Bookings_Employee");

        builder.HasIndex(x => x.Status).HasDatabaseName("IX_Bookings_Status");
        builder.HasIndex(x => x.Channel).HasDatabaseName("IX_Bookings_Channel");
    }
}
