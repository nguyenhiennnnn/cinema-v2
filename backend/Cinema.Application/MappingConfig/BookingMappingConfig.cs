using Cinema.Application.DTOs.Bookings;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Mapster;

namespace Cinema.Application.MappingConfig;

public class BookingMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<Booking, BookingListItemDto>()
            .Map(dest => dest.MovieTitle, src => src.Showtime.Movie.Title)
            .Map(dest => dest.StartTime, src => src.Showtime.StartTime)
            .Map(dest => dest.Status, src => src.Status.ToDtoStatus());

        config
            .NewConfig<Booking, AdminBookingListItemDto>()
            .Map(dest => dest.Status, src => src.Status.ToDtoStatus())
            .Map(dest => dest.Channel, src => src.Channel.ToDtoChannel());

        // ── Nested brief DTOs ─────────────────────────────────────────────────
        config
            .NewConfig<Booking, BookingMovieBriefDto>()
            .Map(dest => dest.MovieId, src => src.Showtime.Movie.MovieId)
            .Map(dest => dest.Title, src => src.Showtime.Movie.Title)
            .Map(dest => dest.PosterUrl, src => src.Showtime.Movie.PosterUrl);

        config
            .NewConfig<Booking, BookingShowtimeBriefDto>()
            .Map(dest => dest.ShowtimeId, src => src.Showtime.ShowtimeId)
            .Map(dest => dest.RoomName, src => src.Showtime.Room.RoomName)
            .Map(dest => dest.StartTime, src => src.Showtime.StartTime);

        config
            .NewConfig<Employee, BookingEmployeeBriefDto>()
            .Map(dest => dest.FullName, src => src.User.FullName);

        // ── Hold responses ────────────────────────────────────────────────────
        config
            .NewConfig<Booking, HoldBookingResponse>()
            .Map(dest => dest.Status, _ => BookingStatus.Pending.ToDtoStatus());

        config
            .NewConfig<Booking, CounterHoldResponse>()
            .Map(dest => dest.Status, _ => BookingStatus.Pending.ToDtoStatus())
            .Map(dest => dest.Channel, _ => "Counter")
            .Map(dest => dest.CreatedByEmployeeId, src => src.CreatedByEmployeeId);

        // ── Seat DTOs ─────────────────────────────────────────────────────────
        config
            .NewConfig<ShowtimeSeat, BookingSeatDto>()
            .Map(dest => dest.SeatRow, src => src.Seat.SeatRow)
            .Map(dest => dest.SeatNumber, src => src.Seat.SeatNumber)
            .Map(dest => dest.Price, src => src.Price);

        config
            .NewConfig<BookingTicket, BookingSeatDto>()
            .Map(dest => dest.SeatRow, src => src.ShowtimeSeat.Seat.SeatRow)
            .Map(dest => dest.SeatNumber, src => src.ShowtimeSeat.Seat.SeatNumber)
            .Map(dest => dest.Price, src => src.Price);

        // ── Combo DTO ─────────────────────────────────────────────────────────
        config
            .NewConfig<BookingCombo, BookingComboItemDto>()
            .Map(dest => dest.Name, src => src.Combo.Name)
            .Map(dest => dest.Quantity, src => src.Quantity)
            .Map(dest => dest.UnitPrice, src => src.UnitPrice);

        // ── Payment DTO ───────────────────────────────────────────────────────
        config
            .NewConfig<Payment, BookingPaymentDto>()
            .Map(dest => dest.PaymentMethod, src => src.PaymentMethod.ToDtoMethod())
            .Map(dest => dest.Status, src => src.Status.ToDtoStatus());

        // ── Ticket DTOs ───────────────────────────────────────────────────────
        config
            .NewConfig<BookingTicket, BookingTicketDto>()
            .Map(dest => dest.SeatRow, src => src.ShowtimeSeat.Seat.SeatRow)
            .Map(dest => dest.SeatNumber, src => src.ShowtimeSeat.Seat.SeatNumber)
            .Map(
                dest => dest.QrCodeImage,
                src => Utils.QrCodeHelper.GenerateDataUri(src.QrCode ?? string.Empty)
            );

        config
            .NewConfig<BookingTicket, CheckinResponse>()
            .Map(dest => dest.TicketId, src => src.TicketId)
            .Map(dest => dest.BookingId, src => src.BookingId)
            .Map(dest => dest.SeatRow, src => src.ShowtimeSeat.Seat.SeatRow)
            .Map(dest => dest.SeatNumber, src => src.ShowtimeSeat.Seat.SeatNumber)
            .Map(dest => dest.MovieTitle, src => src.ShowtimeSeat.Showtime.Movie.Title)
            .Map(dest => dest.StartTime, src => src.ShowtimeSeat.Showtime.StartTime)
            .Map(dest => dest.CheckedInAt, src => src.CheckedInAt!.Value);
    }
}
