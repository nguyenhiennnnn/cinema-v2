using System.Globalization;
using Cinema.Application.DTOs.Showtimes;
using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cinema.Application.Services.Showtimes;

public class ShowtimeBatchService(
    IAppDbContext db,
    IShowtimeCancellationService showtimeCancellationService,
    ILogger<ShowtimeBatchService> logger
) : IShowtimeBatchService
{
    private const int MaxBatchSize = 2000;
    private const int ConflictLookbackDays = 1;

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
                Messages.Invalid_MovieIdNotFound,
                new Dictionary<string, string[]> { ["movieId"] = [Messages.Invalid_MovieIdNotFound] }
            );
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
            throw new BusinessRuleException(
                Messages.Rule_MovieEndedCannotCreateShowtime
            );
        if (!room.IsActive)
            throw new BusinessRuleException(Messages.Rule_RoomInactive);

        var seatCount = await db.Seats.CountAsync(
            s => s.RoomId == request.RoomId && s.IsActive && !s.IsAisle,
            cancellationToken
        );
        if (seatCount == 0)
            throw new BusinessRuleException(Messages.Rule_RoomHasNoSeatLayout);

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
                        BusinessTime.ToUtc(date, timeOfDay),
                        request.BasePrice,
                        request.AllowAfterHours
                    )
                );

                if (drafts.Count > MaxBatchSize)
                    throw new BusinessRuleException(
                        string.Format(Messages.Rule_BulkCreateBatchTooLarge, MaxBatchSize)
                    );
            }
        }

        var result = await CreateShowtimesBatchAsync(
            drafts,
            Messages.Conflict_ShowtimeOverlapExisting,
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
                await showtimeCancellationService.DeleteShowtimeAsync(showtimeId, cancellationToken);
                results.Add(new BulkDeleteShowtimesResultItem(showtimeId, true, null));
            }
            catch (NotFoundException)
            {
                results.Add(
                    new BulkDeleteShowtimesResultItem(
                        showtimeId,
                        false,
                        Messages.NotFound_Showtime
                    )
                );
            }
            catch (AppException ex)
            {
                results.Add(new BulkDeleteShowtimesResultItem(showtimeId, false, ex.Message));
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogWarning(
                    ex,
                    "BulkDeleteShowtime failed ShowtimeId {ShowtimeId}",
                    showtimeId
                );
                results.Add(
                    new BulkDeleteShowtimesResultItem(
                        showtimeId,
                        false,
                        Messages.Bulk_UnknownError
                    )
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

    public async Task<BulkCreateShowtimesResponse> CloneShowtimesAsync(
        CloneShowtimesRequest request,
        CancellationToken cancellationToken
    )
    {
        var from = BusinessTime.StartOfDayUtc(request.SourceFrom);
        var to = BusinessTime.StartOfNextDayUtc(request.SourceTo);

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
                Messages.Rule_NoShowtimesToClone
            );
        if (sources.Count > MaxBatchSize)
            throw new BusinessRuleException(
                string.Format(Messages.Rule_CloneBatchTooLarge, MaxBatchSize)
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
            Messages.Conflict_ShowtimeOverlapExisting,
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

    public async Task<BulkCreateShowtimesResponse> CreateShowtimesBatchAsync(
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
            .Select(s => new
            {
                s.OpeningTime,
                s.ClosingTime,
                s.RoomBufferMinutes,
            })
            .FirstOrDefaultAsync(cancellationToken);
        var bufferMinutes = hours?.RoomBufferMinutes ?? CinemaSettings.DefaultRoomBufferMinutes;
        var intervals = await LoadScheduledIntervalsAsync(
            roomIds,
            drafts.Min(d => d.StartTime).AddDays(-ConflictLookbackDays),
            drafts.Max(d => d.StartTime).AddDays(ShowtimeRules.ConflictLookaheadDays),
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
                reason = Messages.Invalid_MovieIdNotFound;
            else if (!rooms.TryGetValue(draft.RoomId, out var room))
                reason = Messages.Invalid_RoomIdNotFound;
            else if (draft.StartTime <= now)
                reason = Messages.Rule_ShowtimeStartMustBeFuture;
            else if (movie.Status == MovieStatus.Ended)
                reason = Messages.Rule_MovieEndedCannotCreateShowtime;
            else if (!room.IsActive)
                reason = Messages.Rule_RoomInactive;
            else if (
                !seatTemplates.TryGetValue(draft.RoomId, out seatTemplate)
                || seatTemplate.Count == 0
            )
                reason = Messages.Rule_RoomHasNoSeatLayout;
            else
            {
                endTime = draft.StartTime.AddMinutes(movie.DurationMinutes + bufferMinutes);
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
                        BusinessTime
                            .ToLocal(draft.StartTime)
                            .ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                        BusinessTime
                            .ToLocal(draft.StartTime)
                            .ToString("HH:mm", CultureInfo.InvariantCulture),
                        reason ?? Messages.Invalid_ShowtimeInvalid
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
}

