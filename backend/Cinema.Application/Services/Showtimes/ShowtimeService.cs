using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Showtimes;
using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cinema.Application.Services.Showtimes;

public class ShowtimeService(IAppDbContext db, ILogger<ShowtimeService> logger) : IShowtimeService
{
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
            var dayStart = BusinessTime.StartOfDayUtc(filter.Date.Value);
            var dayEnd = BusinessTime.StartOfNextDayUtc(filter.Date.Value);
            query = query.Where(s => s.StartTime >= dayStart && s.StartTime < dayEnd);
        }

        if (
            !string.IsNullOrWhiteSpace(filter.Status)
            && Enum.TryParse<ShowtimeStatus>(filter.Status, out var status)
        )
            query = query.Where(s => s.Status == status);

        query = query.OrderByDescending(s => s.StartTime).ThenByDescending(s => s.ShowtimeId);

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
            ) ?? throw new ValidationAppException(Messages.Invalid_MovieIdNotFound);
        var room =
            await db.CinemaRooms.FirstOrDefaultAsync(
                r => r.RoomId == request.RoomId,
                cancellationToken
            )
            ?? throw new ValidationAppException(
                Messages.Invalid_RoomIdNotFound,
                new Dictionary<string, string[]> { ["roomId"] = [Messages.Invalid_RoomIdNotFound] }
            );

        if (movie.Status == MovieStatus.Ended)
            throw new BusinessRuleException(Messages.Rule_MovieEndedCannotCreateShowtime);
        if (!room.IsActive)
            throw new BusinessRuleException(Messages.Rule_RoomInactive);

        var seatCount = await db.Seats.CountAsync(
            s => s.RoomId == request.RoomId && s.IsActive && !s.IsAisle,
            cancellationToken
        );
        if (seatCount == 0)
            throw new BusinessRuleException(Messages.Rule_RoomHasNoSeatLayout);

        ShowtimeRules.EnsureStartsInFuture(request.StartTime);

        var bufferMinutes = (
            await db.LoadCinemaSettingsOrDefaultAsync(cancellationToken)
        ).RoomBufferMinutes;
        var endTime = request.StartTime.AddMinutes(movie.DurationMinutes + bufferMinutes);

        await db.ValidateOperatingHoursAsync(
            request.StartTime,
            request.StartTime.AddMinutes(movie.DurationMinutes),
            request.AllowAfterHours,
            cancellationToken
        );

        await db.EnsureNoOverlapAsync(
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

        var seatsGenerated = await db.GenerateShowtimeSeatsAsync(
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
            ) ?? throw new NotFoundException(Messages.NotFound_Showtime);

        ShowtimeRules.EnsureScheduled(showtime, Messages.Common_ActionEdit);

        if (request.RoomId != showtime.RoomId)
            throw new BusinessRuleException(Messages.Rule_CannotChangeShowtimeRoom);

        var hasConfirmedBooking = await db.Bookings.AnyAsync(
            b => b.ShowtimeId == showtimeId && b.Status == BookingStatus.Confirmed,
            cancellationToken
        );
        if (hasConfirmedBooking)
            throw new ConflictAppException(Messages.Conflict_ShowtimeHasConfirmedBookings);

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
            throw new ConflictAppException(Messages.Conflict_ShowtimeHasPendingBookings);

        var movie =
            await db.Movies.FirstOrDefaultAsync(
                m => m.MovieId == request.MovieId,
                cancellationToken
            )
            ?? throw new ValidationAppException(
                Messages.Invalid_MovieIdNotFound,
                new Dictionary<string, string[]>
                {
                    ["movieId"] = [Messages.Invalid_MovieIdNotFound],
                }
            );
        if (movie.Status == MovieStatus.Ended)
            throw new BusinessRuleException(Messages.Rule_MovieEndedCannotUpdateShowtime);

        ShowtimeRules.EnsureStartsInFuture(request.StartTime);

        var bufferMinutes = (
            await db.LoadCinemaSettingsOrDefaultAsync(cancellationToken)
        ).RoomBufferMinutes;
        var endTime = request.StartTime.AddMinutes(movie.DurationMinutes + bufferMinutes);
        if (
            showtime.StartTime != request.StartTime
            || showtime.MovieId != request.MovieId
            || request.AllowAfterHours
        )
            await db.ValidateOperatingHoursAsync(
                request.StartTime,
                request.StartTime.AddMinutes(movie.DurationMinutes),
                request.AllowAfterHours,
                cancellationToken
            );
        await db.EnsureNoOverlapAsync(
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
}
