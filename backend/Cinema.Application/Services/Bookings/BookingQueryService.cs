using Cinema.Application.DTOs.Bookings;
using Cinema.Application.DTOs.Common;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Cinema.Application.Services.Bookings;

public class BookingQueryService(IAppDbContext db, IOptions<BookingOptions> bookingOptions)
    : IBookingQueryService
{
    private readonly BookingOptions _options = bookingOptions.Value;

    public async Task<CursorPageResult<BookingListItemDto>> GetMyBookingsAsync(
        int userId,
        string? status,
        string? cursor,
        int limit,
        CancellationToken cancellationToken
    )
    {
        var query = db
            .Bookings.Include(b => b.Showtime)
                .ThenInclude(s => s.Movie)
            .Where(b => b.UserId == userId);

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<BookingStatus>(status, out var st))
            query = query.Where(b => b.Status == st);

        var cursorId = CursorCodec.Decode(cursor);
        if (cursorId is not null)
            query = query.Where(b => b.BookingId < cursorId);
        query = query.OrderByDescending(b => b.BookingId);

        var rows = await query
            .Take(limit + 1)
            .ProjectToType<BookingListItemDto>()
            .ToListAsync(cancellationToken);

        var hasMore = rows.Count > limit;
        var items = hasMore ? rows.Take(limit).ToList() : rows;
        var nextCursor = hasMore ? CursorCodec.Encode(items[^1].BookingId) : null;

        return new CursorPageResult<BookingListItemDto>
        {
            Items = items,
            NextCursor = nextCursor,
            HasMore = hasMore,
        };
    }

    public async Task<BookingDetailDto> GetBookingDetailAsync(
        int userId,
        int bookingId,
        CancellationToken cancellationToken
    )
    {
        var booking = await LoadBookingOrThrowAsync(bookingId, cancellationToken);
        if (booking.UserId != userId)
            throw new ForbiddenAppException(Messages.Forbidden_NotBookingOwner);

        var seats = await GetBookingSeatsAsync(booking, cancellationToken);
        var combos = await GetBookingCombosAsync(bookingId, cancellationToken);
        var tickets = await GetBookingTicketsAsync(booking, cancellationToken);
        var payments = await GetBookingPaymentsAsync(bookingId, cancellationToken);

        return new BookingDetailDto(
            booking.BookingId,
            booking.BookingCode,
            booking.Status.ToDtoStatus(),
            booking.Adapt<BookingMovieBriefDto>(),
            booking.Adapt<BookingShowtimeBriefDto>(),
            seats,
            combos,
            booking.TotalAmount,
            booking.DiscountAmount,
            booking.FinalAmount,
            booking.CreatedAt,
            tickets,
            payments,
            booking.Status == BookingStatus.Pending ? booking.ExpiresAt : null
        );
    }

    public async Task<OffsetPageResult<AdminBookingListItemDto>> GetBookingsAdminAsync(
        AdminBookingListFilter filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        page = Math.Max(1, page);
        pageSize =
            pageSize <= 0 ? _options.DefaultPageSize : Math.Min(pageSize, _options.MaxPageSize);

        var query = db.Bookings.AsQueryable().ApplyAdminFilter(filter);

        query = query.OrderByDescending(b => b.CreatedAt);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<AdminBookingListItemDto>()
            .ToListAsync(cancellationToken);

        return OffsetPageResult<AdminBookingListItemDto>.Create(items, page, pageSize, total);
    }

    public async Task<AdminBookingDetailDto> GetBookingDetailAdminAsync(
        int bookingId,
        CancellationToken cancellationToken
    )
    {
        var booking = await LoadBookingOrThrowAsync(bookingId, cancellationToken);

        var seats = await GetBookingSeatsAsync(booking, cancellationToken);
        var combos = await GetBookingCombosAsync(bookingId, cancellationToken);
        var tickets = await GetBookingTicketsAsync(booking, cancellationToken);
        var payments = await GetBookingPaymentsAsync(bookingId, cancellationToken);

        BookingEmployeeBriefDto? employeeDto = null;
        if (booking.CreatedByEmployeeId is not null)
        {
            var employee = await db
                .Employees.Include(e => e.User)
                .FirstOrDefaultAsync(
                    e => e.EmployeeId == booking.CreatedByEmployeeId,
                    cancellationToken
                );
            if (employee is not null)
                employeeDto = employee.Adapt<BookingEmployeeBriefDto>();
        }

        return new AdminBookingDetailDto(
            booking.BookingId,
            booking.BookingCode,
            booking.Status.ToDtoStatus(),
            booking.Adapt<BookingMovieBriefDto>(),
            booking.Adapt<BookingShowtimeBriefDto>(),
            seats,
            combos,
            booking.TotalAmount,
            booking.DiscountAmount,
            booking.FinalAmount,
            booking.CreatedAt,
            booking.Channel.ToDtoChannel(),
            employeeDto,
            tickets,
            payments
        );
    }

    private async Task<List<BookingSeatDto>> GetBookingSeatsAsync(
        Booking booking,
        CancellationToken cancellationToken
    )
    {
        if (booking.Status == BookingStatus.Pending)
        {
            var holderUserId = await BookingSeatHold.ResolveHolderUserIdAsync(
                db,
                booking,
                cancellationToken
            );
            return await db
                .ShowtimeSeats.HeldFor(booking, holderUserId)
                .ProjectToType<BookingSeatDto>()
                .ToListAsync(cancellationToken);
        }

        return await db
            .BookingTickets.Where(t => t.BookingId == booking.BookingId)
            .ProjectToType<BookingSeatDto>()
            .ToListAsync(cancellationToken);
    }

    private async Task<List<BookingTicketDto>> GetBookingTicketsAsync(
        Booking booking,
        CancellationToken cancellationToken
    )
    {
        if (booking.Status != BookingStatus.Confirmed)
            return [];

        return await db
            .BookingTickets.Where(t => t.BookingId == booking.BookingId)
            .OrderBy(t => t.ShowtimeSeat.Seat.SeatRow)
            .ThenBy(t => t.ShowtimeSeat.Seat.SeatNumber)
            .ProjectToType<BookingTicketDto>()
            .ToListAsync(cancellationToken);
    }

    private async Task<List<BookingComboItemDto>> GetBookingCombosAsync(
        int bookingId,
        CancellationToken cancellationToken
    ) =>
        await db
            .BookingCombos.Where(bc => bc.BookingId == bookingId)
            .ProjectToType<BookingComboItemDto>()
            .ToListAsync(cancellationToken);

    private async Task<List<BookingPaymentDto>> GetBookingPaymentsAsync(
        int bookingId,
        CancellationToken cancellationToken
    ) =>
        await db
            .Payments.Where(p => p.BookingId == bookingId)
            .OrderByDescending(p => p.CreatedAt)
            .ProjectToType<BookingPaymentDto>()
            .ToListAsync(cancellationToken);

    private async Task<Booking> LoadBookingOrThrowAsync(
        int bookingId,
        CancellationToken cancellationToken
    ) =>
        await db
            .Bookings.Include(b => b.Showtime)
                .ThenInclude(s => s.Movie)
            .Include(b => b.Showtime)
                .ThenInclude(s => s.Room)
            .FirstOrDefaultAsync(b => b.BookingId == bookingId, cancellationToken)
        ?? throw new NotFoundException(Messages.NotFound_Booking);
}
