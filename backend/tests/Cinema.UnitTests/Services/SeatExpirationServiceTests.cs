using Cinema.Application.Interfaces;
using Cinema.Application.Services.Bookings;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Infrastructure.Persistence;
using Cinema.UnitTests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class SeatExpirationServiceTests : IDisposable
{
    private readonly SqliteAppDbContextHandle _handle = new();
    private readonly Mock<IRealtimeNotifier> _realtimeNotifierMock = new();

    private AppDbContext Db => _handle.Db;

    private SeatExpirationService CreateSut()
    {
        Db.ChangeTracker.Clear();
        _realtimeNotifierMock
            .Setup(n =>
                n.NotifyHoldExpiredAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<List<int>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.CompletedTask);
        return new SeatExpirationService(
            Db,
            _realtimeNotifierMock.Object,
            new BookingSeatReleaser(Db, _realtimeNotifierMock.Object),
            NullLogger<SeatExpirationService>.Instance
        );
    }

    [Fact]
    public async Task ReleaseStaleHeldSeatsAsync_SeatsHeldPastDeadline_AreReleasedToAvailable()
    {
        var customer = TestDataSeeder.AddCustomer(Db);
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats);
        var showtimeSeat = await Db.ShowtimeSeats.SingleAsync(ss =>
            ss.ShowtimeId == showtime.ShowtimeId
        );
        showtimeSeat.Status = ShowtimeSeatStatus.Held;
        showtimeSeat.HeldByUserId = customer.UserId;
        showtimeSeat.HeldUntil = DateTime.UtcNow.AddMinutes(-1);
        await Db.SaveChangesAsync();
        var sut = CreateSut();

        var releasedCount = await sut.ReleaseStaleHeldSeatsAsync(null, CancellationToken.None);

        releasedCount.ShouldBe(1);
        var reloaded = await Db.ShowtimeSeats.SingleAsync(ss =>
            ss.ShowtimeSeatId == showtimeSeat.ShowtimeSeatId
        );
        reloaded.Status.ShouldBe(ShowtimeSeatStatus.Available);
        reloaded.HeldByUserId.ShouldBeNull();
    }

    [Fact]
    public async Task ReleaseStaleHeldSeatsAsync_SeatsStillWithinHoldWindow_AreNotReleased()
    {
        var customer = TestDataSeeder.AddCustomer(Db);
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats);
        var showtimeSeat = await Db.ShowtimeSeats.SingleAsync(ss =>
            ss.ShowtimeId == showtime.ShowtimeId
        );
        showtimeSeat.Status = ShowtimeSeatStatus.Held;
        showtimeSeat.HeldByUserId = customer.UserId;
        showtimeSeat.HeldUntil = DateTime.UtcNow.AddMinutes(10);
        await Db.SaveChangesAsync();
        var sut = CreateSut();

        var releasedCount = await sut.ReleaseStaleHeldSeatsAsync(null, CancellationToken.None);

        releasedCount.ShouldBe(0);
    }

    [Fact]
    public async Task ReleaseStaleHeldSeatsAsync_EmptyShowtimeIdFilter_ReleasesNothingWithoutQuerying()
    {
        var sut = CreateSut();

        var releasedCount = await sut.ReleaseStaleHeldSeatsAsync([], CancellationToken.None);

        releasedCount.ShouldBe(0);
    }

    [Fact]
    public async Task ExpireDueBookingsAsync_PendingBookingPastExpiresAt_IsExpiredAndSeatsReleased()
    {
        var customer = TestDataSeeder.AddCustomer(Db);
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats);
        var showtimeSeat = await Db.ShowtimeSeats.SingleAsync(ss =>
            ss.ShowtimeId == showtime.ShowtimeId
        );
        showtimeSeat.Status = ShowtimeSeatStatus.Held;
        showtimeSeat.HeldByUserId = customer.UserId;
        showtimeSeat.HeldUntil = DateTime.UtcNow.AddMinutes(-1);
        var booking = new Booking
        {
            UserId = customer.UserId,
            ShowtimeId = showtime.ShowtimeId,
            BookingCode = "BK-EXPIRE-1",
            CustomerName = "Nguyen Van A",
            TotalAmount = 100_000m,
            FinalAmount = 100_000m,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow.AddMinutes(-20),
            ExpiresAt = DateTime.UtcNow.AddMinutes(-5),
        };
        Db.Bookings.Add(booking);
        await Db.SaveChangesAsync();
        var sut = CreateSut();

        await sut.ExpireDueBookingsAsync(CancellationToken.None);

        var reloadedBooking = await Db.Bookings.SingleAsync(b => b.BookingId == booking.BookingId);
        reloadedBooking.Status.ShouldBe(BookingStatus.Expired);
        var reloadedSeat = await Db.ShowtimeSeats.SingleAsync(ss =>
            ss.ShowtimeSeatId == showtimeSeat.ShowtimeSeatId
        );
        reloadedSeat.Status.ShouldBe(ShowtimeSeatStatus.Available);
        _realtimeNotifierMock.Verify(
            n =>
                n.NotifyHoldExpiredAsync(
                    showtime.ShowtimeId,
                    booking.BookingId,
                    It.IsAny<List<int>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task ExpireDueBookingsAsync_PendingBookingNotYetExpired_IsLeftUntouched()
    {
        var customer = TestDataSeeder.AddCustomer(Db);
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats);
        var booking = new Booking
        {
            UserId = customer.UserId,
            ShowtimeId = showtime.ShowtimeId,
            BookingCode = "BK-ACTIVE-1",
            CustomerName = "Nguyen Van A",
            TotalAmount = 100_000m,
            FinalAmount = 100_000m,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
        };
        Db.Bookings.Add(booking);
        await Db.SaveChangesAsync();
        var sut = CreateSut();

        await sut.ExpireDueBookingsAsync(CancellationToken.None);

        var reloaded = await Db.Bookings.SingleAsync(b => b.BookingId == booking.BookingId);
        reloaded.Status.ShouldBe(BookingStatus.Pending);
    }

    private Booking NewBooking(
        int showtimeId,
        string code,
        DateTime expiresAt,
        int? userId,
        BookingChannel channel = BookingChannel.Online,
        int? employeeId = null
    ) =>
        new()
        {
            UserId = userId,
            ShowtimeId = showtimeId,
            BookingCode = code,
            Channel = channel,
            CreatedByEmployeeId = employeeId,
            CustomerName = "Nguyen Van A",
            TotalAmount = 100_000m,
            FinalAmount = 100_000m,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow.AddMinutes(-20),
            ExpiresAt = expiresAt,
        };

    [Fact]
    public async Task ExpireDueBookingsAsync_OldBookingExpired_DoesNotReleaseSeatReheldByNewBooking()
    {
        var customer = TestDataSeeder.AddCustomer(Db);
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats);
        var oldBooking = NewBooking(showtime.ShowtimeId, "BK-OLD-1", DateTime.UtcNow.AddMinutes(-5), customer.UserId);
        var newBooking = NewBooking(showtime.ShowtimeId, "BK-NEW-1", DateTime.UtcNow.AddMinutes(10), customer.UserId);
        Db.Bookings.AddRange(oldBooking, newBooking);
        await Db.SaveChangesAsync();
        var seat = await Db.ShowtimeSeats.SingleAsync(ss => ss.ShowtimeId == showtime.ShowtimeId);
        seat.Status = ShowtimeSeatStatus.Held;
        seat.HeldByUserId = customer.UserId;
        seat.HeldByBookingId = newBooking.BookingId;
        seat.HeldUntil = DateTime.UtcNow.AddMinutes(10);
        await Db.SaveChangesAsync();
        var sut = CreateSut();

        await sut.ExpireDueBookingsAsync(CancellationToken.None);

        (await Db.Bookings.AsNoTracking().SingleAsync(b => b.BookingId == oldBooking.BookingId))
            .Status.ShouldBe(BookingStatus.Expired);
        var reloaded = await Db.ShowtimeSeats.AsNoTracking().SingleAsync(ss => ss.ShowtimeSeatId == seat.ShowtimeSeatId);
        reloaded.Status.ShouldBe(ShowtimeSeatStatus.Held);
        reloaded.HeldByBookingId.ShouldBe(newBooking.BookingId);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ExpireDueBookingsAsync_CounterBookingLinkedToMember_ReleasesSeatsHeldByEmployee(
        bool seatBoundToBooking
    )
    {
        var member = TestDataSeeder.AddCustomer(Db);
        var (employeeUser, employee) = TestDataSeeder.AddEmployee(Db);
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats);
        var booking = NewBooking(
            showtime.ShowtimeId,
            "BK-COUNTER-1",
            DateTime.UtcNow.AddMinutes(-5),
            member.UserId,
            BookingChannel.Counter,
            employee.EmployeeId
        );
        Db.Bookings.Add(booking);
        await Db.SaveChangesAsync();
        var seat = await Db.ShowtimeSeats.SingleAsync(ss => ss.ShowtimeId == showtime.ShowtimeId);
        seat.Status = ShowtimeSeatStatus.Held;
        seat.HeldByUserId = employeeUser.UserId;
        seat.HeldByBookingId = seatBoundToBooking ? booking.BookingId : null;
        seat.HeldUntil = DateTime.UtcNow.AddMinutes(10);
        await Db.SaveChangesAsync();
        var sut = CreateSut();

        await sut.ExpireDueBookingsAsync(CancellationToken.None);

        var reloaded = await Db.ShowtimeSeats.AsNoTracking().SingleAsync(ss => ss.ShowtimeSeatId == seat.ShowtimeSeatId);
        reloaded.Status.ShouldBe(ShowtimeSeatStatus.Available);
        reloaded.HeldByUserId.ShouldBeNull();
        reloaded.HeldByBookingId.ShouldBeNull();
    }

    public void Dispose() => _handle.Dispose();
}
