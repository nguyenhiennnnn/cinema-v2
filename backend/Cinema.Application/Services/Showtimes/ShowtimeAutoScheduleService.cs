using Cinema.Application.DTOs.Showtimes;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cinema.Application.Services.Showtimes;

public class ShowtimeAutoScheduleService(
    IAppDbContext db,
    IShowtimeBatchService showtimeBatchService,
    ILogger<ShowtimeAutoScheduleService> logger,
    IOptions<SchedulerOptions>? schedulerOptions = null
) : IShowtimeAutoScheduleService
{
    private const int MaxAutoScheduleDays = 31;

    private readonly SchedulerOptions _scheduler = schedulerOptions?.Value ?? new();

    private static SchedulerOptions BuildSchedulerOptions(
        SchedulerOptions technical,
        CinemaSettings settings
    ) =>
        new()
        {
            DefaultWindowMinutes = technical.DefaultWindowMinutes,
            SpreadMinutes = technical.SpreadMinutes,
            FitWeight = technical.FitWeight,
            LoadWeight = technical.LoadWeight,
            StepMinutes = settings.SchedulerStepMinutes,
            MinSameMovieGapMinutes = settings.SchedulerMinSameMovieGapMinutes,
            HeatLookbackDays = settings.SchedulerHeatLookbackDays,
            HeatSalesWeight = settings.SchedulerHeatSalesWeight,
            HeatRatingWeight = settings.SchedulerHeatRatingWeight,
            MaxRating = settings.SchedulerMaxRating,
        };

    public async Task<AutoScheduleResponseDto> PreviewAutoScheduleAsync(
        AutoScheduleRequest request,
        CancellationToken cancellationToken
    )
    {
        var toDate = request.ToDate ?? request.Date;
        var dayCount = toDate.DayNumber - request.Date.DayNumber + 1;
        if (dayCount < 1)
            throw new ValidationAppException(Messages.Invalid_ToDateBeforeDate);
        if (dayCount > MaxAutoScheduleDays)
            throw new ValidationAppException(
                string.Format(Messages.Invalid_AutoScheduleRangeTooLong, MaxAutoScheduleDays)
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
                string.Format(Messages.Invalid_MovieIdsNotFound, string.Join(", ", missing))
            );

        var ended = movies.FirstOrDefault(m => m.Status == MovieStatus.Ended);
        if (ended is not null)
            throw new BusinessRuleException(
                string.Format(Messages.Rule_MovieEndedCannotSchedule, ended.Title)
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
            throw new BusinessRuleException(Messages.Rule_NoSchedulableRooms);

        var cinemaSettings = await db.LoadCinemaSettingsOrDefaultAsync(cancellationToken);
        var scheduler = BuildSchedulerOptions(_scheduler, cinemaSettings);
        var bufferMinutes = cinemaSettings.RoomBufferMinutes;
        var hours = new { cinemaSettings.OpeningTime, cinemaSettings.ClosingTime };
        var (open, _) = OperatingHours.Window(request.Date, hours.OpeningTime, hours.ClosingTime);
        var horizon = BusinessTime
            .StartOfDayUtc(toDate)
            .AddDays(ShowtimeRules.ConflictLookaheadDays);
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

        var since = DateTime.UtcNow.AddDays(-scheduler.HeatLookbackDays);
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
                    m.AverageRating,
                    scheduler
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
                hours.OpeningTime,
                hours.ClosingTime,
                request.AllowAfterHours,
                bufferMinutes,
                schedulerRooms,
                schedulerMovies,
                busy,
                scheduler
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

        var result = await showtimeBatchService.CreateShowtimesBatchAsync(
            drafts,
            Messages.Conflict_ShowtimeOverlapInRoom,
            cancellationToken
        );

        logger.LogInformation(
            "AutoScheduleApplied TotalCreated {Created} TotalSkipped {Skipped}",
            result.TotalCreated,
            result.TotalSkipped
        );

        return result;
    }
}
