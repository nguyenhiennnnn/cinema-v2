import { PerfoTicket } from "@/components/common/perfo-ticket";
import { formatCurrency, formatDate, formatTime } from "@/lib/formatters";
import type { BookingDetailDto } from "../../types";

export const BookingResultTickets = ({ booking }: { booking: BookingDetailDto }) => {
  const seats = booking.seats.map((s) => `${s.seatRow}${s.seatNumber}`).join(", ");
  const showtime = `${formatTime(booking.showtime.startTime)}, ${formatDate(booking.showtime.startTime)}`;
  const room = booking.showtime.roomName;
  const total = formatCurrency(booking.finalAmount);

  return (
    <div className="flex w-full flex-col gap-4">
      {booking.tickets.length > 0
        ? booking.tickets.map((ticket) => (
            <PerfoTicket
              key={ticket.ticketId}
              movieTitle={booking.movie.title}
              showtime={showtime}
              room={room}
              seat={`${ticket.seatRow}${ticket.seatNumber}`}
              totalAmount={total}
              ticketCode={`${booking.bookingCode}-${ticket.seatRow}${ticket.seatNumber}`}
              qrValue={ticket.qrCodeImage}
              isCheckedIn={ticket.isCheckedIn}
            />
          ))
        : /* Chưa generate ticket — hiển thị tổng hợp */
          (
            <PerfoTicket
              movieTitle={booking.movie.title}
              showtime={showtime}
              room={room}
              seat={seats}
              totalAmount={total}
              ticketCode={booking.bookingCode}
              qrValue={booking.bookingCode}
            />
          )}
    </div>
  );
};
