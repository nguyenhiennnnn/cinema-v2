using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cinema.UnitTests.TestSupport;

public static class LoyaltyTestHelper
{
    public static async Task<(int BronzeId, int SilverId, int GoldId)> ReplaceTiersAsync(
        AppDbContext db
    )
    {
        db.MembershipTiers.RemoveRange(await db.MembershipTiers.ToListAsync());
        await db.SaveChangesAsync();
        var bronze = new MembershipTier
        {
            TierName = "Bronze",
            MinPoints = 0,
            DiscountPercent = 0m,
        };
        var silver = new MembershipTier
        {
            TierName = "Silver",
            MinPoints = 100,
            DiscountPercent = 5m,
        };
        var gold = new MembershipTier
        {
            TierName = "Gold",
            MinPoints = 200,
            DiscountPercent = 10m,
        };
        db.MembershipTiers.AddRange(bronze, silver, gold);
        await db.SaveChangesAsync();
        return (bronze.TierId, silver.TierId, gold.TierId);
    }

    public static async Task<Booking> AddConfirmedBookingAsync(
        AppDbContext db,
        int userId,
        decimal finalAmount = 100_000m
    )
    {
        var room = TestDataSeeder.AddRoom(db, name: $"R-{Guid.NewGuid():N}"[..12]);
        var seats = TestDataSeeder.AddSeats(db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(db);
        var showtime = TestDataSeeder.AddShowtime(db, movie, room, seats);
        var booking = new Booking
        {
            UserId = userId,
            ShowtimeId = showtime.ShowtimeId,
            BookingCode = $"T{Guid.NewGuid():N}"[..12],
            CustomerName = "Test Customer",
            TotalAmount = finalAmount,
            DiscountAmount = 0m,
            FinalAmount = finalAmount,
            Status = BookingStatus.Confirmed,
            CreatedAt = DateTime.UtcNow,
        };
        db.Bookings.Add(booking);
        await db.SaveChangesAsync();
        return booking;
    }

    public static LoyaltyPointEntry AddEntry(
        AppDbContext db,
        int userId,
        int? bookingId,
        int points,
        DateTime earnedAt
    )
    {
        var entry = new LoyaltyPointEntry
        {
            UserId = userId,
            BookingId = bookingId,
            Points = points,
            EarnedAt = earnedAt,
        };
        db.LoyaltyPointEntries.Add(entry);
        return entry;
    }
}
