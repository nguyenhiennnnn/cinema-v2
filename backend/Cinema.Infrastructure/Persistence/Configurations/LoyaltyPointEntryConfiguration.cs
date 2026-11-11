using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema.Infrastructure.Persistence.Configurations;

public class LoyaltyPointEntryConfiguration : IEntityTypeConfiguration<LoyaltyPointEntry>
{
    public void Configure(EntityTypeBuilder<LoyaltyPointEntry> builder)
    {
        builder.ToTable(
            "LoyaltyPointEntries",
            t =>
            {
                t.HasCheckConstraint("CK_LoyaltyPointEntries_Points", "\"Points\" >= 0");
                t.HasCheckConstraint(
                    "CK_LoyaltyPointEntries_Revoked",
                    "\"RevokedPoints\" >= 0 AND \"RevokedPoints\" <= \"Points\""
                );
            }
        );
        builder.HasKey(x => x.EntryId);
        builder.Property(x => x.RevokedPoints).HasDefaultValue(0);
        builder.Property(x => x.EarnedAt).HasColumnType("timestamp without time zone");

        builder
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasOne(x => x.Booking)
            .WithMany()
            .HasForeignKey(x => x.BookingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => x.BookingId)
            .IsUnique()
            .HasFilter("\"BookingId\" IS NOT NULL")
            .HasDatabaseName("UX_LoyaltyPointEntries_Booking");
        builder
            .HasIndex(x => new { x.UserId, x.EarnedAt })
            .HasDatabaseName("IX_LoyaltyPointEntries_User_EarnedAt");
    }
}
