using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema.Infrastructure.Persistence.Configurations;

public class CinemaSettingsConfiguration : IEntityTypeConfiguration<CinemaSettings>
{
    public void Configure(EntityTypeBuilder<CinemaSettings> builder)
    {
        builder.ToTable(
            "CinemaSettings",
            t => t.HasCheckConstraint("CK_CinemaSettings_Id", "\"Id\" = 1")
        );
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Address).HasMaxLength(255).IsRequired();
        builder.Property(x => x.Phone).HasMaxLength(20);
        builder.Property(x => x.Email).HasMaxLength(150);
        builder.Property(x => x.TaxCode).HasMaxLength(50);
        builder.Property(x => x.LogoUrl).HasMaxLength(255);
        builder
            .Property(x => x.DefaultEmployeePassword)
            .HasMaxLength(100)
            .IsRequired()
            .HasDefaultValue(CinemaSettings.DefaultEmployeePasswordValue);
        builder.Property(x => x.OpeningTime).HasColumnType("time");
        builder.Property(x => x.ClosingTime).HasColumnType("time");
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamp without time zone");
        builder
            .Property(x => x.PointsPerAmount)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(10000m);
        builder
            .Property(x => x.CancelRefundPercent)
            .HasColumnType("decimal(5,2)")
            .HasDefaultValue(100m);
        builder
            .Property(x => x.SeatGapRuleMode)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(Domain.Enums.SeatGapRuleMode.Strict);

        builder
            .Property(x => x.RoomBufferMinutes)
            .HasDefaultValue(CinemaSettings.DefaultRoomBufferMinutes);
        builder
            .Property(x => x.MaxSeatsPerBooking)
            .HasDefaultValue(CinemaSettings.DefaultMaxSeatsPerBooking);
        builder
            .Property(x => x.MaxComboQuantity)
            .HasDefaultValue(CinemaSettings.DefaultMaxComboQuantity);
        builder
            .Property(x => x.LoyaltyWindowMonths)
            .HasDefaultValue(CinemaSettings.DefaultLoyaltyWindowMonths);
        builder
            .Property(x => x.MovieEndGraceDays)
            .HasDefaultValue(CinemaSettings.DefaultMovieEndGraceDays);
        builder
            .Property(x => x.ShowtimeLookaheadDays)
            .HasDefaultValue(CinemaSettings.DefaultShowtimeLookaheadDays);
        builder
            .Property(x => x.SchedulerStepMinutes)
            .HasDefaultValue(CinemaSettings.DefaultSchedulerStepMinutes);
        builder
            .Property(x => x.SchedulerMinSameMovieGapMinutes)
            .HasDefaultValue(CinemaSettings.DefaultSchedulerMinSameMovieGapMinutes);
        builder
            .Property(x => x.SchedulerHeatLookbackDays)
            .HasDefaultValue(CinemaSettings.DefaultSchedulerHeatLookbackDays);
        builder
            .Property(x => x.SchedulerHeatSalesWeight)
            .HasDefaultValue(CinemaSettings.DefaultSchedulerHeatSalesWeight);
        builder
            .Property(x => x.SchedulerHeatRatingWeight)
            .HasDefaultValue(CinemaSettings.DefaultSchedulerHeatRatingWeight);
        builder
            .Property(x => x.SchedulerMaxRating)
            .HasDefaultValue(CinemaSettings.DefaultSchedulerMaxRating);
    }
}
