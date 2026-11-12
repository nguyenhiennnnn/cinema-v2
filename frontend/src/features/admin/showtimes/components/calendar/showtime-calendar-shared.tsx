import {
  CalendarCheck,
  CheckCircle2,
  XCircle,
  type LucideIcon,
} from "lucide-react";

import { formatShowtimeTime } from "@/lib/cinema-format";
import { formatCurrency } from "@/lib/formatters";
import { cn } from "@/lib/utils";
import { ShowtimeStatus } from "@/types/enum";
import type { ShowtimeCalendarItemDto } from "@/features/admin/showtimes/types";
import {
  getFillPercent,
  getSoldSeats,
} from "@/features/admin/showtimes/utils/calendar-layout";

export interface ShowtimeStatusMeta {
  label: string;
  icon: LucideIcon;
  block: string;
  bar: string;
}

export const CONFLICT_BLOCK = "bg-warning/30 text-foreground";

const STATUS_META: Record<string, ShowtimeStatusMeta> = {
  [ShowtimeStatus.Scheduled]: {
    label: "Đã lên lịch",
    icon: CalendarCheck,
    block: "bg-foreground/10 text-foreground",
    bar: "bg-primary",
  },
  [ShowtimeStatus.Completed]: {
    label: "Đã hoàn thành",
    icon: CheckCircle2,
    block: "bg-foreground/5 text-muted-foreground",
    bar: "bg-muted-foreground",
  },
  [ShowtimeStatus.Cancelled]: {
    label: "Đã hủy",
    icon: XCircle,
    block: "bg-destructive/10 text-destructive",
    bar: "",
  },
};

export const getShowtimeStatusMeta = (status: string): ShowtimeStatusMeta =>
  STATUS_META[status] ?? { ...STATUS_META[ShowtimeStatus.Scheduled], label: status };

export const isCancelledShowtime = (showtime: ShowtimeCalendarItemDto) =>
  showtime.status === ShowtimeStatus.Cancelled;

export const getShowtimeBlockClass = (
  showtime: ShowtimeCalendarItemDto,
  conflict: boolean,
) =>
  conflict && !isCancelledShowtime(showtime)
    ? CONFLICT_BLOCK
    : getShowtimeStatusMeta(showtime.status).block;

export const formatTimeRange = (showtime: ShowtimeCalendarItemDto) =>
  `${formatShowtimeTime(showtime.startTime)} ~ ${formatShowtimeTime(showtime.endTime)}`;

export function describeShowtime(
  showtime: ShowtimeCalendarItemDto,
  conflict: boolean,
) {
  const meta = getShowtimeStatusMeta(showtime.status);
  const parts = [
    showtime.movieTitle,
    `từ ${formatShowtimeTime(showtime.startTime)} đến ${formatShowtimeTime(showtime.endTime)}`,
    showtime.roomName,
    `${getSoldSeats(showtime)} trên ${showtime.totalSeats} ghế đã bán`,
    meta.label,
  ];
  if (conflict) parts.push("trùng giờ với suất khác trong phòng");
  return parts.join(", ");
}

interface SeatFillBarProps {
  percent: number;
  barClassName: string;
  className?: string;
}

export const SeatFillBar = ({
  percent,
  barClassName,
  className,
}: SeatFillBarProps) => (
  <span aria-hidden className={cn("block h-1.5 w-full bg-foreground/20", className)}>
    <span
      className={cn("block h-full", barClassName)}
      style={{ width: `${percent}%` }}
    />
  </span>
);

interface SeatCountProps {
  showtime: ShowtimeCalendarItemDto;
  className?: string;
}

export const SeatCount = ({ showtime, className }: SeatCountProps) => {
  const full = showtime.totalSeats > 0 && showtime.availableSeats === 0;
  return (
    <span className={cn("t-code shrink-0 text-xs", full && "font-bold", className)}>
      {full
        ? "Hết ghế"
        : `${getSoldSeats(showtime)}/${showtime.totalSeats}`}
    </span>
  );
};

interface ShowtimeTooltipBodyProps {
  showtime: ShowtimeCalendarItemDto;
  conflict: boolean;
}

export const ShowtimeTooltipBody = ({
  showtime,
  conflict,
}: ShowtimeTooltipBodyProps) => {
  const meta = getShowtimeStatusMeta(showtime.status);
  return (
    <>
      <span className="font-medium">{showtime.movieTitle}</span>
      <span>{formatTimeRange(showtime)}</span>
      <span>
        {showtime.roomName}
        {showtime.screenType ? `, ${showtime.screenType}` : ""}
      </span>
      <span>Giá vé {formatCurrency(showtime.basePrice)}</span>
      <span>
        {getSoldSeats(showtime)}/{showtime.totalSeats} ghế đã bán (
        {getFillPercent(showtime)}%)
      </span>
      <span>{meta.label}</span>
      {conflict && <span>Trùng giờ với suất khác trong phòng</span>}
    </>
  );
};
