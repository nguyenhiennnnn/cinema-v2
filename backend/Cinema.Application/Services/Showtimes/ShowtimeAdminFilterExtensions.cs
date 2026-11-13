using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;

namespace Cinema.Application.Services.Showtimes;

internal static class ShowtimeAdminFilterExtensions
{
    public static IQueryable<Showtime> ApplyAdminFilter(
        this IQueryable<Showtime> query,
        AdminShowtimeListFilter filter
    )
    {
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

        return query;
    }
}
