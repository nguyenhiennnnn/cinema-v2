using Cinema.Application.Utils;
using Cinema.Application.Services.Dashboard;
using Cinema.Application.Services.Reports;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.UnitTests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class DashboardServiceTests : IDisposable
{
    private readonly SqliteAppDbContextHandle _handle = new();

    private DashboardService CreateSut()
    {
        _handle.Db.ChangeTracker.Clear();
        return new DashboardService(_handle.Db, new ReportService(_handle.Db));
    }

    private async Task<(Showtime Showtime, List<ShowtimeSeat> Seats)> SeedShowtimeWithSeatsAsync(
        DateTime startTime,
        int seatCount = 4
    )
    {
        var room = TestDataSeeder.AddRoom(_handle.Db, name: $"Room-{Guid.NewGuid():N}");
        var seats = TestDataSeeder.AddSeats(_handle.Db, room.RoomId, count: seatCount);
        var movie = TestDataSeeder.AddMovie(_handle.Db, title: $"Movie-{Guid.NewGuid():N}");
        var showtime = TestDataSeeder.AddShowtime(
            _handle.Db,
            movie,
            room,
            seats,
            startTime: startTime
        );
        var showtimeSeats = await _handle
            .Db.ShowtimeSeats.Where(ss => ss.ShowtimeId == showtime.ShowtimeId)
            .ToListAsync();
        return (showtime, showtimeSeats);
    }

    private async Task<Booking> AddConfirmedBookingAsync(
        int showtimeId,
        DateTime createdAt,
        decimal amount
    )
    {
        var customer = TestDataSeeder.AddCustomer(
            _handle.Db,
            email: $"customer-{Guid.NewGuid():N}@test.local"
        );
        var booking = new Booking
        {
            UserId = customer.UserId,
            ShowtimeId = showtimeId,
            BookingCode = $"BK{Guid.NewGuid():N}"[..14].ToUpperInvariant(),
            CustomerName = "Nguyen Van A",
            TotalAmount = amount,
            FinalAmount = amount,
            Status = BookingStatus.Confirmed,
            CreatedAt = createdAt,
        };
        _handle.Db.Bookings.Add(booking);
        await _handle.Db.SaveChangesAsync();

        _handle.Db.Payments.Add(
            new Payment
            {
                BookingId = booking.BookingId,
                PaymentMethod = PaymentMethod.Cash,
                Amount = amount,
                Status = PaymentStatus.Success,
                PaidAt = createdAt,
                CreatedAt = createdAt,
            }
        );

        var usedSeatIds = await _handle.Db.BookingTickets.Select(t => t.ShowtimeSeatId).ToListAsync();
        var seat = await _handle.Db.ShowtimeSeats.FirstOrDefaultAsync(ss => ss.ShowtimeId == showtimeId && !usedSeatIds.Contains(ss.ShowtimeSeatId));
        if (seat != null)
        {
            _handle.Db.BookingTickets.Add(
                new BookingTicket
                {
                    BookingId = booking.BookingId,
                    ShowtimeSeatId = seat.ShowtimeSeatId,
                    Price = amount,
                    QrCode = $"QR-{Guid.NewGuid():N}",
                }
            );
            await _handle.Db.SaveChangesAsync();
        }

        return booking;
    }

    [Fact]
    public async Task GetSummaryAsync_NoBookingsAtAll_ReturnsZeroesAndNullChangePercent()
    {
        var sut = CreateSut();

        var summary = await sut.GetSummaryAsync(CancellationToken.None);

        summary.TodayRevenue.ShouldBe(0m);
        summary.YesterdayRevenue.ShouldBe(0m);
        summary.RevenueChangePercent.ShouldBeNull();
        summary.TicketsSoldToday.ShouldBe(0);
        summary.ConfirmedBookingsToday.ShouldBe(0);
        summary.ActivePendingBookings.ShouldBe(0);
        summary.TodayOccupancyPercent.ShouldBeNull();
    }

    [Fact]
    public async Task GetSummaryAsync_TodayRevenueHigherThanYesterday_ComputesPositiveChangePercent()
    {
        var now = DateTime.UtcNow;
        var (showtime, _) = await SeedShowtimeWithSeatsAsync(now.AddDays(3));
        await AddConfirmedBookingAsync(showtime.ShowtimeId, now, 200_000m);
        await AddConfirmedBookingAsync(showtime.ShowtimeId, now.AddDays(-1), 100_000m); 
        var sut = CreateSut();

        var summary = await sut.GetSummaryAsync(CancellationToken.None);

        summary.TodayRevenue.ShouldBe(200_000m);
        summary.YesterdayRevenue.ShouldBe(100_000m);
        summary.RevenueChangePercent.ShouldBe(100.0m); 
    }

    [Fact]
    public async Task GetSummaryAsync_OnlyPendingBookingsWithActiveHold_CountAsActivePendingBookings()
    {
        var now = DateTime.UtcNow;
        var (showtime, _) = await SeedShowtimeWithSeatsAsync(now.AddDays(3));
        var customer = TestDataSeeder.AddCustomer(_handle.Db);

        _handle.Db.Bookings.Add(
            new Booking
            {
                UserId = customer.UserId,
                ShowtimeId = showtime.ShowtimeId,
                BookingCode = "BK-ACTIVE-0001",
                CustomerName = "Active Hold",
                TotalAmount = 100_000m,
                FinalAmount = 100_000m,
                Status = BookingStatus.Pending,
                ExpiresAt = now.AddMinutes(10),
                CreatedAt = now,
            }
        );
        _handle.Db.Bookings.Add(
            new Booking
            {
                UserId = customer.UserId,
                ShowtimeId = showtime.ShowtimeId,
                BookingCode = "BK-EXPIRED-0001",
                CustomerName = "Expired Hold",
                TotalAmount = 100_000m,
                FinalAmount = 100_000m,
                Status = BookingStatus.Pending,
                ExpiresAt = now.AddMinutes(-5),
                CreatedAt = now,
            }
        );
        await _handle.Db.SaveChangesAsync();
        var sut = CreateSut();

        var summary = await sut.GetSummaryAsync(CancellationToken.None);

        summary.ActivePendingBookings.ShouldBe(1);
    }

    [Fact]
    public async Task GetSummaryAsync_ShowtimesTodayWithSomeSeatsBooked_ComputesOccupancyPercent()
    {
        var now = DateTime.UtcNow;
        var (showtime, seats) = await SeedShowtimeWithSeatsAsync(
            BusinessTime.ToUtc(BusinessTime.Today(), new TimeOnly(20, 0)),
            seatCount: 4
        );
        var bookedSeat = seats[0];
        bookedSeat.Status = ShowtimeSeatStatus.Booked;
        await _handle.Db.SaveChangesAsync();

        var sut = CreateSut();
        var summary = await sut.GetSummaryAsync(CancellationToken.None);

        summary.TodayOccupancyPercent.ShouldBe(25.0m);
    }

    [Fact]
    public async Task GetInsightsAsync_DaysOutOfRange_IsClampedTo90()
    {
        var sut = CreateSut();

        var insights = await sut.GetInsightsAsync(9999, CancellationToken.None);

        insights.ShouldNotBeNull();
        insights.RevenueTrend.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetInsightsAsync_IncludesTodaysShowtimesAndTopMoviesWithinWindow()
    {
        var now = DateTime.UtcNow;
        var (showtime, _) = await SeedShowtimeWithSeatsAsync(
            BusinessTime.ToUtc(BusinessTime.Today(), new TimeOnly(18, 0))
        );
        await AddConfirmedBookingAsync(showtime.ShowtimeId, now, 150_000m);
        var sut = CreateSut();

        var insights = await sut.GetInsightsAsync(7, CancellationToken.None);

        insights.TodayShowtimes.ShouldContain(s => s.ShowtimeId == showtime.ShowtimeId);
        insights.TopMovies.ShouldContain(m => m.MovieId == showtime.MovieId);
        insights.RevenueTrend.Sum(r => r.TotalRevenue).ShouldBe(150_000m);
    }

    [Fact]
    public async Task GetSummaryAsync_BookingJustAfterVietnamMidnight_CountsAsTodayAndJustBeforeAsYesterday()
    {
        var startOfToday = BusinessTime.StartOfDayUtc(BusinessTime.Today());
        var (showtime, _) = await SeedShowtimeWithSeatsAsync(DateTime.UtcNow.AddDays(3));
        await AddConfirmedBookingAsync(showtime.ShowtimeId, startOfToday.AddMinutes(30), 200_000m);
        await AddConfirmedBookingAsync(showtime.ShowtimeId, startOfToday.AddMinutes(-30), 100_000m);
        var sut = CreateSut();

        var summary = await sut.GetSummaryAsync(CancellationToken.None);

        summary.TodayRevenue.ShouldBe(200_000m);
        summary.YesterdayRevenue.ShouldBe(100_000m);
    }

    [Fact]
    public async Task GetSummaryAsync_PartiallyRefundedPaymentToday_CountsOnlyRetainedAmount()
    {
        var (showtime, _) = await SeedShowtimeWithSeatsAsync(DateTime.UtcNow.AddDays(3));
        var booking = await AddConfirmedBookingAsync(showtime.ShowtimeId, DateTime.UtcNow, 100_000m);
        var payment = await _handle.Db.Payments.SingleAsync(p => p.BookingId == booking.BookingId);
        payment.Status = PaymentStatus.Refunded;
        payment.RefundedAmount = 30_000m;
        payment.PaidAt = BusinessTime.StartOfDayUtc(BusinessTime.Today()).AddMinutes(30);
        var tracked = await _handle.Db.Bookings.SingleAsync(b => b.BookingId == booking.BookingId);
        tracked.Status = BookingStatus.Cancelled;
        await _handle.Db.SaveChangesAsync();
        var sut = CreateSut();

        var summary = await sut.GetSummaryAsync(CancellationToken.None);

        summary.TodayRevenue.ShouldBe(70_000m);
    }

    public void Dispose() => _handle.Dispose();
}
