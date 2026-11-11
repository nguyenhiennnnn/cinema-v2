using Cinema.Application.DTOs.Bookings;
using Cinema.Application.Services.Bookings;
using Cinema.Domain.Entities;
using Cinema.UnitTests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class LoyaltyDiscountTests : BookingServiceTestBase
{
    private async Task<HoldBookingResponse> HoldAsync(
        int userId,
        decimal seatPrice = 100_000m,
        List<ComboSelectionRequest>? combos = null
    )
    {
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats, basePrice: seatPrice);
        var seatId = (
            await Db.ShowtimeSeats.FirstAsync(ss => ss.ShowtimeId == showtime.ShowtimeId)
        ).ShowtimeSeatId;
        return await CreateHoldSut()
            .HoldSeatsAsync(
                userId,
                new HoldSeatsRequest(showtime.ShowtimeId, "Nguyen Van A", null, [seatId], combos),
                CancellationToken.None
            );
    }

    [Fact]
    public async Task HoldSeatsAsync_StaleTierInProfileCache_IsIgnoredWhenLedgerHasNoPoints()
    {
        var (_, _, goldId) = await LoyaltyTestHelper.ReplaceTiersAsync(Db);
        var customer = TestDataSeeder.AddCustomer(Db);
        var profile = await Db.CustomerProfiles.SingleAsync(p => p.UserId == customer.UserId);
        profile.Points = 500;
        profile.TierId = goldId; // cache cũ, sổ điểm không còn điểm nào
        await Db.SaveChangesAsync();

        var response = await HoldAsync(customer.UserId);

        response.DiscountAmount.ShouldBe(0m);
        response.FinalAmount.ShouldBe(100_000m);
    }

    [Fact]
    public async Task HoldSeatsAsync_LedgerPointsInWindow_AppliesTierEvenWhenCacheIsEmpty()
    {
        await LoyaltyTestHelper.ReplaceTiersAsync(Db);
        var customer = TestDataSeeder.AddCustomer(Db);
        LoyaltyTestHelper.AddEntry(Db, customer.UserId, null, 100, DateTime.UtcNow.AddMonths(-2));
        await Db.SaveChangesAsync();

        var response = await HoldAsync(customer.UserId);

        response.DiscountAmount.ShouldBe(5_000m); // Silver 5%
    }

    [Fact]
    public async Task HoldSeatsAsync_PointsOlderThan12Months_DoNotCount()
    {
        await LoyaltyTestHelper.ReplaceTiersAsync(Db);
        var customer = TestDataSeeder.AddCustomer(Db);
        LoyaltyTestHelper.AddEntry(
            Db,
            customer.UserId,
            null,
            500,
            DateTime.UtcNow.AddMonths(-12).AddDays(-1)
        );
        await Db.SaveChangesAsync();

        var response = await HoldAsync(customer.UserId);

        response.DiscountAmount.ShouldBe(0m);
    }

    [Fact]
    public async Task HoldSeatsAsync_PointsJustInsideWindow_StillCount()
    {
        await LoyaltyTestHelper.ReplaceTiersAsync(Db);
        var customer = TestDataSeeder.AddCustomer(Db);
        LoyaltyTestHelper.AddEntry(
            Db,
            customer.UserId,
            null,
            200,
            DateTime.UtcNow.AddMonths(-12).AddDays(1)
        );
        await Db.SaveChangesAsync();

        var response = await HoldAsync(customer.UserId);

        response.DiscountAmount.ShouldBe(10_000m); // Gold 10%
    }

    [Fact]
    public async Task HoldSeatsAsync_RevokedPointsReduceTier()
    {
        await LoyaltyTestHelper.ReplaceTiersAsync(Db);
        var customer = TestDataSeeder.AddCustomer(Db);
        var entry = LoyaltyTestHelper.AddEntry(Db, customer.UserId, null, 200, DateTime.UtcNow);
        entry.RevokedPoints = 150; // còn 50 điểm → chưa đủ Silver
        await Db.SaveChangesAsync();

        var response = await HoldAsync(customer.UserId);

        response.DiscountAmount.ShouldBe(0m);
    }

    [Fact]
    public async Task HoldSeatsAsync_DiscountAppliesToTicketsOnly_NotCombo()
    {
        await LoyaltyTestHelper.ReplaceTiersAsync(Db);
        var customer = TestDataSeeder.AddCustomer(Db);
        LoyaltyTestHelper.AddEntry(Db, customer.UserId, null, 200, DateTime.UtcNow);
        var combo = new Combo
        {
            Name = "Bap nuoc",
            Price = 50_000m,
            IsActive = true,
        };
        Db.Combos.Add(combo);
        await Db.SaveChangesAsync();

        var response = await HoldAsync(
            customer.UserId,
            seatPrice: 100_000m,
            combos: [new ComboSelectionRequest(combo.ComboId, 2)]
        );

        response.TotalAmount.ShouldBe(200_000m); // 100k vé + 2 × 50k combo
        response.DiscountAmount.ShouldBe(10_000m); // 10% × 100k vé, không tính combo
        response.FinalAmount.ShouldBe(190_000m);
    }

    [Fact]
    public async Task HoldSeatsAsync_ComboOnlyDiscountBase_RoundsAwayFromZeroOnTicketAmount()
    {
        await LoyaltyTestHelper.ReplaceTiersAsync(Db);
        var customer = TestDataSeeder.AddCustomer(Db);
        LoyaltyTestHelper.AddEntry(Db, customer.UserId, null, 100, DateTime.UtcNow); // Silver 5%
        await Db.SaveChangesAsync();

        var response = await HoldAsync(customer.UserId, seatPrice: 90_010m);

        response.DiscountAmount.ShouldBe(4_501m); // 4500.5 → 4501
    }

    [Fact]
    public async Task HoldSeatsAsync_CustomerWithoutLedgerAndTierZeroPercent_NoDiscount()
    {
        await LoyaltyTestHelper.ReplaceTiersAsync(Db);
        var customer = TestDataSeeder.AddCustomer(Db);

        var response = await HoldAsync(customer.UserId);

        response.DiscountAmount.ShouldBe(0m);
    }
}

