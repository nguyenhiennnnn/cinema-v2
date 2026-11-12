import { TriangleAlert } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { formatShowtimeTime } from "@/lib/cinema-format";
import { cn } from "@/lib/utils";
import type { ShowtimeCalendarItemDto } from "@/features/admin/showtimes/types";
import {
  formatGapLength,
  formatMinutes,
  getFillPercent,
  type DayLayout,
  type FreeGap,
  type PlacedShowtime,
  type RoomLayout,
} from "@/features/admin/showtimes/utils/calendar-layout";
import {
  describeShowtime,
  getShowtimeBlockClass,
  getShowtimeStatusMeta,
  isCancelledShowtime,
  SeatCount,
  SeatFillBar,
} from "./showtime-calendar-shared";

type AgendaEntry =
  | { kind: "show"; startMin: number; placed: PlacedShowtime }
  | { kind: "gap"; startMin: number; gap: FreeGap };

const buildEntries = (room: RoomLayout): AgendaEntry[] =>
  [
    ...room.items.map(
      (placed): AgendaEntry => ({ kind: "show", startMin: placed.startMin, placed }),
    ),
    ...room.gaps.map(
      (gap): AgendaEntry => ({ kind: "gap", startMin: gap.startMin, gap }),
    ),
  ].sort((a, b) => a.startMin - b.startMin);

interface AgendaShowtimeProps {
  placed: PlacedShowtime;
  onSelect?: (showtime: ShowtimeCalendarItemDto) => void;
}

const AgendaShowtime = ({ placed, onSelect }: AgendaShowtimeProps) => {
  const { showtime, conflict } = placed;
  const meta = getShowtimeStatusMeta(showtime.status);
  const StatusIcon = meta.icon;
  const cancelled = isCancelledShowtime(showtime);
  const interactive = Boolean(onSelect);

  const className = cn(
    "grid w-full grid-cols-[3.5rem_1fr] gap-3 cut-sm px-3 py-2 text-left transition-colors",
    getShowtimeBlockClass(showtime, conflict),
    interactive ? "cursor-pointer hover:bg-foreground/20" : "cursor-default",
  );

  const content = (
    <>
      <span className="flex flex-col">
        <span className="t-numeral text-xl">
          {formatShowtimeTime(showtime.startTime)}
        </span>
        <span className="t-code text-xs">
          {formatShowtimeTime(showtime.endTime)}
        </span>
      </span>
      <span className="flex min-w-0 flex-col gap-1">
        <span
          className={cn(
            "text-sm leading-tight font-medium",
            cancelled && "line-through",
          )}
        >
          {showtime.movieTitle}
        </span>
        <span className="flex flex-wrap items-center gap-x-3 gap-y-0.5 text-xs">
          <span className="flex items-center gap-1">
            <StatusIcon className="size-3.5" aria-hidden />
            {meta.label}
          </span>
          {conflict && !cancelled && (
            <span className="flex items-center gap-1 font-medium">
              <TriangleAlert className="size-3.5" aria-hidden />
              Trùng giờ
            </span>
          )}
        </span>
        {!cancelled && (
          <span className="flex items-center gap-2">
            <SeatFillBar
              percent={getFillPercent(showtime)}
              barClassName={meta.bar}
            />
            <SeatCount showtime={showtime} />
          </span>
        )}
      </span>
    </>
  );

  return interactive ? (
    <button
      type="button"
      className={className}
      aria-label={describeShowtime(showtime, conflict)}
      onClick={() => onSelect?.(showtime)}
    >
      {content}
    </button>
  ) : (
    <div className={className} role="group" aria-label={describeShowtime(showtime, conflict)}>
      {content}
    </div>
  );
};

interface ShowtimeCalendarAgendaProps {
  layout: DayLayout;
  onSelectShowtime?: (showtime: ShowtimeCalendarItemDto) => void;
  className?: string;
}

export const ShowtimeCalendarAgenda = ({
  layout,
  onSelectShowtime,
  className,
}: ShowtimeCalendarAgendaProps) => (
  <div className={cn("flex flex-col gap-6", className)}>
    {layout.rooms.map((room) => (
      <section
        key={room.roomId}
        aria-label={`${room.roomName}, ${room.items.length} suất chiếu`}
        className="flex flex-col gap-2"
      >
        <div className="flex flex-wrap items-baseline gap-x-2 gap-y-1">
          <h3 className="t-h3">{room.roomName}</h3>
          {room.screenType && (
            <span className="text-sm text-muted-foreground">
              {room.screenType}
            </span>
          )}
          {room.conflictCount > 0 && (
            <Badge variant="warning">
              <TriangleAlert data-icon="inline-start" />
              Trùng giờ
            </Badge>
          )}
        </div>
        <ul className="flex flex-col gap-1">
          {buildEntries(room).map((entry) =>
            entry.kind === "show" ? (
              <li key={entry.placed.showtime.showtimeId}>
                <AgendaShowtime
                  placed={entry.placed}
                  onSelect={onSelectShowtime}
                />
              </li>
            ) : (
              <li
                key={`gap-${entry.gap.startMin}`}
                className="border border-dashed border-border bg-foreground/5 px-3 py-1.5 text-xs text-muted-foreground"
              >
                Trống {formatGapLength(entry.gap.endMin - entry.gap.startMin)},
                từ {formatMinutes(entry.gap.startMin)} đến{" "}
                {formatMinutes(entry.gap.endMin)}
              </li>
            ),
          )}
        </ul>
      </section>
    ))}
  </div>
);
