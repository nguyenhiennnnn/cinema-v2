using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;

namespace Cinema.Application.Services.Bookings;

internal static class BookingAdminFilterExtensions
{
    public static IQueryable<Booking> ApplyAdminFilter(
        this IQueryable<Booking> query,
        AdminBookingListFilter filter
    )
    {
        if (
            !string.IsNullOrWhiteSpace(filter.Status)
            && Enum.TryParse<BookingStatus>(filter.Status, out var st)
        )
            query = query.Where(b => b.Status == st);
        if (
            !string.IsNullOrWhiteSpace(filter.Channel)
            && Enum.TryParse<BookingChannel>(filter.Channel, out var ch)
        )
            query = query.Where(b => b.Channel == ch);
        if (filter.FromDate is not null)
            query = query.Where(b =>
                b.CreatedAt >= BusinessTime.StartOfDayUtc(filter.FromDate.Value)
            );
        if (filter.ToDate is not null)
            query = query.Where(b =>
                b.CreatedAt < BusinessTime.StartOfNextDayUtc(filter.ToDate.Value)
            );
        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(b =>
                b.BookingCode.Contains(filter.Search)
                || b.CustomerName.Contains(filter.Search)
                || (b.CustomerPhone != null && b.CustomerPhone.Contains(filter.Search))
            );

        return query;
    }
}