public class LoyaltyLedgerTests : BookingServiceTestBase
{
    [Fact]
    public async Task ConfirmBookingAsync_CreatesLedgerEntryForBooking()
    {
        TestDataSeeder.SetCinemaSettings(Db, pointsPerAmount: 10_000m);
        var customer = TestDataSeeder.AddCustomer(Db);
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats, basePrice: 100_000m);
        var seatId = (
            await Db.ShowtimeSeats.FirstAsync(ss => ss.ShowtimeId == showtime.ShowtimeId)
        ).ShowtimeSeatId;
        var hold = await CreateHoldSut()
            .HoldSeatsAsync(
                customer.UserId,
                new HoldSeatsRequest(showtime.ShowtimeId, "Nguyen Van A", null, [seatId], null),
                CancellationToken.None
            );

        await CreateFulfillmentSut()
            .ConfirmBookingAndGenerateTicketsAsync(hold.BookingId, CancellationToken.None);

        var entry = await Db.LoyaltyPointEntries.AsNoTracking().SingleAsync();
        entry.UserId.ShouldBe(customer.UserId);
        entry.BookingId.ShouldBe(hold.BookingId);
        entry.Points.ShouldBe(10);
        entry.RevokedPoints.ShouldBe(0);
        entry.EarnedAt.ShouldBeInRange(
            DateTime.UtcNow.AddMinutes(-1),
            DateTime.UtcNow.AddMinutes(1)
        );
        var profile = await Db.CustomerProfiles.AsNoTracking().SingleAsync();
        profile.Points.ShouldBe(10);
    }

    [Fact]
    public async Task ConfirmBookingAsync_StaleExpiredPointsInCache_AreDroppedAndTierDowngraded()
    {
        TestDataSeeder.SetCinemaSettings(Db, pointsPerAmount: 10_000m);
        var (bronzeId, silverId, _) = await LoyaltyTestHelper.ReplaceTiersAsync(Db);
        var customer = TestDataSeeder.AddCustomer(Db);
        var old = await LoyaltyTestHelper.AddConfirmedBookingAsync(Db, customer.UserId);
        LoyaltyTestHelper.AddEntry(
            Db,
            customer.UserId,
            old.BookingId,
            100,
            DateTime.UtcNow.AddMonths(-14)
        );
        var profile = await Db.CustomerProfiles.SingleAsync(p => p.UserId == customer.UserId);
        profile.Points = 100; // job chưa kịp chạy
        profile.TierId = silverId;
        await Db.SaveChangesAsync();

        var room = TestDataSeeder.AddRoom(Db, name: "R-new");
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats, basePrice: 100_000m);
        var seatId = (
            await Db.ShowtimeSeats.FirstAsync(ss => ss.ShowtimeId == showtime.ShowtimeId)
        ).ShowtimeSeatId;
        var hold = await CreateHoldSut()
            .HoldSeatsAsync(
                customer.UserId,
                new HoldSeatsRequest(showtime.ShowtimeId, "Nguyen Van A", null, [seatId], null),
                CancellationToken.None
            );
        hold.DiscountAmount.ShouldBe(0m); // điểm cũ đã hết hạn nên không còn giảm giá

        await CreateFulfillmentSut()
            .ConfirmBookingAndGenerateTicketsAsync(hold.BookingId, CancellationToken.None);

        var reloaded = await Db
            .CustomerProfiles.AsNoTracking()
            .SingleAsync(p => p.UserId == customer.UserId);
        reloaded.Points.ShouldBe(10);
        reloaded.TierId.ShouldBe(bronzeId);
    }

    [Fact]
    public async Task ReverseLoyaltyAsync_EntryAlreadyExpired_DoesNotReduceCurrentWindowPoints()
    {
        TestDataSeeder.SetCinemaSettings(Db, pointsPerAmount: 10_000m);
        await LoyaltyTestHelper.ReplaceTiersAsync(Db);
        var customer = TestDataSeeder.AddCustomer(Db);
        var oldBooking = await LoyaltyTestHelper.AddConfirmedBookingAsync(
            Db,
            customer.UserId,
            500_000m
        );
        var freshBooking = await LoyaltyTestHelper.AddConfirmedBookingAsync(
            Db,
            customer.UserId,
            300_000m
        );
        var oldEntry = LoyaltyTestHelper.AddEntry(
            Db,
            customer.UserId,
            oldBooking.BookingId,
            50,
            DateTime.UtcNow.AddMonths(-13)
        );
        LoyaltyTestHelper.AddEntry(
            Db,
            customer.UserId,
            freshBooking.BookingId,
            30,
            DateTime.UtcNow
        );
        var profile = await Db.CustomerProfiles.SingleAsync(p => p.UserId == customer.UserId);
        profile.Points = 30;
        profile.TotalSpent = 800_000m;
        await Db.SaveChangesAsync();

        await CreateFulfillmentSut()
            .ReverseLoyaltyAsync(
                customer.UserId,
                oldBooking.BookingId,
                500_000m,
                CancellationToken.None
            );
        await Db.SaveChangesAsync();

        (await Db.CustomerProfiles.AsNoTracking().SingleAsync()).Points.ShouldBe(30);
        (
            await Db
                .LoyaltyPointEntries.AsNoTracking()
                .SingleAsync(e => e.EntryId == oldEntry.EntryId)
        ).RevokedPoints.ShouldBe(50);
    }

    [Fact]
    public async Task ReverseLoyaltyAsync_PartialRefund_OnlyTouchesOwnEntry()
    {
        TestDataSeeder.SetCinemaSettings(Db, pointsPerAmount: 10_000m);
        await LoyaltyTestHelper.ReplaceTiersAsync(Db);
        var customer = TestDataSeeder.AddCustomer(Db);
        var a = await LoyaltyTestHelper.AddConfirmedBookingAsync(Db, customer.UserId, 500_000m);
        var b = await LoyaltyTestHelper.AddConfirmedBookingAsync(Db, customer.UserId, 300_000m);
        var entryA = LoyaltyTestHelper.AddEntry(
            Db,
            customer.UserId,
            a.BookingId,
            50,
            DateTime.UtcNow
        );
        var entryB = LoyaltyTestHelper.AddEntry(
            Db,
            customer.UserId,
            b.BookingId,
            30,
            DateTime.UtcNow
        );
        await Db.SaveChangesAsync();

        await CreateFulfillmentSut()
            .ReverseLoyaltyAsync(customer.UserId, a.BookingId, 200_000m, CancellationToken.None);
        await Db.SaveChangesAsync();

        (
            await Db
                .LoyaltyPointEntries.AsNoTracking()
                .SingleAsync(e => e.EntryId == entryA.EntryId)
        ).RevokedPoints.ShouldBe(20);
        (
            await Db
                .LoyaltyPointEntries.AsNoTracking()
                .SingleAsync(e => e.EntryId == entryB.EntryId)
        ).RevokedPoints.ShouldBe(0);
        (await Db.CustomerProfiles.AsNoTracking().SingleAsync()).Points.ShouldBe(60);
    }

    [Fact]
    public async Task ReverseLoyaltyAsync_RevokeCappedAtEntryPoints()
    {
        TestDataSeeder.SetCinemaSettings(Db, pointsPerAmount: 10_000m);
        await LoyaltyTestHelper.ReplaceTiersAsync(Db);
        var customer = TestDataSeeder.AddCustomer(Db);
        var a = await LoyaltyTestHelper.AddConfirmedBookingAsync(Db, customer.UserId, 50_000m);
        var b = await LoyaltyTestHelper.AddConfirmedBookingAsync(Db, customer.UserId, 300_000m);
        var entryA = LoyaltyTestHelper.AddEntry(
            Db,
            customer.UserId,
            a.BookingId,
            5,
            DateTime.UtcNow
        );
        LoyaltyTestHelper.AddEntry(Db, customer.UserId, b.BookingId, 30, DateTime.UtcNow);
        await Db.SaveChangesAsync();

        await CreateFulfillmentSut()
            .ReverseLoyaltyAsync(customer.UserId, a.BookingId, 1_000_000m, CancellationToken.None);
        await Db.SaveChangesAsync();

        (
            await Db
                .LoyaltyPointEntries.AsNoTracking()
                .SingleAsync(e => e.EntryId == entryA.EntryId)
        ).RevokedPoints.ShouldBe(5);
        (await Db.CustomerProfiles.AsNoTracking().SingleAsync()).Points.ShouldBe(30);
    }

    [Fact]
    public async Task ReverseLoyaltyAsync_BookingWithoutEntry_StillReducesTotalSpentAndDoesNotThrow()
    {
        TestDataSeeder.SetCinemaSettings(Db, pointsPerAmount: 10_000m);
        var customer = TestDataSeeder.AddCustomer(Db);
        var profile = await Db.CustomerProfiles.SingleAsync(p => p.UserId == customer.UserId);
        profile.TotalSpent = 300_000m;
        await Db.SaveChangesAsync();

        await Should.NotThrowAsync(async () =>
        {
            await CreateFulfillmentSut()
                .ReverseLoyaltyAsync(customer.UserId, 9999, 100_000m, CancellationToken.None);
            await Db.SaveChangesAsync();
        });

        (await Db.CustomerProfiles.AsNoTracking().SingleAsync()).TotalSpent.ShouldBe(200_000m);
    }
}

