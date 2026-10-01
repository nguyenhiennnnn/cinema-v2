using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema.Infrastructure.Persistence.Configurations;

public class BookingComboConfiguration : IEntityTypeConfiguration<BookingCombo>
{
    public void Configure(EntityTypeBuilder<BookingCombo> builder)
    {
        builder.ToTable(
            "BookingCombos",
            t =>
            {
                t.HasCheckConstraint("CK_BookingCombos_Quantity", "\"Quantity\" > 0");
                t.HasCheckConstraint("CK_BookingCombos_UnitPrice", "\"UnitPrice\" >= 0");
            }
        );
        builder.HasKey(x => x.BookingComboId);
        builder.Property(x => x.Quantity).HasDefaultValue(1);
        builder.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");

        builder
            .HasOne(x => x.Booking)
            .WithMany(bk => bk.BookingCombos)
            .HasForeignKey(x => x.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => x.BookingId).HasDatabaseName("IX_BookingCombos_Booking");

        builder
            .HasOne(x => x.Combo)
            .WithMany(c => c.BookingCombos)
            .HasForeignKey(x => x.ComboId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
