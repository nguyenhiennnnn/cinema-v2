using Cinema.Application.Utils;
using Cinema.Application.BackgroundJobs;
using Cinema.Application.DTOs.Showtimes;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Services.Notifications;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.UnitTests.TestSupport;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class ShowtimeServiceFixesTests : ShowtimeServiceTestBase
{
    private static DateTime Day(int daysFromNow, int hour = 14) =>
        VietnamTime.ToUtc(VietnamTime.Today().AddDays(daysFromNow), new TimeOnly(hour, 0));

    private static Booking NewBooking(
        int showtimeId,
        string code,
        BookingStatus status,
        DateTime? expiresAt = null
    ) =>
        new()
        {
            ShowtimeId = showtimeId,
            BookingCode = code,
            CustomerName = "Nguyen Van A",
            Status = status,
            TotalAmount = 90_000m,
            FinalAmount = 90_000m,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt,
        };

    // ── Update ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateShowtimeAsync_PersistsChangesToDatabase()
    {
        var room = TestDataSeeder.AddRoom(Db);
        TestDataSeeder.AddSeats(Db, room.RoomId, count: 2);
        var movie = TestDataSeeder.AddMovie(Db, durationMinutes: 100);
        var sut = CreateSut();
        var created = await sut.CreateShowtimeAsync(
            new UpsertShowtimeRequest(movie.MovieId, room.RoomId, Day(1), 90_000m),
            CancellationToken.None
        );

        await sut.UpdateShowtimeAsync(
            created.ShowtimeId,
            new UpsertShowtimeRequest(movie.MovieId, room.RoomId, Day(2, 16), 120_000m),
            CancellationToken.None
        );

        Db.ChangeTracker.Clear(); // đọc lại từ DB, không dùng bản đang track
        var reloaded = await Db.Showtimes.AsNoTracking().SingleAsync(s => s.ShowtimeId == created.ShowtimeId);
        reloaded.StartTime.ShouldBe(Day(2, 16));
        reloaded.BasePrice.ShouldBe(120_000m);
        (await Db.ShowtimeSeats.AsNoTracking().Where(ss => ss.ShowtimeId == created.ShowtimeId).ToListAsync())
            .ShouldAllBe(ss => ss.Price == 120_000m);
    }

    [Theory]
    [InlineData(ShowtimeStatus.Completed)]
    [InlineData(ShowtimeStatus.Cancelled)]
    public async Task UpdateShowtimeAsync_NotScheduled_ThrowsBusinessRuleException(ShowtimeStatus status)
    {
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats, Day(-1), status: status);
        var sut = CreateSut();

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.UpdateShowtimeAsync(
                showtime.ShowtimeId,
                new UpsertShowtimeRequest(movie.MovieId, room.RoomId, Day(3), 90_000m),
                CancellationToken.None
            )
        );
    }

    [Fact]
    public async Task UpdateShowtimeAsync_ActivePendingBooking_ThrowsConflict()
    {
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats, Day(1));
        Db.Bookings.Add(
            NewBooking(showtime.ShowtimeId, "BK-PENDING-LIVE", BookingStatus.Pending, DateTime.UtcNow.AddMinutes(10))
        );
        await Db.SaveChangesAsync();
        var sut = CreateSut();

        await Should.ThrowAsync<ConflictAppException>(() =>
            sut.UpdateShowtimeAsync(
                showtime.ShowtimeId,
                new UpsertShowtimeRequest(movie.MovieId, room.RoomId, Day(2), 90_000m),
                CancellationToken.None
            )
        );
    }

    [Fact]
    public async Task UpdateShowtimeAsync_ActiveHeldSeat_ThrowsConflict()
    {
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats, Day(1));
        var showtimeSeat = await Db.ShowtimeSeats.SingleAsync(ss => ss.ShowtimeId == showtime.ShowtimeId);
        showtimeSeat.Status = ShowtimeSeatStatus.Held;
        showtimeSeat.HeldByUserId = TestDataSeeder.AddCustomer(Db).UserId;
        showtimeSeat.HeldUntil = DateTime.UtcNow.AddMinutes(10);
        await Db.SaveChangesAsync();
        var sut = CreateSut();

        await Should.ThrowAsync<ConflictAppException>(() =>
            sut.UpdateShowtimeAsync(
                showtime.ShowtimeId,
                new UpsertShowtimeRequest(movie.MovieId, room.RoomId, Day(2), 90_000m),
                CancellationToken.None
            )
        );
    }

    [Fact]
    public async Task UpdateShowtimeAsync_ExpiredPendingBooking_DoesNotBlock()
    {
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats, Day(1));
        Db.Bookings.Add(
            NewBooking(showtime.ShowtimeId, "BK-PENDING-OLD", BookingStatus.Pending, DateTime.UtcNow.AddMinutes(-5))
        );
        await Db.SaveChangesAsync();
        var sut = CreateSut();

        var result = await sut.UpdateShowtimeAsync(
            showtime.ShowtimeId,
            new UpsertShowtimeRequest(movie.MovieId, room.RoomId, Day(2), 90_000m),
            CancellationToken.None
        );

        result.StartTime.ShouldBe(Day(2));
    }

    // ── Delete ───────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(ShowtimeStatus.Completed)]
    [InlineData(ShowtimeStatus.Cancelled)]
    public async Task DeleteShowtimeAsync_NotScheduled_ThrowsAndLeavesBookingsUntouched(ShowtimeStatus status)
    {
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats, Day(-1), status: status);
        Db.Bookings.Add(NewBooking(showtime.ShowtimeId, "BK-DONE", BookingStatus.Confirmed));
        await Db.SaveChangesAsync();
        var sut = CreateSut();

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.DeleteShowtimeAsync(showtime.ShowtimeId, CancellationToken.None)
        );

        var booking = await Db.Bookings.AsNoTracking().SingleAsync(b => b.BookingCode == "BK-DONE");
        booking.Status.ShouldBe(BookingStatus.Confirmed);
        var reloaded = await Db.Showtimes.AsNoTracking().SingleAsync(s => s.ShowtimeId == showtime.ShowtimeId);
        reloaded.Status.ShouldBe(status);
    }

    [Fact]
    public async Task DeleteShowtimeAsync_OnlinePayment_QueuesRefundJobInsteadOfCallingGateway()
    {
        var gateway = AddPaymentGateway("VNPay");
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats, Day(1));
        var booking = NewBooking(showtime.ShowtimeId, "BK-ONLINE", BookingStatus.Confirmed);
        Db.Bookings.Add(booking);
        await Db.SaveChangesAsync();
        Db.Payments.Add(
            new Payment
            {
                BookingId = booking.BookingId,
                PaymentMethod = PaymentMethod.VNPay,
                TransactionId = "TX-1",
                Amount = 90_000m,
                Status = PaymentStatus.Success,
                PaidAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
            }
        );
        await Db.SaveChangesAsync();
        var sut = CreateSut();

        var result = await sut.DeleteShowtimeAsync(showtime.ShowtimeId, CancellationToken.None);

        result.AutoRefundQueuedCount.ShouldBe(1);
        result.ManualRefundRequiredCount.ShouldBe(0);
        gateway.Verify(
            g => g.RefundAsync(It.IsAny<RefundRequest>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        BackgroundJobClientMock.Verify(
            c => c.Create(It.Is<Job>(j => j.Type == typeof(ShowtimeRefundJob)), It.IsAny<IState>()),
            Times.Once
        );
        (await Db.Bookings.AsNoTracking().SingleAsync(b => b.BookingId == booking.BookingId))
            .Status.ShouldBe(BookingStatus.Cancelled);
    }

    // ── ShowtimeRefundJob ────────────────────────────────────────────────────

    private async Task<int> SeedCancelledOnlinePaymentAsync()
    {
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(
            Db, movie, room, seats, Day(1), status: ShowtimeStatus.Cancelled);
        var booking = NewBooking(showtime.ShowtimeId, "BK-REFUND", BookingStatus.Cancelled);
        Db.Bookings.Add(booking);
        await Db.SaveChangesAsync();
        var payment = new Payment
        {
            BookingId = booking.BookingId,
            PaymentMethod = PaymentMethod.VNPay,
            TransactionId = "TX-2",
            Amount = 90_000m,
            Status = PaymentStatus.Success,
            PaidAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
        };
        Db.Payments.Add(payment);
        await Db.SaveChangesAsync();
        return payment.PaymentId;
    }

    private ShowtimeRefundJob CreateRefundJob()
    {
        Db.ChangeTracker.Clear();
        return new ShowtimeRefundJob(
            Db,
            PaymentGatewayMocks.Select(m => m.Object),
            new NotificationService(BackgroundJobClientMock.Object, Options.Create(new BrandOptions())),
            NullLogger<ShowtimeRefundJob>.Instance
        );
    }

    [Fact]
    public async Task ShowtimeRefundJob_Success_MarksRefunded_AndIsIdempotent()
    {
        var gateway = AddPaymentGateway("VNPay");
        gateway
            .Setup(g => g.RefundAsync(It.IsAny<RefundRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RefundResult(true, "RF-1", null));
        var paymentId = await SeedCancelledOnlinePaymentAsync();
        var job = CreateRefundJob();

        await job.RunAsync(paymentId, null, CancellationToken.None);
        await job.RunAsync(paymentId, null, CancellationToken.None); // chạy lại: phải bỏ qua

        gateway.Verify(
            g => g.RefundAsync(It.IsAny<RefundRequest>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        var payment = await Db.Payments.AsNoTracking().SingleAsync(p => p.PaymentId == paymentId);
        payment.Status.ShouldBe(PaymentStatus.Refunded);
        payment.RefundedAmount.ShouldBe(90_000m);
        payment.RefundTransactionId.ShouldBe("RF-1");
    }

    [Fact]
    public async Task ShowtimeRefundJob_GatewayRejects_KeepsPaymentInManualQueue()
    {
        var gateway = AddPaymentGateway("VNPay");
        gateway
            .Setup(g => g.RefundAsync(It.IsAny<RefundRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RefundResult(false, null, "Hết hạn hoàn tiền"));
        var paymentId = await SeedCancelledOnlinePaymentAsync();
        var job = CreateRefundJob();

        await Should.ThrowAsync<InvalidOperationException>(() =>
            job.RunAsync(paymentId, null, CancellationToken.None)
        );

        var payment = await Db.Payments.AsNoTracking().SingleAsync(p => p.PaymentId == paymentId);
        payment.Status.ShouldBe(PaymentStatus.Success);
    }

    // ── Batch create ─────────────────────────────────────────────────────────

    [Fact]
    public async Task ApplyAutoScheduleAsync_UnknownMovie_IsSkippedWithReason_OthersStillCreated()
    {
        var room = TestDataSeeder.AddRoom(Db);
        TestDataSeeder.AddSeats(Db, room.RoomId, count: 3);
        var movie = TestDataSeeder.AddMovie(Db, durationMinutes: 100);
        var sut = CreateSut();

        var result = await sut.ApplyAutoScheduleAsync(
            new ApplyAutoScheduleRequest(
                [
                    new UpsertShowtimeRequest(999, room.RoomId, Day(2, 10), 90_000m),
                    new UpsertShowtimeRequest(movie.MovieId, room.RoomId, Day(2, 12), 90_000m),
                ]
            ),
            CancellationToken.None
        );

        result.TotalCreated.ShouldBe(1);
        result.TotalSkipped.ShouldBe(1);
        result.Skipped[0].Reason.ShouldContain("movieId");
        result.Created[0].SeatsGenerated.ShouldBe(3);
        (await Db.ShowtimeSeats.CountAsync()).ShouldBe(3);
    }

    [Fact]
    public async Task BulkCreateShowtimesAsync_SkipsOverlapWithExistingShowtime_AndCreatesTheRest()
    {
        var room = TestDataSeeder.AddRoom(Db);
        TestDataSeeder.AddSeats(Db, room.RoomId, count: 2);
        var movie = TestDataSeeder.AddMovie(Db, durationMinutes: 100);
        var sut = CreateSut();
        var target = VietnamTime.LocalDate(Day(2));
        await sut.CreateShowtimeAsync(
            new UpsertShowtimeRequest(movie.MovieId, room.RoomId, VietnamTime.ToUtc(target, new TimeOnly(14, 0)), 90_000m),
            CancellationToken.None
        );

        var result = await sut.BulkCreateShowtimesAsync(
            new BulkCreateShowtimesRequest(
                movie.MovieId,
                room.RoomId,
                new BulkDateRange(target, target),
                [target.DayOfWeek.ToString()[..3]],
                ["10:00", "14:30", "18:00"],
                90_000m
            ),
            CancellationToken.None
        );

        result.TotalCreated.ShouldBe(2); // 10:00 và 18:00
        result.TotalSkipped.ShouldBe(1); // 14:30 đè suất 14:00
        result.Skipped[0].Time.ShouldBe("14:30");
    }

    // ── Auto-schedule nhiều ngày + nhân bản ──────────────────────────────────

    [Fact]
    public async Task PreviewAutoScheduleAsync_DateRange_PlacesShowsOnEveryDay()
    {
        var room = TestDataSeeder.AddRoom(Db);
        TestDataSeeder.AddSeats(Db, room.RoomId, count: 4);
        var movie = TestDataSeeder.AddMovie(Db, durationMinutes: 100);
        var sut = CreateSut();
        var from = VietnamTime.LocalDate(Day(2));
        var to = from.AddDays(2);

        var result = await sut.PreviewAutoScheduleAsync(
            new AutoScheduleRequest(
                from,
                [new AutoScheduleMovieRequest(movie.MovieId, 2)],
                90_000m,
                ToDate: to
            ),
            CancellationToken.None
        );

        result.Items.Count.ShouldBe(6);
        result.Unplaced.ShouldBeEmpty();
        result
            .Items.Select(i => VietnamTime.LocalDate(i.StartTime))
            .Distinct()
            .Count()
            .ShouldBe(3);
    }

    [Fact]
    public async Task CloneShowtimesAsync_ShiftsSourceShowtimes_AndSkipsOnSecondRun()
    {
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 2);
        var movie = TestDataSeeder.AddMovie(Db, durationMinutes: 100);
        var sourceDay = VietnamTime.LocalDate(Day(-1));
        TestDataSeeder.AddShowtime(
            Db, movie, room, seats, VietnamTime.ToUtc(sourceDay, new TimeOnly(14, 0)),
            status: ShowtimeStatus.Completed);
        var sut = CreateSut();
        var request = new CloneShowtimesRequest(sourceDay, sourceDay, ShiftDays: 7);

        var first = await sut.CloneShowtimesAsync(request, CancellationToken.None);
        var second = await sut.CloneShowtimesAsync(request, CancellationToken.None);

        first.TotalCreated.ShouldBe(1);
        first.Created[0].StartTime.ShouldBe(VietnamTime.ToUtc(sourceDay.AddDays(7), new TimeOnly(14, 0)));
        first.Created[0].SeatsGenerated.ShouldBe(2);
        second.TotalCreated.ShouldBe(0);
        second.TotalSkipped.ShouldBe(1);
    }

    [Fact]
    public async Task CloneShowtimesAsync_NoSourceShowtimes_ThrowsBusinessRuleException()
    {
        var sut = CreateSut();
        var day = VietnamTime.LocalDate(Day(-30));

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.CloneShowtimesAsync(new CloneShowtimesRequest(day, day), CancellationToken.None)
        );
    }

    [Fact]
    public async Task CreateShowtimeAsync_StartTimeInThePast_ThrowsBusinessRuleException()
    {
        var room = TestDataSeeder.AddRoom(Db);
        TestDataSeeder.AddSeats(Db, room.RoomId, count: 2);
        var movie = TestDataSeeder.AddMovie(Db);
        var sut = CreateSut();

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.CreateShowtimeAsync(
                new UpsertShowtimeRequest(movie.MovieId, room.RoomId, DateTime.UtcNow.AddHours(-1), 90_000m, true),
                CancellationToken.None
            )
        );
        (await Db.Showtimes.CountAsync()).ShouldBe(0);
    }

    [Fact]
    public async Task UpdateShowtimeAsync_StartTimeInThePast_ThrowsBusinessRuleException()
    {
        var room = TestDataSeeder.AddRoom(Db);
        TestDataSeeder.AddSeats(Db, room.RoomId, count: 2);
        var movie = TestDataSeeder.AddMovie(Db);
        var sut = CreateSut();
        var created = await sut.CreateShowtimeAsync(
            new UpsertShowtimeRequest(movie.MovieId, room.RoomId, Day(1), 90_000m),
            CancellationToken.None
        );

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.UpdateShowtimeAsync(
                created.ShowtimeId,
                new UpsertShowtimeRequest(movie.MovieId, room.RoomId, DateTime.UtcNow.AddHours(-1), 90_000m, true),
                CancellationToken.None
            )
        );
    }

    [Fact]
    public async Task BulkCreateShowtimesAsync_DatesInThePast_AreSkippedWithFutureReason()
    {
        var room = TestDataSeeder.AddRoom(Db);
        TestDataSeeder.AddSeats(Db, room.RoomId, count: 2);
        var movie = TestDataSeeder.AddMovie(Db);
        var yesterday = VietnamTime.Today().AddDays(-1);
        var sut = CreateSut();

        var result = await sut.BulkCreateShowtimesAsync(
            new BulkCreateShowtimesRequest(
                movie.MovieId,
                room.RoomId,
                new BulkDateRange(yesterday, yesterday),
                [yesterday.DayOfWeek.ToString()[..3]],
                ["10:00"],
                90_000m
            ),
            CancellationToken.None
        );

        result.TotalCreated.ShouldBe(0);
        result.TotalSkipped.ShouldBe(1);
        result.Skipped[0].Reason.ShouldContain("tương lai");
    }

    [Fact]
    public async Task DeleteShowtimeAsync_ShowtimeAlreadyStarted_ThrowsAndKeepsScheduled()
    {
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats, DateTime.UtcNow.AddMinutes(-30));
        var sut = CreateSut();

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.DeleteShowtimeAsync(showtime.ShowtimeId, CancellationToken.None)
        );

        (await Db.Showtimes.AsNoTracking().SingleAsync(s => s.ShowtimeId == showtime.ShowtimeId))
            .Status.ShouldBe(ShowtimeStatus.Scheduled);
    }

    [Fact]
    public async Task DeleteShowtimeAsync_PendingBooking_MarksPendingPaymentsFailed()
    {
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats, Day(1));
        var booking = NewBooking(showtime.ShowtimeId, "BK-PEND-PAY", BookingStatus.Pending, DateTime.UtcNow.AddMinutes(10));
        Db.Bookings.Add(booking);
        await Db.SaveChangesAsync();
        Db.Payments.Add(
            new Payment
            {
                BookingId = booking.BookingId,
                PaymentMethod = PaymentMethod.VNPay,
                TransactionId = "TX-PEND",
                Amount = 90_000m,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow,
            }
        );
        await Db.SaveChangesAsync();
        var sut = CreateSut();

        await sut.DeleteShowtimeAsync(showtime.ShowtimeId, CancellationToken.None);

        (await Db.Bookings.AsNoTracking().SingleAsync(b => b.BookingId == booking.BookingId))
            .Status.ShouldBe(BookingStatus.Cancelled);
        (await Db.Payments.AsNoTracking().SingleAsync(p => p.BookingId == booking.BookingId))
            .Status.ShouldBe(PaymentStatus.Failed);
    }
}
