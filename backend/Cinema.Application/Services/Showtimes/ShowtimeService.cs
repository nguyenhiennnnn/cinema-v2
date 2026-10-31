using System.Globalization;
using Cinema.Application.BackgroundJobs;
using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Showtimes;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Hangfire;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cinema.Application.Services.Showtimes;

public class ShowtimeService(
    IAppDbContext db,
    IOptions<BookingOptions> bookingOptions,
    IEnumerable<IPaymentGateway> paymentGateways,
    IBackgroundJobClient backgroundJobClient,
    IBookingService bookingService,
    ILogger<ShowtimeService> logger,
    IOptions<BrandOptions>? brandOptions = null,
    IOptions<SchedulerOptions>? schedulerOptions = null
) : IShowtimeService
{
    private const int MaxBatchSize = 2000;
    private const int MaxAutoScheduleDays = 31;

    private readonly int _bufferMinutes = bookingOptions.Value.RoomBufferMinutes;
    private readonly BrandOptions _brand = brandOptions?.Value ?? new();
    private readonly SchedulerOptions _scheduler = schedulerOptions?.Value ?? new();

    private sealed record ShowtimeDraft(
        int MovieId,
        int RoomId,
        DateTime StartTime,
        decimal BasePrice,
        bool AllowAfterHours
    );

    public async Task<OffsetPageResult<AdminShowtimeDto>> GetShowtimesAdminAsync(
        AdminShowtimeListFilter filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = db.Showtimes.Include(s => s.Movie).Include(s => s.Room).AsQueryable();

        if (filter.RoomId is not null)
            query = query.Where(s => s.RoomId == filter.RoomId);
        if (filter.MovieId is not null)
            query = query.Where(s => s.MovieId == filter.MovieId);
        if (filter.Date is not null)
        {
            var dayStart = VietnamTime.StartOfDayUtc(filter.Date.Value);
            var dayEnd = VietnamTime.StartOfNextDayUtc(filter.Date.Value);
            query = query.Where(s => s.StartTime >= dayStart && s.StartTime < dayEnd);
        }

        if (
            !string.IsNullOrWhiteSpace(filter.Status)
            && Enum.TryParse<ShowtimeStatus>(filter.Status, out var status)
        )
            query = query.Where(s => s.Status == status);

        query = query.OrderByDescending(s => s.StartTime);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<AdminShowtimeDto>()
            .ToListAsync(cancellationToken);

        return OffsetPageResult<AdminShowtimeDto>.Create(items, page, pageSize, total);
    }

    public async Task<CreateShowtimeResponse> CreateShowtimeAsync(
        UpsertShowtimeRequest request,
        CancellationToken cancellationToken
    )
    {
        var movie =
            await db.Movies.FirstOrDefaultAsync(
                m => m.MovieId == request.MovieId,
                cancellationToken
            ) ?? throw new ValidationAppException("movieId không tồn tại.");
        var room =
            await db.CinemaRooms.FirstOrDefaultAsync(
                r => r.RoomId == request.RoomId,
                cancellationToken
            )
            ?? throw new ValidationAppException(
                "roomId không tồn tại.",
                new Dictionary<string, string[]> { ["roomId"] = ["roomId không tồn tại."] }
            );

        if (movie.Status == MovieStatus.Ended)
            throw new BusinessRuleException(
                "Phim đã ngừng chiếu (Ended), không thể tạo suất chiếu."
            );
        if (!room.IsActive)
            throw new BusinessRuleException("Phòng chiếu không hoạt động (inactive).");

        var seatCount = await db.Seats.CountAsync(
            s => s.RoomId == request.RoomId && s.IsActive && !s.IsAisle,
            cancellationToken
        );
        if (seatCount == 0)
            throw new BusinessRuleException("Phòng chiếu chưa được thiết lập sơ đồ ghế.");

        EnsureStartsInFuture(request.StartTime);

        var endTime = request.StartTime.AddMinutes(movie.DurationMinutes + _bufferMinutes);

        await ValidateOperatingHoursAsync(
            request.StartTime,
            request.StartTime.AddMinutes(movie.DurationMinutes),
            request.AllowAfterHours,
            cancellationToken
        );

        await EnsureNoOverlapAsync(
            request.RoomId,
            request.StartTime,
            endTime,
            excludeShowtimeId: null,
            cancellationToken
        );

        var showtime = new Showtime
        {
            MovieId = request.MovieId,
            RoomId = request.RoomId,
            StartTime = request.StartTime,
            EndTime = endTime,
            BasePrice = request.BasePrice,
            Status = ShowtimeStatus.Scheduled,
            CreatedAt = DateTime.UtcNow,
        };

        db.Showtimes.Add(showtime);

        var seatsGenerated = await GenerateShowtimeSeatsAsync(
            showtime,
            request.RoomId,
            request.BasePrice,
            cancellationToken
        );

        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "ShowtimeCreated ShowtimeId {ShowtimeId} MovieId {MovieId} RoomId {RoomId} SeatsGenerated {Count}",
            showtime.ShowtimeId,
            showtime.MovieId,
            showtime.RoomId,
            seatsGenerated
        );

        return new CreateShowtimeResponse(
            showtime.ShowtimeId,
            showtime.MovieId,
            movie.Title,
            showtime.RoomId,
            room.RoomName,
            showtime.StartTime,
            showtime.EndTime,
            showtime.BasePrice,
            showtime.Status.ToDtoStatus(),
            seatsGenerated
        );
    }

    public async Task<AdminShowtimeDto> UpdateShowtimeAsync(
        int showtimeId,
        UpsertShowtimeRequest request,
        CancellationToken cancellationToken
    )
    {
        var showtime =
            await db.Showtimes.FirstOrDefaultAsync(
                s => s.ShowtimeId == showtimeId,
                cancellationToken
            ) ?? throw new NotFoundException("Không tìm thấy suất chiếu.");

        EnsureScheduled(showtime, "sửa");

        if (request.RoomId != showtime.RoomId)
            throw new BusinessRuleException(
                "Không thể đổi phòng chiếu sau khi đã tạo suất chiếu. Hãy hủy và tạo suất chiếu mới."
            );

        var hasConfirmedBooking = await db.Bookings.AnyAsync(
            b => b.ShowtimeId == showtimeId && b.Status == BookingStatus.Confirmed,
            cancellationToken
        );
        if (hasConfirmedBooking)
            throw new ConflictAppException(
                "Suất chiếu đã có booking đã xác nhận (Confirmed), không thể sửa."
            );

        // Khách đang giữ ghế / đang thanh toán (Pending) cũng không được bị đổi suất giữa chừng.
        var now = DateTime.UtcNow;
        var hasActiveCheckout =
            await db.Bookings.AnyAsync(
                b =>
                    b.ShowtimeId == showtimeId
                    && b.Status == BookingStatus.Pending
                    && (b.ExpiresAt == null || b.ExpiresAt > now),
                cancellationToken
            )
            || await db.ShowtimeSeats.AnyAsync(
                ss =>
                    ss.ShowtimeId == showtimeId
                    && (
                        ss.Status == ShowtimeSeatStatus.Booked
                        || (
                            ss.Status == ShowtimeSeatStatus.Held
                            && (ss.HeldUntil == null || ss.HeldUntil > now)
                        )
                    ),
                cancellationToken
            );
        if (hasActiveCheckout)
            throw new ConflictAppException(
                "Suất chiếu đang có khách giữ ghế hoặc đang thanh toán (Pending), không thể sửa lúc này. Vui lòng thử lại sau vài phút."
            );

        var movie =
            await db.Movies.FirstOrDefaultAsync(
                m => m.MovieId == request.MovieId,
                cancellationToken
            )
            ?? throw new ValidationAppException(
                "movieId không tồn tại.",
                new Dictionary<string, string[]> { ["movieId"] = ["movieId không tồn tại."] }
            );
        if (movie.Status == MovieStatus.Ended)
            throw new BusinessRuleException(
                "Phim đã ngừng chiếu (Ended), không thể cập nhật suất chiếu."
            );

        EnsureStartsInFuture(request.StartTime);

        var endTime = request.StartTime.AddMinutes(movie.DurationMinutes + _bufferMinutes);
        if (
            showtime.StartTime != request.StartTime
            || showtime.MovieId != request.MovieId
            || request.AllowAfterHours
        )
            await ValidateOperatingHoursAsync(
                request.StartTime,
                request.StartTime.AddMinutes(movie.DurationMinutes),
                request.AllowAfterHours,
                cancellationToken
            );
        await EnsureNoOverlapAsync(
            showtime.RoomId,
            request.StartTime,
            endTime,
            excludeShowtimeId: showtimeId,
            cancellationToken
        );

        var basePriceChanged = showtime.BasePrice != request.BasePrice;

        showtime.MovieId = request.MovieId;
        showtime.StartTime = request.StartTime;
        showtime.EndTime = endTime;
        showtime.BasePrice = request.BasePrice;

        if (basePriceChanged)
        {
            var seatsToReprice = await db
                .ShowtimeSeats.Include(ss => ss.Seat)
                    .ThenInclude(s => s.SeatType)
                .Where(ss => ss.ShowtimeId == showtimeId && ss.Status != ShowtimeSeatStatus.Booked)
                .ToListAsync(cancellationToken);
            foreach (var ss in seatsToReprice)
                ss.Price = request.BasePrice * (ss.Seat.SeatType?.PriceMultiplier ?? 1.0m);
        }

        showtime.Movie = movie;
        showtime.Room = await db.CinemaRooms.FirstAsync(
            r => r.RoomId == showtime.RoomId,
            cancellationToken
        );

        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "ShowtimeUpdated ShowtimeId {ShowtimeId} MovieId {MovieId} StartTime {StartTime}",
            showtime.ShowtimeId,
            showtime.MovieId,
            showtime.StartTime
        );

        return showtime.Adapt<AdminShowtimeDto>();
    }

    public async Task<DeleteShowtimeResponse> DeleteShowtimeAsync(
        int showtimeId,
        CancellationToken cancellationToken
    )
    {
        var showtime =
            await db.Showtimes.FirstOrDefaultAsync(
                s => s.ShowtimeId == showtimeId,
                cancellationToken
            ) ?? throw new NotFoundException("Không tìm thấy suất chiếu.");

        EnsureScheduled(showtime, "hủy");
        if (showtime.StartTime <= DateTime.UtcNow)
            throw new BusinessRuleException("Suất chiếu đã bắt đầu, không thể hủy.");

        var manualRefundCount = 0;
        var refundPaymentIds = new List<int>();
        var emailJobs = new List<(string ToEmail, string Subject, string Html)>();
        var bookingCount = 0;

        await using (var transaction = await db.Database.BeginTransactionAsync(cancellationToken))
        {
            showtime.Status = ShowtimeStatus.Cancelled;

            var bookings = await db
                .Bookings.Include(b => b.Payments)
                .Include(b => b.User)
                .Where(b =>
                    b.ShowtimeId == showtimeId
                    && (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed)
                )
                .ToListAsync(cancellationToken);
            bookingCount = bookings.Count;

            foreach (var booking in bookings)
            {
                var payment = booking
                    .Payments.Where(p => p.Status == PaymentStatus.Success)
                    .OrderByDescending(p => p.PaymentId)
                    .FirstOrDefault();

                var wasConfirmed = booking.Status == BookingStatus.Confirmed;
                booking.Status = BookingStatus.Cancelled;

                if (!wasConfirmed)
                    foreach (
                        var pending in booking.Payments.Where(p => p.Status == PaymentStatus.Pending)
                    )
                        pending.Status = PaymentStatus.Failed;

                if (wasConfirmed && booking.UserId is not null)
                    await bookingService.ReverseLoyaltyAsync(
                        booking.UserId.Value,
                        booking.FinalAmount,
                        cancellationToken
                    );

                if (payment is null)
                    continue;

                if (payment.PaymentMethod == PaymentMethod.Cash)
                {
                    manualRefundCount++;
                    if (booking.User is not null)
                        emailJobs.Add(
                            (
                                booking.User.Email,
                                $"Suất chiếu bạn đặt vé đã bị hủy — {booking.BookingCode}",
                                EmailTemplates.ShowtimeCancelledManualRefund(
                                    _brand,
                                    booking.CustomerName,
                                    booking.BookingCode,
                                    payment.Amount
                                )
                            )
                        );
                    continue;
                }

                refundPaymentIds.Add(payment.PaymentId);
            }

            await db
                .ShowtimeSeats.Where(ss =>
                    ss.ShowtimeId == showtimeId && ss.Status == ShowtimeSeatStatus.Held
                )
                .ExecuteUpdateAsync(
                    s =>
                        s.SetProperty(x => x.Status, ShowtimeSeatStatus.Available)
                            .SetProperty(x => x.HeldByUserId, (int?)null)
                            .SetProperty(x => x.HeldByBookingId, (int?)null)
                            .SetProperty(x => x.HeldUntil, (DateTime?)null),
                    cancellationToken
                );

            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        foreach (var job in emailJobs)
        {
            try
            {
                backgroundJobClient.Enqueue<SendEmailJob>(j =>
                    j.RunAsync(job.ToEmail, job.Subject, job.Html, CancellationToken.None)
                );
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "ShowtimeCancelEmailEnqueueFailed ShowtimeId {ShowtimeId}",
                    showtimeId
                );
            }
        }

        foreach (var paymentId in refundPaymentIds)
        {
            try
            {
                backgroundJobClient.Enqueue<ShowtimeRefundJob>(j =>
                    j.RunAsync(paymentId, null, CancellationToken.None)
                );
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "ShowtimeRefundEnqueueFailed ShowtimeId {ShowtimeId} PaymentId {PaymentId}",
                    showtimeId,
                    paymentId
                );
            }
        }

        logger.LogInformation(
            "ShowtimeCancelled ShowtimeId {ShowtimeId} BookingsCancelled {BookingCount} AutoRefundQueued {QueuedCount} ManualRefundNeeded {ManualCount}",
            showtimeId,
            bookingCount,
            refundPaymentIds.Count,
            manualRefundCount
        );

        return new DeleteShowtimeResponse(
            showtimeId,
            "Cancelled",
            manualRefundCount,
            refundPaymentIds.Count
        );
    }

    public async Task<BulkCreateShowtimesResponse> BulkCreateShowtimesAsync(
        BulkCreateShowtimesRequest request,
        CancellationToken cancellationToken
    )
    {
        var movie =
            await db.Movies.FirstOrDefaultAsync(
                m => m.MovieId == request.MovieId,
                cancellationToken
            )
            ?? throw new ValidationAppException(
                "movieId không tồn tại.",
                new Dictionary<string, string[]> { ["movieId"] = ["movieId không tồn tại."] }
            );
        var room =
            await db.CinemaRooms.FirstOrDefaultAsync(
                r => r.RoomId == request.RoomId,
                cancellationToken
            )
            ?? throw new ValidationAppException(
                "roomId không tồn tại.",
                new Dictionary<string, string[]> { ["roomId"] = ["roomId không tồn tại."] }
            );

        if (movie.Status == MovieStatus.Ended)
            throw new BusinessRuleException(
                "Phim đã ngừng chiếu (Ended), không thể tạo suất chiếu."
            );
        if (!room.IsActive)
            throw new BusinessRuleException("Phòng chiếu không hoạt động (inactive).");

        var seatCount = await db.Seats.CountAsync(
            s => s.RoomId == request.RoomId && s.IsActive && !s.IsAisle,
            cancellationToken
        );
        if (seatCount == 0)
            throw new BusinessRuleException("Phòng chiếu chưa được thiết lập sơ đồ ghế.");

        var dayOfWeekMap = new Dictionary<string, DayOfWeek>
        {
            ["Mon"] = DayOfWeek.Monday,
            ["Tue"] = DayOfWeek.Tuesday,
            ["Wed"] = DayOfWeek.Wednesday,
            ["Thu"] = DayOfWeek.Thursday,
            ["Fri"] = DayOfWeek.Friday,
            ["Sat"] = DayOfWeek.Saturday,
            ["Sun"] = DayOfWeek.Sunday,
        };

        var allowedDays = request.DaysOfWeek.Select(d => dayOfWeekMap[d]).ToHashSet();

        var drafts = new List<ShowtimeDraft>();
        for (
            var date = request.DateRange.From;
            date <= request.DateRange.To;
            date = date.AddDays(1)
        )
        {
            if (!allowedDays.Contains(date.DayOfWeek))
                continue;

            foreach (var timeStr in request.Times)
            {
                var timeOfDay = TimeOnly.ParseExact(timeStr, "HH:mm");
                drafts.Add(
                    new ShowtimeDraft(
                        request.MovieId,
                        request.RoomId,
                        VietnamTime.ToUtc(date, timeOfDay),
                        request.BasePrice,
                        request.AllowAfterHours
                    )
                );

                if (drafts.Count > MaxBatchSize)
                    throw new BusinessRuleException(
                        $"Tối đa {MaxBatchSize} suất chiếu mỗi lần tạo hàng loạt. Hãy thu hẹp khoảng ngày."
                    );
            }
        }

        var result = await CreateShowtimesBatchAsync(
            drafts,
            "Trùng khung giờ với suất chiếu đã có.",
            cancellationToken
        );

        logger.LogInformation(
            "BulkShowtimesCreated MovieId {MovieId} RoomId {RoomId} TotalCreated {Created} TotalSkipped {Skipped}",
            request.MovieId,
            request.RoomId,
            result.TotalCreated,
            result.TotalSkipped
        );

        return result;
    }

    public async Task<BulkDeleteShowtimesResponse> BulkDeleteShowtimesAsync(
        BulkDeleteShowtimesRequest request,
        CancellationToken cancellationToken
    )
    {
        var results = new List<BulkDeleteShowtimesResultItem>();

        foreach (var showtimeId in request.ShowtimeIds.Distinct())
        {
            try
            {
                await DeleteShowtimeAsync(showtimeId, cancellationToken);
                results.Add(new BulkDeleteShowtimesResultItem(showtimeId, true, null));
            }
            catch (NotFoundException)
            {
                results.Add(
                    new BulkDeleteShowtimesResultItem(
                        showtimeId,
                        false,
                        "Không tìm thấy suất chiếu."
                    )
                );
            }
            catch (AppException ex)
            {
                results.Add(new BulkDeleteShowtimesResultItem(showtimeId, false, ex.Message));
            }
            catch (Exception ex)
            {
                logger.LogWarning(
                    ex,
                    "BulkDeleteShowtime failed ShowtimeId {ShowtimeId}",
                    showtimeId
                );
                results.Add(
                    new BulkDeleteShowtimesResultItem(showtimeId, false, "Lỗi không xác định.")
                );
            }
        }

        var successCount = results.Count(r => r.Success);
        logger.LogInformation(
            "BulkShowtimesDeleted TotalSuccess {Success} TotalFailed {Failed}",
            successCount,
            results.Count - successCount
        );

        return new BulkDeleteShowtimesResponse(results, successCount, results.Count - successCount);
    }

    public async Task<int> CompleteEndedShowtimesAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var count = await db
            .Showtimes.Where(s => s.Status == ShowtimeStatus.Scheduled && s.EndTime < now)
            .ExecuteUpdateAsync(
                s => s.SetProperty(x => x.Status, ShowtimeStatus.Completed),
                cancellationToken
            );
        if (count > 0)
            logger.LogInformation("ShowtimesCompleted Count {Count}", count);

        return count;
    }

    public async Task<int> EndMoviesWithoutUpcomingShowtimesAsync(
        CancellationToken cancellationToken
    )
    {
        var now = DateTime.UtcNow;
        var cutoff = ShowtimeLifecycle.MovieEndCutoff(now);
        var count = await db
            .Movies.Where(m =>
                m.Status == MovieStatus.NowShowing
                && m.Showtimes.Any(s => s.Status != ShowtimeStatus.Cancelled)
                && !m.Showtimes.Any(s => s.Status == ShowtimeStatus.Scheduled && s.EndTime > cutoff)
            )
            .ExecuteUpdateAsync(
                m => m.SetProperty(x => x.Status, MovieStatus.Ended),
                cancellationToken
            );
        if (count > 0)
            logger.LogInformation("MoviesEnded Count {Count}", count);

        return count;
    }

    public async Task<AutoScheduleResponseDto> PreviewAutoScheduleAsync(
        AutoScheduleRequest request,
        CancellationToken cancellationToken
    )
    {
        var toDate = request.ToDate ?? request.Date;
        var dayCount = toDate.DayNumber - request.Date.DayNumber + 1;
        if (dayCount < 1)
            throw new ValidationAppException("toDate phải >= date.");
        if (dayCount > MaxAutoScheduleDays)
            throw new ValidationAppException(
                $"Khoảng ngày xếp lịch không được vượt quá {MaxAutoScheduleDays} ngày."
            );

        var movieIds = request.Movies.Select(m => m.MovieId).ToList();
        var movies = await db
            .Movies.Where(m => movieIds.Contains(m.MovieId))
            .Select(m => new
            {
                m.MovieId,
                m.Title,
                m.DurationMinutes,
                m.Status,
                m.AverageRating,
            })
            .ToListAsync(cancellationToken);

        var missing = movieIds.Except(movies.Select(m => m.MovieId)).ToList();
        if (missing.Count > 0)
            throw new ValidationAppException(
                $"movieId không tồn tại: {string.Join(", ", missing)}."
            );

        var ended = movies.FirstOrDefault(m => m.Status == MovieStatus.Ended);
        if (ended is not null)
            throw new BusinessRuleException(
                $"Phim \"{ended.Title}\" đã ngừng chiếu (Ended), không thể xếp lịch."
            );

        var roomQuery = db.CinemaRooms.Where(r =>
            r.IsActive && r.Seats.Any(s => s.IsActive && !s.IsAisle)
        );
        if (request.RoomIds is { Count: > 0 } roomIds)
            roomQuery = roomQuery.Where(r => roomIds.Contains(r.RoomId));
        var rooms = await roomQuery
            .Select(r => new
            {
                r.RoomId,
                r.RoomName,
                Seats = r.Seats.Count(s => s.IsActive && !s.IsAisle),
            })
            .ToListAsync(cancellationToken);
        if (rooms.Count == 0)
            throw new BusinessRuleException(
                "Không có phòng chiếu hoạt động và đã có sơ đồ ghế để xếp lịch."
            );

        var hours = await db
            .CinemaSettings.Where(s => s.Id == AppDbContextExtensions.CinemaSettingsId)
            .Select(s => new { s.OpeningTime, s.ClosingTime })
            .FirstOrDefaultAsync(cancellationToken);
        var (open, _) = OperatingHours.Window(request.Date, hours?.OpeningTime, hours?.ClosingTime);
        var horizon = VietnamTime.StartOfDayUtc(toDate).AddDays(2);
        var scheduledRoomIds = rooms.Select(r => r.RoomId).ToList();
        var busy = await db
            .Showtimes.Where(s =>
                s.Status == ShowtimeStatus.Scheduled
                && scheduledRoomIds.Contains(s.RoomId)
                && s.EndTime > open
                && s.StartTime < horizon
            )
            .Select(s => new SchedulerBusy(s.RoomId, s.StartTime, s.EndTime))
            .ToListAsync(cancellationToken);

        var since = DateTime.UtcNow.AddDays(-7);
        var tickets = await db
            .BookingTickets.Where(t =>
                t.Booking.Status == BookingStatus.Confirmed
                && t.Booking.CreatedAt >= since
                && movieIds.Contains(t.ShowtimeSeat.Showtime.MovieId)
            )
            .GroupBy(t => t.ShowtimeSeat.Showtime.MovieId)
            .Select(g => new { MovieId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.MovieId, x => x.Count, cancellationToken);
        var maxTickets = tickets.Values.DefaultIfEmpty(0).Max();

        var requested = request.Movies.ToDictionary(m => m.MovieId, m => m.Shows);
        var moviePrices = request.Movies.ToDictionary(
            m => m.MovieId,
            m => m.BasePrice ?? request.BasePrice
        );
        var heats = movies.ToDictionary(
            m => m.MovieId,
            m =>
                ShowtimeScheduler.Heat(
                    tickets.GetValueOrDefault(m.MovieId),
                    maxTickets,
                    m.AverageRating
                )
        );
        var schedulerRooms = rooms.Select(r => new SchedulerRoom(r.RoomId, r.Seats)).ToList();
        var schedulerMovies = movies
            .Select(m => new SchedulerMovie(
                m.MovieId,
                m.DurationMinutes,
                heats[m.MovieId],
                requested[m.MovieId]
            ))
            .ToList();

        var slots = new List<SchedulerSlot>();
        for (var day = request.Date; day <= toDate; day = day.AddDays(1))
        {
            var daySlots = ShowtimeScheduler.Build(
                day,
                hours?.OpeningTime,
                hours?.ClosingTime,
                request.AllowAfterHours,
                _bufferMinutes,
                schedulerRooms,
                schedulerMovies,
                busy
            );
            slots.AddRange(daySlots);
            busy.AddRange(daySlots.Select(x => new SchedulerBusy(x.RoomId, x.Start, x.End)));
        }
        slots = slots.OrderBy(x => x.Start).ThenBy(x => x.RoomId).ToList();

        var titles = movies.ToDictionary(m => m.MovieId, m => m.Title);
        var roomNames = rooms.ToDictionary(r => r.RoomId, r => r.RoomName);
        var items = slots
            .Select(s => new AutoScheduleItemDto(
                s.MovieId,
                titles[s.MovieId],
                s.RoomId,
                roomNames[s.RoomId],
                s.Start,
                s.End,
                moviePrices[s.MovieId]
            ))
            .ToList();
        var unplaced = movies
            .Select(m => new AutoScheduleUnplacedDto(
                m.MovieId,
                m.Title,
                requested[m.MovieId] * dayCount,
                slots.Count(s => s.MovieId == m.MovieId)
            ))
            .Where(u => u.Placed < u.Requested)
            .ToList();

        return new AutoScheduleResponseDto(
            request.Date.ToString("dd/MM/yyyy"),
            items,
            unplaced,
            dayCount > 1 ? toDate.ToString("dd/MM/yyyy") : null
        );
    }

    public async Task<BulkCreateShowtimesResponse> ApplyAutoScheduleAsync(
        ApplyAutoScheduleRequest request,
        CancellationToken cancellationToken
    )
    {
        var drafts = request
            .Items.OrderBy(i => i.StartTime)
            .Select(i => new ShowtimeDraft(
                i.MovieId,
                i.RoomId,
                i.StartTime,
                i.BasePrice,
                i.AllowAfterHours
            ))
            .ToList();

        var result = await CreateShowtimesBatchAsync(
            drafts,
            "Trùng khung giờ chiếu trong phòng này.",
            cancellationToken
        );

        logger.LogInformation(
            "AutoScheduleApplied TotalCreated {Created} TotalSkipped {Skipped}",
            result.TotalCreated,
            result.TotalSkipped
        );

        return result;
    }

    public async Task<BulkCreateShowtimesResponse> CloneShowtimesAsync(
        CloneShowtimesRequest request,
        CancellationToken cancellationToken
    )
    {
        var from = VietnamTime.StartOfDayUtc(request.SourceFrom);
        var to = VietnamTime.StartOfNextDayUtc(request.SourceTo);

        var query = db.Showtimes.Where(s =>
            s.Status != ShowtimeStatus.Cancelled && s.StartTime >= from && s.StartTime < to
        );
        if (request.RoomIds is { Count: > 0 } roomIds)
            query = query.Where(s => roomIds.Contains(s.RoomId));

        var sources = await query
            .OrderBy(s => s.StartTime)
            .ThenBy(s => s.RoomId)
            .Select(s => new
            {
                s.MovieId,
                s.RoomId,
                s.StartTime,
                s.BasePrice,
            })
            .Take(MaxBatchSize + 1)
            .ToListAsync(cancellationToken);

        if (sources.Count == 0)
            throw new BusinessRuleException(
                "Không có suất chiếu nào trong khoảng ngày nguồn để nhân bản."
            );
        if (sources.Count > MaxBatchSize)
            throw new BusinessRuleException(
                $"Tối đa {MaxBatchSize} suất chiếu mỗi lần nhân bản. Hãy thu hẹp khoảng ngày nguồn."
            );

        var drafts = sources
            .Select(x => new ShowtimeDraft(
                x.MovieId,
                x.RoomId,
                x.StartTime.AddDays(request.ShiftDays),
                x.BasePrice,
                request.AllowAfterHours
            ))
            .ToList();

        var result = await CreateShowtimesBatchAsync(
            drafts,
            "Trùng khung giờ với suất chiếu đã có.",
            cancellationToken
        );

        logger.LogInformation(
            "ShowtimesCloned Source {From}..{To} ShiftDays {Shift} TotalCreated {Created} TotalSkipped {Skipped}",
            request.SourceFrom,
            request.SourceTo,
            request.ShiftDays,
            result.TotalCreated,
            result.TotalSkipped
        );

        return result;
    }

    public async Task<List<SeatSuggestionDto>> GetSeatSuggestionsAsync(
        int showtimeId,
        int count,
        CancellationToken cancellationToken
    )
    {
        var roomCols =
            await db
                .Showtimes.Where(s => s.ShowtimeId == showtimeId)
                .Select(s => (int?)s.Room.Cols)
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Không tìm thấy suất chiếu.");

        var seats = await db.LoadSeatSlotsAsync(showtimeId, cancellationToken);
        var cols = Math.Max(roomCols, seats.Count > 0 ? seats.Max(s => s.Number) : 1);
        return SeatLayoutRules.Suggest(seats, cols, count);
    }

    public async Task<ShowtimeSeatsResponseDto> GetShowtimeSeatsAsync(
        int showtimeId,
        CancellationToken cancellationToken
    )
    {
        var showtime =
            await db
                .Showtimes.Include(s => s.Room)
                .Include(s => s.Movie)
                .FirstOrDefaultAsync(s => s.ShowtimeId == showtimeId, cancellationToken)
            ?? throw new NotFoundException("Không tìm thấy suất chiếu.");

        var seats = await db
            .ShowtimeSeats.Include(ss => ss.Seat)
                .ThenInclude(s => s.SeatType)
            .Where(ss => ss.ShowtimeId == showtimeId)
            .OrderBy(ss => ss.Seat.SeatRow)
            .ThenBy(ss => ss.Seat.SeatNumber)
            .Select(ss => new ShowtimeSeatItemDto(
                ss.ShowtimeSeatId,
                ss.Seat.SeatRow,
                ss.Seat.SeatNumber,
                ss.Seat.SeatTypeId,
                ss.Seat.SeatType != null ? ss.Seat.SeatType.Name : null,
                ss.Seat.CoupleId,
                ss.Status.ToString(),
                ss.Price
            ))
            .ToListAsync(cancellationToken);

        var aisles = await db
            .Seats.Where(s => s.RoomId == showtime.RoomId && s.IsAisle)
            .Select(s => new AislePositionDto(s.SeatRow, s.SeatNumber))
            .ToListAsync(cancellationToken);

        var maxSeatRowIdx =
            seats.Count > 0
                ? seats.Max(s => s.SeatRow.Length > 0 ? (s.SeatRow[0] - 'A' + 1) : 0)
                : 0;
        var maxAisleRowIdx =
            aisles.Count > 0
                ? aisles.Max(a => a.SeatRow.Length > 0 ? (a.SeatRow[0] - 'A' + 1) : 0)
                : 0;
        var actualMaxRow = Math.Max(maxSeatRowIdx, maxAisleRowIdx);
        var effectiveRows = actualMaxRow > 0 ? actualMaxRow : showtime.Room.Rows;

        var maxSeatCol = seats.Count > 0 ? seats.Max(s => s.SeatNumber) : 0;
        var maxAisleCol = aisles.Count > 0 ? aisles.Max(a => a.SeatNumber) : 0;
        var actualMaxCol = Math.Max(maxSeatCol, maxAisleCol);
        var effectiveCols = actualMaxCol > 0 ? actualMaxCol : showtime.Room.Cols;

        return new ShowtimeSeatsResponseDto(
            showtimeId,
            showtime.Room.RoomName,
            showtime.Movie.Title,
            showtime.StartTime,
            effectiveRows,
            effectiveCols,
            aisles,
            seats
        );
    }

    public async Task<WholeCinemaScheduleDto> GetWholeCinemaScheduleAsync(
        DateOnly date,
        CancellationToken cancellationToken
    )
    {
        var dayStart = VietnamTime.StartOfDayUtc(date);
        var dayEnd = VietnamTime.StartOfNextDayUtc(date);

        var showtimes = await db
            .Showtimes.Include(s => s.Room)
                .ThenInclude(r => r.ScreenType)
            .Include(s => s.Movie)
            .Where(s =>
                s.Status == ShowtimeStatus.Scheduled
                && s.StartTime >= dayStart
                && s.StartTime < dayEnd
            )
            .OrderBy(s => s.Movie.Title)
            .ThenBy(s => s.StartTime)
            .ToListAsync(cancellationToken);

        var showtimeIds = showtimes.Select(s => s.ShowtimeId).ToList();

        var seatCounts = await db
            .ShowtimeSeats.Where(ss => showtimeIds.Contains(ss.ShowtimeId))
            .GroupBy(ss => ss.ShowtimeId)
            .Select(g => new
            {
                ShowtimeId = g.Key,
                Total = g.Count(),
                Available = g.Count(x => x.Status == ShowtimeSeatStatus.Available),
            })
            .ToListAsync(cancellationToken);
        var seatCountLookup = seatCounts.ToDictionary(x => x.ShowtimeId, x => x);

        var movies = showtimes
            .GroupBy(s => s.Movie)
            .Select(g => new WholeCinemaMovieDto(
                g.Key.MovieId,
                g.Key.Title,
                g.Key.PosterUrl,
                g.Select(s =>
                    {
                        var counts = seatCountLookup.GetValueOrDefault(s.ShowtimeId);
                        var slot = s.Adapt<WholeCinemaShowtimeSlotDto>();
                        return slot with
                        {
                            AvailableSeats = counts?.Available ?? 0,
                            TotalSeats = counts?.Total ?? 0,
                        };
                    })
                    .ToList()
            ))
            .ToList();

        return new WholeCinemaScheduleDto(date.ToString("dd/MM/yyyy"), movies);
    }

    public async Task<ShowtimeCalendarResponseDto> GetShowtimesCalendarAsync(
        AdminShowtimeCalendarFilter filter,
        CancellationToken cancellationToken
    )
    {
        var rangeStart = VietnamTime.StartOfDayUtc(filter.From);
        var rangeEndExclusive = VietnamTime.StartOfNextDayUtc(filter.To);

        var query = db
            .Showtimes.Include(s => s.Movie)
            .Include(s => s.Room)
                .ThenInclude(r => r.ScreenType)
            .Where(s => s.StartTime >= rangeStart && s.StartTime < rangeEndExclusive);

        if (filter.RoomId is not null)
            query = query.Where(s => s.RoomId == filter.RoomId);
        if (filter.MovieId is not null)
            query = query.Where(s => s.MovieId == filter.MovieId);
        if (
            !string.IsNullOrWhiteSpace(filter.Status)
            && Enum.TryParse<ShowtimeStatus>(filter.Status, out var status)
        )
            query = query.Where(s => s.Status == status);

        var showtimes = await query
            .OrderBy(s => s.StartTime)
            .ThenBy(s => s.Room.RoomName)
            .ToListAsync(cancellationToken);

        var showtimeIds = showtimes.Select(s => s.ShowtimeId).ToList();

        var seatCounts = await db
            .ShowtimeSeats.Where(ss => showtimeIds.Contains(ss.ShowtimeId))
            .GroupBy(ss => ss.ShowtimeId)
            .Select(g => new
            {
                ShowtimeId = g.Key,
                Total = g.Count(),
                Available = g.Count(x => x.Status == ShowtimeSeatStatus.Available),
            })
            .ToListAsync(cancellationToken);
        var seatCountLookup = seatCounts.ToDictionary(x => x.ShowtimeId, x => x);

        var days = showtimes
            .GroupBy(s => VietnamTime.LocalDate(s.StartTime))
            .OrderBy(g => g.Key)
            .Select(g => new ShowtimeCalendarDayDto(
                g.Key.ToString("dd/MM/yyyy"),
                g.Select(s =>
                    {
                        var counts = seatCountLookup.GetValueOrDefault(s.ShowtimeId);
                        var item = s.Adapt<ShowtimeCalendarItemDto>();
                        return item with
                        {
                            AvailableSeats = counts?.Available ?? 0,
                            TotalSeats = counts?.Total ?? 0,
                        };
                    })
                    .ToList()
            ))
            .ToList();

        return new ShowtimeCalendarResponseDto(
            filter.From.ToString("dd/MM/yyyy"),
            filter.To.ToString("dd/MM/yyyy"),
            days
        );
    }

    private static void EnsureScheduled(Showtime showtime, string action)
    {
        if (showtime.Status != ShowtimeStatus.Scheduled)
            throw new BusinessRuleException(
                $"Chỉ có thể {action} suất chiếu đang ở trạng thái Scheduled (hiện tại: {showtime.Status})."
            );
    }

    private async Task<
        Dictionary<int, List<(int SeatId, decimal Multiplier)>>
    > LoadSeatTemplatesAsync(List<int> roomIds, CancellationToken cancellationToken)
    {
        var seats = await db
            .Seats.AsNoTracking()
            .Include(s => s.SeatType)
            .Where(s => roomIds.Contains(s.RoomId) && s.IsActive && !s.IsAisle)
            .ToListAsync(cancellationToken);

        return seats
            .GroupBy(s => s.RoomId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(s => (s.SeatId, s.SeatType?.PriceMultiplier ?? 1.0m)).ToList()
            );
    }

    private async Task<
        Dictionary<int, List<(DateTime Start, DateTime End)>>
    > LoadScheduledIntervalsAsync(
        List<int> roomIds,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken
    )
    {
        var rows = await db
            .Showtimes.Where(s =>
                roomIds.Contains(s.RoomId)
                && s.Status == ShowtimeStatus.Scheduled
                && s.EndTime > from
                && s.StartTime < to
            )
            .Select(s => new
            {
                s.RoomId,
                s.StartTime,
                s.EndTime,
            })
            .ToListAsync(cancellationToken);

        var result = roomIds.ToDictionary(
            id => id,
            _ => new List<(DateTime Start, DateTime End)>()
        );
        foreach (var row in rows)
            result[row.RoomId].Add((row.StartTime, row.EndTime));
        return result;
    }

    private async Task<BulkCreateShowtimesResponse> CreateShowtimesBatchAsync(
        IReadOnlyList<ShowtimeDraft> drafts,
        string overlapMessage,
        CancellationToken cancellationToken
    )
    {
        var created = new List<BulkCreatedShowtimeItem>();
        var skipped = new List<BulkSkippedShowtimeItem>();
        if (drafts.Count == 0)
            return new BulkCreateShowtimesResponse(created, skipped, 0, 0);

        var movieIds = drafts.Select(d => d.MovieId).Distinct().ToList();
        var roomIds = drafts.Select(d => d.RoomId).Distinct().ToList();

        var movies = await db
            .Movies.Where(m => movieIds.Contains(m.MovieId))
            .Select(m => new
            {
                m.MovieId,
                m.Status,
                m.DurationMinutes,
            })
            .ToDictionaryAsync(m => m.MovieId, cancellationToken);
        var rooms = await db
            .CinemaRooms.Where(r => roomIds.Contains(r.RoomId))
            .Select(r => new { r.RoomId, r.IsActive })
            .ToDictionaryAsync(r => r.RoomId, cancellationToken);
        var seatTemplates = await LoadSeatTemplatesAsync(roomIds, cancellationToken);
        var hours = await db
            .CinemaSettings.Where(s => s.Id == AppDbContextExtensions.CinemaSettingsId)
            .Select(s => new { s.OpeningTime, s.ClosingTime })
            .FirstOrDefaultAsync(cancellationToken);
        var intervals = await LoadScheduledIntervalsAsync(
            roomIds,
            drafts.Min(d => d.StartTime).AddDays(-1),
            drafts.Max(d => d.StartTime).AddDays(2),
            cancellationToken
        );

        var planned = new List<Showtime>();
        var now = DateTime.UtcNow;

        foreach (var draft in drafts)
        {
            string? reason = null;
            var endTime = draft.StartTime;
            List<(int SeatId, decimal Multiplier)>? seatTemplate = null;

            if (!movies.TryGetValue(draft.MovieId, out var movie))
                reason = "movieId không tồn tại.";
            else if (!rooms.TryGetValue(draft.RoomId, out var room))
                reason = "roomId không tồn tại.";
            else if (draft.StartTime <= now)
                reason = "Giờ bắt đầu suất chiếu phải ở tương lai.";
            else if (movie.Status == MovieStatus.Ended)
                reason = "Phim đã ngừng chiếu (Ended), không thể tạo suất chiếu.";
            else if (!room.IsActive)
                reason = "Phòng chiếu không hoạt động (inactive).";
            else if (
                !seatTemplates.TryGetValue(draft.RoomId, out seatTemplate)
                || seatTemplate.Count == 0
            )
                reason = "Phòng chiếu chưa được thiết lập sơ đồ ghế.";
            else
            {
                endTime = draft.StartTime.AddMinutes(movie.DurationMinutes + _bufferMinutes);
                reason =
                    OperatingHours.Violation(
                        draft.StartTime,
                        draft.StartTime.AddMinutes(movie.DurationMinutes),
                        hours?.OpeningTime,
                        hours?.ClosingTime,
                        draft.AllowAfterHours
                    )
                    ?? (
                        intervals[draft.RoomId]
                            .Any(i => i.End > draft.StartTime && i.Start < endTime)
                            ? overlapMessage
                            : null
                    );
            }

            if (reason is not null || seatTemplate is null)
            {
                skipped.Add(
                    new BulkSkippedShowtimeItem(
                        VietnamTime
                            .ToLocal(draft.StartTime)
                            .ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                        VietnamTime
                            .ToLocal(draft.StartTime)
                            .ToString("HH:mm", CultureInfo.InvariantCulture),
                        reason ?? "Suất chiếu không hợp lệ."
                    )
                );
                continue;
            }

            var showtime = new Showtime
            {
                MovieId = draft.MovieId,
                RoomId = draft.RoomId,
                StartTime = draft.StartTime,
                EndTime = endTime,
                BasePrice = draft.BasePrice,
                Status = ShowtimeStatus.Scheduled,
                CreatedAt = DateTime.UtcNow,
            };
            foreach (var (seatId, multiplier) in seatTemplate)
            {
                showtime.ShowtimeSeats.Add(
                    new ShowtimeSeat
                    {
                        SeatId = seatId,
                        Status = ShowtimeSeatStatus.Available,
                        Price = draft.BasePrice * multiplier,
                    }
                );
            }

            db.Showtimes.Add(showtime);
            planned.Add(showtime);
            intervals[draft.RoomId].Add((draft.StartTime, endTime));
        }

        if (planned.Count > 0)
            await db.SaveChangesAsync(cancellationToken);

        foreach (var showtime in planned)
            created.Add(
                new BulkCreatedShowtimeItem(
                    showtime.ShowtimeId,
                    showtime.StartTime,
                    showtime.ShowtimeSeats.Count
                )
            );

        return new BulkCreateShowtimesResponse(created, skipped, created.Count, skipped.Count);
    }

    private async Task<bool> HasOverlapAsync(
        int roomId,
        DateTime startTime,
        DateTime endTime,
        int? excludeShowtimeId,
        CancellationToken cancellationToken
    )
    {
        var overlapQuery = db.Showtimes.Where(s =>
            s.RoomId == roomId
            && s.Status == ShowtimeStatus.Scheduled
            && s.EndTime > startTime
            && s.StartTime < endTime
        );

        if (excludeShowtimeId is not null)
            overlapQuery = overlapQuery.Where(s => s.ShowtimeId != excludeShowtimeId);

        return await overlapQuery.AnyAsync(cancellationToken);
    }

    private async Task EnsureNoOverlapAsync(
        int roomId,
        DateTime startTime,
        DateTime endTime,
        int? excludeShowtimeId,
        CancellationToken cancellationToken
    )
    {
        if (await HasOverlapAsync(roomId, startTime, endTime, excludeShowtimeId, cancellationToken))
            throw new ConflictAppException("Trùng khung giờ chiếu trong phòng này.");
    }

    private async Task<int> GenerateShowtimeSeatsAsync(
        Showtime showtime,
        int roomId,
        decimal basePrice,
        CancellationToken cancellationToken
    )
    {
        var activeSeats = await db
            .Seats.Include(s => s.SeatType)
            .Where(s => s.RoomId == roomId && s.IsActive && !s.IsAisle)
            .ToListAsync(cancellationToken);

        foreach (var seat in activeSeats)
        {
            showtime.ShowtimeSeats.Add(
                new ShowtimeSeat
                {
                    SeatId = seat.SeatId,
                    Status = ShowtimeSeatStatus.Available,
                    Price = basePrice * (seat.SeatType?.PriceMultiplier ?? 1.0m),
                }
            );
        }

        return activeSeats.Count;
    }

    private static void EnsureStartsInFuture(DateTime startTime)
    {
        if (startTime <= DateTime.UtcNow)
            throw new BusinessRuleException("Giờ bắt đầu suất chiếu phải ở tương lai.");
    }

    private async Task ValidateOperatingHoursAsync(
        DateTime startTime,
        DateTime endTime,
        bool allowAfterHours,
        CancellationToken cancellationToken
    )
    {
        var hours = await db
            .CinemaSettings.Where(s => s.Id == AppDbContextExtensions.CinemaSettingsId)
            .Select(s => new { s.OpeningTime, s.ClosingTime })
            .FirstOrDefaultAsync(cancellationToken);

        var violation = OperatingHours.Violation(
            startTime,
            endTime,
            hours?.OpeningTime,
            hours?.ClosingTime,
            allowAfterHours
        );
        if (violation is not null)
            throw new BusinessRuleException(violation);
    }
}