public class LoyaltyExpiryServiceTests : BookingServiceTestBase
{
    private LoyaltyExpiryService CreateService() =>
        new(Db, NullLogger<LoyaltyExpiryService>.Instance);

    [Fact]
    public async Task RecalculateAllAsync_ExpiredPointsRemoved_AndTierDowngraded()
    {
        var (bronzeId, silverId, _) = await LoyaltyTestHelper.ReplaceTiersAsync(Db);
        var customer = TestDataSeeder.AddCustomer(Db);
        LoyaltyTestHelper.AddEntry(Db, customer.UserId, null, 150, DateTime.UtcNow.AddMonths(-13));
        var profile = await Db.CustomerProfiles.SingleAsync(p => p.UserId == customer.UserId);
        profile.Points = 150;
        profile.TierId = silverId;
        await Db.SaveChangesAsync();

        var changed = await CreateService().RecalculateAllAsync(CancellationToken.None);

        changed.ShouldBe(1);
        var reloaded = await Db.CustomerProfiles.AsNoTracking().SingleAsync();
        reloaded.Points.ShouldBe(0);
        reloaded.TierId.ShouldBe(bronzeId);
    }

    [Fact]
    public async Task RecalculateAllAsync_MixedEntries_KeepsOnlyFreshPoints()
    {
        var (_, silverId, _) = await LoyaltyTestHelper.ReplaceTiersAsync(Db);
        var customer = TestDataSeeder.AddCustomer(Db);
        LoyaltyTestHelper.AddEntry(Db, customer.UserId, null, 100, DateTime.UtcNow.AddMonths(-13));
        LoyaltyTestHelper.AddEntry(Db, customer.UserId, null, 150, DateTime.UtcNow.AddMonths(-1));
        var profile = await Db.CustomerProfiles.SingleAsync(p => p.UserId == customer.UserId);
        profile.Points = 250;
        await Db.SaveChangesAsync();

        await CreateService().RecalculateAllAsync(CancellationToken.None);

        var reloaded = await Db.CustomerProfiles.AsNoTracking().SingleAsync();
        reloaded.Points.ShouldBe(150);
        reloaded.TierId.ShouldBe(silverId);
    }

