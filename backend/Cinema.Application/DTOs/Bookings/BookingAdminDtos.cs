namespace Cinema.Application.DTOs.Bookings;

public record BookingEmployeeBriefDto(int EmployeeId, string FullName);

public record AdminBookingListItemDto(
    int BookingId,
    string BookingCode,
    string CustomerName,
    string? GuestPhone,
    decimal FinalAmount,
    string Status,
    string Channel,
    DateTime CreatedAt
);

public record AdminBookingDetailDto(
    int BookingId,
    string BookingCode,
    string Status,
    BookingMovieBriefDto Movie,
    BookingShowtimeBriefDto Showtime,
    List<BookingSeatDto> Seats,
    List<BookingComboItemDto> Combos,
    decimal TotalAmount,
    decimal DiscountAmount,
    decimal FinalAmount,
    DateTime CreatedAt,
    string Channel,
    BookingEmployeeBriefDto? CreatedByEmployee,
    List<BookingTicketDto> Tickets,
    List<BookingPaymentDto> Payments
);
