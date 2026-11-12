using Cinema.Application.BackgroundJobs;
using Cinema.Application.DTOs.Bookings;
using Cinema.Application.Interfaces;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.UnitTests.TestSupport;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class BookingServiceLoyaltyTests(PostgresContainerFixture fixture) : BookingServiceTestBase(fixture)
{
    private async Task<(int BronzeId, int SilverId)> ReplaceTiersAsync()
    {
        Db.MembershipTiers.RemoveRange(await Db.MembershipTiers.ToListAsync());
        await Db.SaveChangesAsync();
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
        Db.MembershipTiers.AddRange(bronze, silver);
        await Db.SaveChangesAsync();
        return (bronze.TierId, silver.TierId);
    }

    [Fact]
    public async Task ReverseLoyaltyAsync_PointsDropBelowTierThreshold_DowngradesTier()
    {
        TestDataSeeder.SetCinemaSettings(Db, pointsPerAmount: 10_000m);
        var (bronzeId, silverId) = await ReplaceTiersAsync();
        var customer = TestDataSeeder.AddCustomer(Db);
        var profile = await Db.CustomerProfiles.SingleAsync(p => p.UserId == customer.UserId);
        profile.Points = 150;
        profile.TotalSpent = 1_500_000m;
        profile.TierId = silverId;
        var booking = await LoyaltyTestHelper.AddConfirmedBookingAsync(
            Db,
            customer.UserId,
            1_500_000m
        );
        LoyaltyTestHelper.AddEntry(Db, customer.UserId, booking.BookingId, 150, DateTime.UtcNow);
        await Db.SaveChangesAsync();
        var sut = CreateFulfillmentSut();

        await sut.ReverseLoyaltyAsync(
            customer.UserId,
            booking.BookingId,
            600_000m,
            CancellationToken.None
        );
        await Db.SaveChangesAsync();

        var reloaded = await Db
            .CustomerProfiles.AsNoTracking()
            .SingleAsync(p => p.UserId == customer.UserId);
        reloaded.Points.ShouldBe(90);
        reloaded.TierId.ShouldBe(bronzeId);
    }

    [Fact]
    public async Task ReverseLoyaltyAsync_PointsStillMeetTier_KeepsTier()
    {
        TestDataSeeder.SetCinemaSettings(Db, pointsPerAmount: 10_000m);
        var (_, silverId) = await ReplaceTiersAsync();
        var customer = TestDataSeeder.AddCustomer(Db);
        var profile = await Db.CustomerProfiles.SingleAsync(p => p.UserId == customer.UserId);
        profile.Points = 150;
        profile.TierId = silverId;
        var booking = await LoyaltyTestHelper.AddConfirmedBookingAsync(
            Db,
            customer.UserId,
            1_500_000m
        );
        LoyaltyTestHelper.AddEntry(Db, customer.UserId, booking.BookingId, 150, DateTime.UtcNow);
        await Db.SaveChangesAsync();
        var sut = CreateFulfillmentSut();

        await sut.ReverseLoyaltyAsync(
            customer.UserId,
            booking.BookingId,
            100_000m,
            CancellationToken.None
        );
        await Db.SaveChangesAsync();

        var reloaded = await Db
            .CustomerProfiles.AsNoTracking()
            .SingleAsync(p => p.UserId == customer.UserId);
        reloaded.Points.ShouldBe(140);
        reloaded.TierId.ShouldBe(silverId);
    }

    [Fact]
    public async Task CancelBookingAsync_PartialRefund_RevokesLoyaltyProportionallyToRefund()
    {
        TestDataSeeder.SetCinemaSettings(
            Db,
            cancelRefundPercent: 50m,
            cancelDeadlineHours: 2,
            pointsPerAmount: 10_000m
        );
        var customer = TestDataSeeder.AddCustomer(Db);
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(
            Db,
            movie,
            room,
            seats,
            basePrice: 200_000m,
            startTime: DateTime.UtcNow.AddDays(2)
        );
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
        var booking = await Db
            .Bookings.AsNoTracking()
            .SingleAsync(b => b.BookingId == hold.BookingId);
        Db.Payments.Add(
            new Payment
            {
                BookingId = booking.BookingId,
                PaymentMethod = PaymentMethod.VNPay,
                Status = PaymentStatus.Success,
                Amount = booking.FinalAmount,
                TransactionId = "TXN-PARTIAL",
                PaidAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
            }
        );
        await Db.SaveChangesAsync();
        var pointsAfterConfirm = (
            await Db.CustomerProfiles.AsNoTracking().SingleAsync(p => p.UserId == customer.UserId)
        ).Points;
        var gatewayMock = AddPaymentGateway("VNPay");
        gatewayMock
            .Setup(g => g.RefundAsync(It.IsAny<RefundRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RefundResult(true, "REFUND-P", null));

        await CreateCancellationSut()
            .CancelBookingAsync(customer.UserId, booking.BookingId, CancellationToken.None);

        var refunded = booking.FinalAmount / 2m;
        var revokedPoints = (int)Math.Floor(refunded / 10_000m);
        var profile = await Db
            .CustomerProfiles.AsNoTracking()
            .SingleAsync(p => p.UserId == customer.UserId);
        profile.Points.ShouldBe(pointsAfterConfirm - revokedPoints);
        profile.Points.ShouldBeGreaterThan(0);
        profile.TotalSpent.ShouldBe(booking.FinalAmount - refunded);
    }
}

public class BookingServiceConfirmEmailTests(PostgresContainerFixture fixture) : BookingServiceTestBase(fixture)
{
    [Fact]
    public async Task ConfirmBookingAndGenerateTicketsAsync_UserBooking_EmailContainsMovieTitle()
    {
        TestDataSeeder.SetCinemaSettings(Db);
        var customer = TestDataSeeder.AddCustomer(Db);
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db, title: "Phim Kiem Tra Email");
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats);
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

        BackgroundJobClientMock.Verify(
            c =>
                c.Create(
                    It.Is<Job>(j =>
                        j.Type == typeof(SendEmailJob)
                        && ((string)j.Args[2]).Contains("Phim Kiem Tra Email")
                    ),
                    It.IsAny<IState>()
                ),
            Times.Once
        );
    }
}
