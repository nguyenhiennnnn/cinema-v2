import { formatCurrency, formatDateTime } from "@/lib/formatters";
import type { BookingDetailDto } from "../../types";

export const BookingResultTickets = ({ booking }: { booking: BookingDetailDto }) => {
  return (
    <div className="w-full overflow-hidden border border-border bg-card shadow-md">
      <div className="bg-primary px-5 py-4 text-primary-foreground">
        <p className="text-eyebrow text-primary-foreground/70">Vé điện tử</p>
        <p className="mt-0.5 font-heading text-xl leading-tight font-semibold tracking-tight uppercase">
          {booking.movie.title}
        </p>
        <p className="mt-1 text-sm text-primary-foreground/80">
          Phòng {booking.showtime.roomName} · {formatDateTime(booking.showtime.startTime)}
        </p>
      </div>

      <div className="ticket-notch perforation" />

      <div className="flex flex-col gap-5 p-5">
        {booking.tickets.length > 0 && (
          <div className="grid grid-cols-2 gap-3 sm:grid-cols-3">
            {booking.tickets.map((ticket) => (
              <div
                key={ticket.ticketId}
                className="flex flex-col items-center gap-2 border border-border bg-background p-3"
              >
                <img
                  src={ticket.qrCodeImage}
                  alt={`Vé ${ticket.seatRow}${ticket.seatNumber}`}
                  className="size-24"
                />
                <p className="font-mono text-xs font-semibold tabular">
                  Ghế {ticket.seatRow}
                  {ticket.seatNumber}
                </p>
              </div>
            ))}
          </div>
        )}

        <div className="flex flex-col gap-2 text-sm">
          <div className="flex items-center justify-between">
            <span className="text-muted-foreground">Ghế</span>
            <span className="font-mono font-medium tabular">
              {booking.seats.map((s) => `${s.seatRow}${s.seatNumber}`).join(", ")}
            </span>
          </div>
          {booking.combos.length > 0 && (
            <div className="flex items-center justify-between gap-4">
              <span className="shrink-0 text-muted-foreground">Combo</span>
              <span className="text-right font-medium">
                {booking.combos.map((c) => `${c.name} x${c.quantity}`).join(", ")}
              </span>
            </div>
          )}

          <div className="perforation my-1" />

          <div className="flex items-center justify-between">
            <span className="font-heading text-sm font-semibold tracking-wide text-foreground uppercase">
              Tổng thanh toán
            </span>
            <span className="font-heading text-2xl font-semibold tabular text-primary">
              {formatCurrency(booking.finalAmount)}
            </span>
          </div>
        </div>
      </div>
    </div>
  );
}
