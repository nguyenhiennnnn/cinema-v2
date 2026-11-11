using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Services.Showtimes;

public static class ShowtimeRules
{
    public const int ConflictLookaheadDays = 2;

    public static void EnsureScheduled(Showtime showtime, string action)
    {
        if (showtime.Status != ShowtimeStatus.Scheduled)
            throw new BusinessRuleException(
                string.Format(Messages.Rule_ShowtimeMustBeScheduled, action, showtime.Status)
            );
    }

    public static void EnsureStartsInFuture(DateTime startTime)
    {
        if (startTime <= DateTime.UtcNow)
            throw new BusinessRuleException(Messages.Rule_ShowtimeStartMustBeFuture);
    }

    public static async Task<bool> HasOverlapAsync(
        this IAppDbContext db,
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

    public static async Task EnsureNoOverlapAsync(
        this IAppDbContext db,
        int roomId,
        DateTime startTime,
        DateTime endTime,
        int? excludeShowtimeId,
        CancellationToken cancellationToken
    )
    {
        if (await db.HasOverlapAsync(roomId, startTime, endTime, excludeShowtimeId, cancellationToken))
            throw new ConflictAppException(Messages.Conflict_ShowtimeOverlapInRoom);
    }

    public static async Task ValidateOperatingHoursAsync(
        this IAppDbContext db,
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

    public static async Task<int> GenerateShowtimeSeatsAsync(
        this IAppDbContext db,
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
}

