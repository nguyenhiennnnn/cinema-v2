import { bookingResultStatusConfig } from "@/components/common/status-badge";
import type { BookingStatus as BookingStatusType } from "@/types/enum";
import { cn } from "@/lib/utils";

interface BookingResultStatusProps {
  status: BookingStatusType;
  bookingCode: string;
}

export const BookingResultStatus = ({ status, bookingCode }: BookingResultStatusProps) => {
  const config = bookingResultStatusConfig[status];
  const Icon = config.icon;

  return (
    <div className="flex flex-col items-center gap-2 text-center">
      <Icon className={cn("size-12", config.className)} />
      <h1 className="font-heading text-2xl font-semibold tracking-tight text-foreground">
        {config.label}
      </h1>
      <p className="max-w-md text-sm text-muted-foreground">{config.description}</p>
      <p className="text-xs tracking-wide text-muted-foreground tabular">
        Mã đặt vé · {bookingCode}
      </p>
    </div>
  );
};