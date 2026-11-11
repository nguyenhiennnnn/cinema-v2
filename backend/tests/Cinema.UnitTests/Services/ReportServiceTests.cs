using Cinema.Application.Services.Reports;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.UnitTests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class ReportServiceTests : IDisposable
{
    private readonly SqliteAppDbContextHandle _handle = new();

    private ReportService CreateSut()
    {
        _handle.Db.ChangeTracker.Clear();
        return new ReportService(_handle.Db);
    }

    private async Task<Booking> SeedConfirmedBookingAsync(
        DateTime createdAt,
        decimal ticketPrice = 100_000m,
        decimal? comboUnitPrice = null,
        int comboQuantity = 1,
        BookingChannel channel = BookingChannel.Online,
        int? createdByEmployeeId = null,
        int? movieOverrideId = null
    )
    {
        var customer = TestDataSeeder.AddCustomer(
            _handle.Db,
            email: $"customer-{Guid.NewGuid():N}@test.local"
        );
        var room = TestDataSeeder.AddRoom(_handle.Db, name: $"Room-{Guid.NewGuid():N}");
        var seats = TestDataSeeder.AddSeats(_handle.Db, room.RoomId, count: 1);
        var movie = movieOverrideId is null
            ? TestDataSeeder.AddMovie(_handle.Db, title: $"Movie-{Guid.NewGuid():N}")
            : await _handle.Db.Movies.FirstAsync(m => m.MovieId == movieOverrideId);
        var showtime = TestDataSeeder.AddShowtime(
            _handle.Db,
            movie,
            room,
            seats,
            basePrice: ticketPrice
        );
        var showtimeSeat = await _handle.Db.ShowtimeSeats.FirstAsync(ss =>
            ss.ShowtimeId == showtime.ShowtimeId
        );

        var finalAmount = ticketPrice + (comboUnitPrice ?? 0m) * comboQuantity;
        var booking = new Booking
        {
            UserId = customer.UserId,
            ShowtimeId = showtime.ShowtimeId,
            BookingCode = $"BK{Guid.NewGuid():N}"[..14].ToUpperInvariant(),
            CustomerName = "Nguyen Van A",
            Channel = channel,
            CreatedByEmployeeId = createdByEmployeeId,
            TotalAmount = finalAmount,
            DiscountAmount = 0m,
            FinalAmount = finalAmount,
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
                Amount = finalAmount,
                Status = PaymentStatus.Success,
                PaidAt = createdAt,
                CreatedAt = createdAt,
            }
        );

        _handle.Db.BookingTickets.Add(
            new BookingTicket
            {
                BookingId = booking.BookingId,
                ShowtimeSeatId = showtimeSeat.ShowtimeSeatId,
                Price = ticketPrice,
                QrCode = $"QR-{booking.BookingId}",
                IsCheckedIn = false,
            }
        );

        if (comboUnitPrice is not null)
        {
            var combo = new Combo
            {
                Name = $"Combo-{Guid.NewGuid():N}",
                Price = comboUnitPrice.Value,
                IsActive = true,
            };
            _handle.Db.Combos.Add(combo);
            await _handle.Db.SaveChangesAsync();

            _handle.Db.BookingCombos.Add(
                new BookingCombo
                {
                    BookingId = booking.BookingId,
                    ComboId = combo.ComboId,
                    Quantity = comboQuantity,
                    UnitPrice = comboUnitPrice.Value,
                }
            );
        }

        await _handle.Db.SaveChangesAsync();
        return booking;
    }

    [Fact]
    public async Task GetRevenueReportAsync_InvalidGroupBy_ThrowsValidationAppException()
    {
        var sut = CreateSut();

        await Should.ThrowAsync<ValidationAppException>(() =>
            sut.GetRevenueReportAsync(
                DateOnly.FromDateTime(DateTime.UtcNow),
                DateOnly.FromDateTime(DateTime.UtcNow),
                "year",
                CancellationToken.None
            )
        );
    }

    [Fact]
    public async Task GetRevenueReportAsync_ConfirmedBookingsWithTicketsAndCombos_ComputesTicketComboAndTotalRevenueCorrectly()
    {
        var today = DateTime.UtcNow.Date.AddHours(10);
        await SeedConfirmedBookingAsync(
            today,
            ticketPrice: 100_000m,
            comboUnitPrice: 50_000m,
            comboQuantity: 2
        );
        await SeedConfirmedBookingAsync(today.AddHours(1), ticketPrice: 80_000m);
        var sut = CreateSut();

        var report = await sut.GetRevenueReportAsync(
            DateOnly.FromDateTime(today),
            DateOnly.FromDateTime(today),
            "day",
            CancellationToken.None
        );

        report.Items.Count.ShouldBe(1);
        var item = report.Items[0];
        item.TicketRevenue.ShouldBe(180_000m);
        item.ComboRevenue.ShouldBe(100_000m);
        item.TotalRevenue.ShouldBe(280_000m);
        item.BookingCount.ShouldBe(2);

        report.Summary.TotalRevenue.ShouldBe(280_000m);
        report.Summary.TotalBookings.ShouldBe(2);
    }

    [Fact]
    public async Task GetRevenueReportAsync_BookingOutsideDateRange_IsExcludedFromReport()
    {
        var today = DateTime.UtcNow.Date;
        await SeedConfirmedBookingAsync(today.AddDays(-5), ticketPrice: 100_000m);
        var sut = CreateSut();

        var report = await sut.GetRevenueReportAsync(
            DateOnly.FromDateTime(today),
            DateOnly.FromDateTime(today),
            "day",
            CancellationToken.None
        );

        report.Items.ShouldBeEmpty();
        report.Summary.TotalRevenue.ShouldBe(0m);
        report.Summary.TotalBookings.ShouldBe(0);
    }

    [Fact]
    public async Task GetRevenueReportAsync_PendingOrCancelledBookings_AreExcludedFromRevenue()
    {
        var today = DateTime.UtcNow.Date.AddHours(9);
        var confirmed = await SeedConfirmedBookingAsync(today, ticketPrice: 100_000m);

        var customer = TestDataSeeder.AddCustomer(_handle.Db, email: "pending-customer@test.local");
        var room = TestDataSeeder.AddRoom(_handle.Db, name: "Room-Pending");
        var seats = TestDataSeeder.AddSeats(_handle.Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(_handle.Db, title: "Pending Movie");
        var showtime = TestDataSeeder.AddShowtime(_handle.Db, movie, room, seats);
        _handle.Db.Bookings.Add(
            new Booking
            {
                UserId = customer.UserId,
                ShowtimeId = showtime.ShowtimeId,
                BookingCode = "BK-PENDING-0001",
                CustomerName = "Pending Customer",
                TotalAmount = 999_000m,
                FinalAmount = 999_000m,
                Status = BookingStatus.Pending,
                CreatedAt = today,
            }
        );
        await _handle.Db.SaveChangesAsync();

        var sut = CreateSut();
        var report = await sut.GetRevenueReportAsync(
            DateOnly.FromDateTime(today),
            DateOnly.FromDateTime(today),
            "day",
            CancellationToken.None
        );

        report.Summary.TotalRevenue.ShouldBe(confirmed.FinalAmount);
        report.Summary.TotalBookings.ShouldBe(1);
    }

    private async Task SetPaymentAsync(
        Booking booking,
        BookingStatus status,
        PaymentStatus paymentStatus,
        decimal? refunded,
        DateTime paidAt
    )
    {
        var payment = await _handle.Db.Payments.SingleAsync(p => p.BookingId == booking.BookingId);
        payment.Status = paymentStatus;
        payment.RefundedAmount = refunded;
        payment.PaidAt = paidAt;
        var tracked = await _handle.Db.Bookings.SingleAsync(b => b.BookingId == booking.BookingId);
        tracked.Status = status;
        await _handle.Db.SaveChangesAsync();
    }

    [Fact]
    public async Task GetRevenueReportAsync_CancelledBookingWithPartialRefund_CountsRetainedAmount()
    {
        var paidAt = DateTime.UtcNow.Date.AddHours(9);
        var booking = await SeedConfirmedBookingAsync(paidAt, ticketPrice: 100_000m);
        await SetPaymentAsync(booking, BookingStatus.Cancelled, PaymentStatus.Refunded, 40_000m, paidAt);
        var sut = CreateSut();

        var report = await sut.GetRevenueReportAsync(
            DateOnly.FromDateTime(paidAt),
            DateOnly.FromDateTime(paidAt),
            "day",
            CancellationToken.None
        );

        report.Summary.TotalRevenue.ShouldBe(60_000m);
        report.Summary.TotalBookings.ShouldBe(1);
        report.Items.Single().TicketRevenue.ShouldBe(60_000m);
    }

    [Fact]
    public async Task GetRevenueReportAsync_FullyRefundedBooking_ContributesNothing()
    {
        var paidAt = DateTime.UtcNow.Date.AddHours(9);
        var booking = await SeedConfirmedBookingAsync(paidAt, ticketPrice: 100_000m);
        await SetPaymentAsync(booking, BookingStatus.Cancelled, PaymentStatus.Refunded, 100_000m, paidAt);
        var sut = CreateSut();

        var report = await sut.GetRevenueReportAsync(
            DateOnly.FromDateTime(paidAt),
            DateOnly.FromDateTime(paidAt),
            "day",
            CancellationToken.None
        );

        report.Summary.TotalRevenue.ShouldBe(0m);
        report.Summary.TotalBookings.ShouldBe(0);
    }

    [Fact]
    public async Task GetRevenueReportAsync_GroupsByPaidAtInBusinessTime()
    {
        var day = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        var paidAt = new DateTime(day.Year, day.Month, day.Day, 18, 0, 0, DateTimeKind.Utc);
        var booking = await SeedConfirmedBookingAsync(paidAt.AddDays(-3), ticketPrice: 100_000m);
        await SetPaymentAsync(booking, BookingStatus.Confirmed, PaymentStatus.Success, null, paidAt);
        var sut = CreateSut();

        var report = await sut.GetRevenueReportAsync(day.AddDays(1), day.AddDays(1), "day", CancellationToken.None);

        report.Summary.TotalRevenue.ShouldBe(100_000m);
        report.Items.Single().Period.ShouldBe(day.AddDays(1).ToString("dd/MM/yyyy"));
    }

    [Fact]
    public async Task GetTopMoviesReportAsync_MultipleMoviesWithDifferentRevenue_OrdersDescendingAndRespectsLimit()
    {
        var today = DateTime.UtcNow.Date.AddHours(10);
        var movieA = TestDataSeeder.AddMovie(_handle.Db, title: "Movie A - low revenue");
        var movieB = TestDataSeeder.AddMovie(_handle.Db, title: "Movie B - high revenue");

        await SeedConfirmedBookingAsync(
            today,
            ticketPrice: 50_000m,
            movieOverrideId: movieA.MovieId
        );
        await SeedConfirmedBookingAsync(
            today,
            ticketPrice: 200_000m,
            movieOverrideId: movieB.MovieId
        );
        await SeedConfirmedBookingAsync(
            today,
            ticketPrice: 200_000m,
            movieOverrideId: movieB.MovieId
        );

        var sut = CreateSut();
        var results = await sut.GetTopMoviesReportAsync(
            DateOnly.FromDateTime(today),
            DateOnly.FromDateTime(today),
            limit: 1,
            CancellationToken.None
        );

        results.Count.ShouldBe(1);
        results[0].MovieId.ShouldBe(movieB.MovieId);
        results[0].TicketsSold.ShouldBe(2);
        results[0].Revenue.ShouldBe(400_000m);
    }

    [Fact]
    public async Task GetEmployeePerformanceReportAsync_OnlyCountsConfirmedCounterBookingsWithEmployee()
    {
        var today = DateTime.UtcNow.Date.AddHours(10);
        var (employeeUser, employee) = TestDataSeeder.AddEmployee(
            _handle.Db,
            email: "staff@test.local"
        );

        await SeedConfirmedBookingAsync(
            today,
            ticketPrice: 120_000m,
            channel: BookingChannel.Counter,
            createdByEmployeeId: employee.EmployeeId
        );

        await SeedConfirmedBookingAsync(
            today,
            ticketPrice: 500_000m,
            channel: BookingChannel.Online
        );

        var sut = CreateSut();
        var results = await sut.GetEmployeePerformanceReportAsync(
            DateOnly.FromDateTime(today),
            DateOnly.FromDateTime(today),
            CancellationToken.None
        );

        results.Count.ShouldBe(1);
        results[0].EmployeeId.ShouldBe(employee.EmployeeId);
        results[0].FullName.ShouldBe(employeeUser.FullName);
        results[0].TicketsSold.ShouldBe(1);
        results[0].Revenue.ShouldBe(120_000m);
        results[0].BookingCount.ShouldBe(1);
    }

    public void Dispose() => _handle.Dispose();
}
