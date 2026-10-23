import { cn } from "@/lib/utils";
import { formatShowtimeHour, formatCurrency } from "@/lib/formatters";
import { ShowtimeStatus } from "@/types/enum";
import type { ShowtimeCalendarItemDto } from "../../types";

const MINUTES_IN_DAY = 24 * 60;

const statusStyles: Record<string, string> = {
  [ShowtimeStatus.Scheduled]:
    "bg-success/15 border-success/40 text-success hover:bg-success/25",
  [ShowtimeStatus.Cancelled]:
    "bg-destructive/15 border-destructive/40 text-destructive hover:bg-destructive/25",
  [ShowtimeStatus.Completed]:
    "bg-muted border-border text-muted-foreground hover:bg-muted/70",
};

function minutesOfDay(iso: string): number {
  const d = new Date(iso);
  return d.getHours() * 60 + d.getMinutes();
}

interface ShowtimeCalendarItemProps {
  showtime: ShowtimeCalendarItemDto;
  onClick?: (showtime: ShowtimeCalendarItemDto) => void;
}

export const ShowtimeCalendarItem = ({
  showtime,
  onClick,
}: ShowtimeCalendarItemProps) => {
  const start = minutesOfDay(showtime.startTime);
  const rawEnd = minutesOfDay(showtime.endTime);
  const end = rawEnd > start ? rawEnd : MINUTES_IN_DAY;

  const leftPercent = (start / MINUTES_IN_DAY) * 100;
  const widthPercent = Math.max(((end - start) / MINUTES_IN_DAY) * 100, 3);

  const occupancyLabel = `${showtime.totalSeats - showtime.availableSeats}/${showtime.totalSeats} ghế`;

  return (
    <button
      type="button"
      onClick={() => onClick?.(showtime)}
      title={`${showtime.movieTitle} (${formatShowtimeHour(showtime.startTime)} - ${formatShowtimeHour(
        showtime.endTime
      )}), ${occupancyLabel}, ${formatCurrency(showtime.basePrice)}`}
      style={{ left: `${leftPercent}%`, width: `${widthPercent}%` }}
      className={cn(
        "absolute top-1 flex h-[calc(100%-0.5rem)] flex-col justify-center overflow-hidden cut-xs px-2 py-1 text-left text-xs leading-tight transition-colors",
        statusStyles[showtime.status] ?? statusStyles[ShowtimeStatus.Scheduled],
      )}
    >
      <span className="truncate font-medium">{showtime.movieTitle}</span>
      <span className="truncate opacity-80">
        {formatShowtimeHour(showtime.startTime)}, {occupancyLabel}
      </span>
    </button>
  );
};