    [Fact]
    public async Task RecalculateAllAsync_SecondRunIsNoOp()
    {
        await LoyaltyTestHelper.ReplaceTiersAsync(Db);
        var customer = TestDataSeeder.AddCustomer(Db);
        LoyaltyTestHelper.AddEntry(Db, customer.UserId, null, 120, DateTime.UtcNow.AddDays(-3));
        var profile = await Db.CustomerProfiles.SingleAsync(p => p.UserId == customer.UserId);
        profile.Points = 999; // lệch cache
        await Db.SaveChangesAsync();
        var sut = CreateService();

        (await sut.RecalculateAllAsync(CancellationToken.None)).ShouldBe(1);
        (await sut.RecalculateAllAsync(CancellationToken.None)).ShouldBe(0);
    }

    [Fact]
    public async Task RecalculateAllAsync_RevokedPointsExcluded()
    {
        await LoyaltyTestHelper.ReplaceTiersAsync(Db);
        var customer = TestDataSeeder.AddCustomer(Db);
        var entry = LoyaltyTestHelper.AddEntry(Db, customer.UserId, null, 120, DateTime.UtcNow);
        entry.RevokedPoints = 70;
        var profile = await Db.CustomerProfiles.SingleAsync(p => p.UserId == customer.UserId);
        profile.Points = 120;
        await Db.SaveChangesAsync();

        await CreateService().RecalculateAllAsync(CancellationToken.None);

        (await Db.CustomerProfiles.AsNoTracking().SingleAsync()).Points.ShouldBe(50);
    }

    [Fact]
    public async Task RecalculateAllAsync_ProfileWithCachedPointsButNoLedger_IsZeroed()
    {
        // Ghi lại hành vi: khách chưa được backfill sổ điểm sẽ mất điểm → phải chạy script backfill trước khi bật job.
        await LoyaltyTestHelper.ReplaceTiersAsync(Db);
        var customer = TestDataSeeder.AddCustomer(Db);
        var profile = await Db.CustomerProfiles.SingleAsync(p => p.UserId == customer.UserId);
        profile.Points = 300;
        await Db.SaveChangesAsync();

        await CreateService().RecalculateAllAsync(CancellationToken.None);

        (await Db.CustomerProfiles.AsNoTracking().SingleAsync()).Points.ShouldBe(0);
    }
}
