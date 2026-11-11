using Cinema.Application.DTOs.Bookings;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.UnitTests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class BookingFulfillmentServiceTests : BookingServiceTestBase
{
    private async Task<(Booking Booking, User Customer, Showtime Showtime)> SeedHeldBookingAsync(
        decimal basePrice = 100_000m,
        DateTime? startTime = null
    )
    {
        var customer = TestDataSeeder.AddCustomer(Db);
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(
            Db,
            movie,
            room,
            seats,
            basePrice: basePrice,
            startTime: startTime
        );
        var seatId = (
            await Db.ShowtimeSeats.FirstAsync(ss => ss.ShowtimeId == showtime.ShowtimeId)
        ).ShowtimeSeatId;

        var holdingSut = CreateHoldSut();
        var hold = await holdingSut.HoldSeatsAsync(
            customer.UserId,
            new HoldSeatsRequest(showtime.ShowtimeId, "Nguyen Van A", null, [seatId], null),
            CancellationToken.None
        );
        var booking = await Db.Bookings.SingleAsync(b => b.BookingId == hold.BookingId);
        return (booking, customer, showtime);
    }

    [Fact]
    public async Task ConfirmBookingAndGenerateTicketsAsync_HeldSeatsStillValid_BooksSeatsAndGeneratesTicketsAndLoyaltyPoints()
    {
        TestDataSeeder.SetCinemaSettings(Db, pointsPerAmount: 10_000m);
        var (booking, customer, _) = await SeedHeldBookingAsync(basePrice: 100_000m);
        var sut = CreateFulfillmentSut();

        await sut.ConfirmBookingAndGenerateTicketsAsync(booking.BookingId, CancellationToken.None);

        var confirmed = await Db.Bookings.SingleAsync(b => b.BookingId == booking.BookingId);
        confirmed.Status.ShouldBe(BookingStatus.Confirmed);

        var tickets = await Db
            .BookingTickets.Where(t => t.BookingId == booking.BookingId)
            .ToListAsync();
        tickets.Count.ShouldBe(1);
        tickets[0].QrCode.ShouldNotBeNullOrWhiteSpace();

        var bookedSeat = await Db.ShowtimeSeats.SingleAsync(ss =>
            ss.ShowtimeId == booking.ShowtimeId
        );
        bookedSeat.Status.ShouldBe(ShowtimeSeatStatus.Booked);

        var profile = await Db.CustomerProfiles.SingleAsync(p => p.UserId == customer.UserId);
        profile.Points.ShouldBe(10);
        profile.TotalSpent.ShouldBe(100_000m);
    }

    [Fact]
    public async Task ConfirmBookingAndGenerateTicketsAsync_AlreadyConfirmed_IsIdempotentAndDoesNotDuplicateTickets()
    {
        TestDataSeeder.SetCinemaSettings(Db);
        var (booking, _, _) = await SeedHeldBookingAsync();
        var sut = CreateFulfillmentSut();
        await sut.ConfirmBookingAndGenerateTicketsAsync(booking.BookingId, CancellationToken.None);

        await sut.ConfirmBookingAndGenerateTicketsAsync(booking.BookingId, CancellationToken.None);

        var tickets = await Db
            .BookingTickets.Where(t => t.BookingId == booking.BookingId)
            .ToListAsync();
        tickets.Count.ShouldBe(1);
    }

    [Fact]
    public async Task ConfirmBookingAndGenerateTicketsAsync_SeatsNoLongerHeld_CancelsBookingAndThrows()
    {
        TestDataSeeder.SetCinemaSettings(Db);
        var (booking, _, _) = await SeedHeldBookingAsync();
        var seat = await Db.ShowtimeSeats.SingleAsync(ss => ss.ShowtimeId == booking.ShowtimeId);
        seat.Status = ShowtimeSeatStatus.Available;
        seat.HeldByUserId = null;
        await Db.SaveChangesAsync();
        var sut = CreateFulfillmentSut();

        await Should.ThrowAsync<SeatsNoLongerHeldException>(() =>
            sut.ConfirmBookingAndGenerateTicketsAsync(booking.BookingId, CancellationToken.None)
        );

        var reloaded = await Db.Bookings.SingleAsync(b => b.BookingId == booking.BookingId);
        reloaded.Status.ShouldBe(BookingStatus.Cancelled);
    }

    [Fact]
    public async Task ProcessPaymentFailureAsync_PendingBooking_ReleasesSeatsAndCancelsBooking()
    {
        var (booking, _, _) = await SeedHeldBookingAsync();
        booking.ExpiresAt = DateTime.UtcNow.AddMinutes(-1);
        await Db.SaveChangesAsync();
        var sut = CreateFulfillmentSut();

        await sut.ProcessPaymentFailureAsync(booking.BookingId, CancellationToken.None);

        var reloaded = await Db.Bookings.SingleAsync(b => b.BookingId == booking.BookingId);
        reloaded.Status.ShouldBe(BookingStatus.Cancelled);
        var seat = await Db.ShowtimeSeats.SingleAsync(ss => ss.ShowtimeId == booking.ShowtimeId);
        seat.Status.ShouldBe(ShowtimeSeatStatus.Available);
    }

    [Fact]
    public async Task ProcessPaymentFailureAsync_HoldNotExpired_KeepsBookingPendingForRetry()
    {
        var (booking, _, _) = await SeedHeldBookingAsync();
        var sut = CreateFulfillmentSut();

        await sut.ProcessPaymentFailureAsync(booking.BookingId, CancellationToken.None);

        var reloaded = await Db.Bookings.SingleAsync(b => b.BookingId == booking.BookingId);
        reloaded.Status.ShouldBe(BookingStatus.Pending);
        var seat = await Db.ShowtimeSeats.SingleAsync(ss => ss.ShowtimeId == booking.ShowtimeId);
        seat.Status.ShouldBe(ShowtimeSeatStatus.Held);
    }

    [Fact]
    public async Task ProcessPaymentFailureAsync_BookingAlreadyConfirmed_DoesNothing()
    {
        TestDataSeeder.SetCinemaSettings(Db);
        var (booking, _, _) = await SeedHeldBookingAsync();
        var confirmSut = CreateFulfillmentSut();
        await confirmSut.ConfirmBookingAndGenerateTicketsAsync(
            booking.BookingId,
            CancellationToken.None
        );
        var sut = CreateFulfillmentSut();

        await sut.ProcessPaymentFailureAsync(booking.BookingId, CancellationToken.None);

        var reloaded = await Db.Bookings.SingleAsync(b => b.BookingId == booking.BookingId);
        reloaded.Status.ShouldBe(BookingStatus.Confirmed);
    }
}
