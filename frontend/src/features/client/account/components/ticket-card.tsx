import { Link } from "react-router";
import { QrCode, Ticket as TicketIcon } from "lucide-react";
import { Button } from "@/components/ui/button";
import { BookingStatusBadge } from "@/components/common/status-badge";
import { formatCurrency, formatDateTime } from "@/lib/formatters";
import { BookingStatus } from "@/types/enum";
import type { BookingListItemDto } from "@/features/client/bookings/types";
import { useNow } from "@/hooks/use-now";
import { useCinemaSettings } from "@/features/client/settings/hooks/use-settings";

interface TicketCardProps {
  booking: BookingListItemDto;
  onCancel?: () => void;
}

export const TicketCard = ({ booking, onCancel }: TicketCardProps) => {
  const now = useNow();
  const { data: settings } = useCinemaSettings();
  const cancelDeadlineHours = settings?.cancelDeadlineHours ?? 2;
  const isRefundable = settings ? settings.cancelRefundPercent > 0 : true;

  const canCancel =
    booking.status === BookingStatus.Confirmed &&
    isRefundable &&
    new Date(booking.startTime).getTime() - now > cancelDeadlineHours * 60 * 60 * 1000;

  return (
    <div className="flex flex-col gap-4 bg-card p-4 sm:flex-row sm:items-center sm:justify-between">
      <div className="flex items-center gap-3">
        <div className="flex size-12 shrink-0 items-center justify-center bg-muted">
          <TicketIcon className="size-5" />
        </div>
        <div>
          <p className="font-heading text-sm font-semibold text-foreground">{booking.movieTitle}</p>
          <p className="mt-0.5 text-xs text-muted-foreground">{formatDateTime(booking.startTime)}</p>
          <p className="mt-0.5 t-code text-xs text-muted-foreground">{booking.bookingCode}</p>
        </div>
      </div>

      <div className="flex items-center gap-3">
        <div className="text-right">
          <BookingStatusBadge status={booking.status} />
          <p className="mt-1 text-sm font-semibold text-primary tabular-nums">{formatCurrency(booking.finalAmount)}</p>
        </div>
        <div className="flex flex-col gap-2">
          <Link
            to={`/account/tickets/${booking.bookingId}`}
            className="inline-flex items-center justify-center gap-1.5 border border-border bg-muted px-2 py-0.5 text-xs font-medium"
          >
            <QrCode className="size-3.5" />
            Chi tiết & QR
          </Link>
          {canCancel && onCancel && (
            <Button variant="destructive" size="sm" onClick={onCancel}>
              Hủy vé
            </Button>
          )}
        </div>
      </div>
    </div>
  );
}