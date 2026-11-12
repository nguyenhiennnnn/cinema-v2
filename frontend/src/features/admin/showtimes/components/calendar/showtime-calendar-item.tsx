import { TriangleAlert } from "lucide-react";

import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from "@/components/ui/tooltip";
import { cn } from "@/lib/utils";
import type { ShowtimeCalendarItemDto } from "@/features/admin/showtimes/types";
import {
  getFillPercent,
  type PlacedShowtime,
} from "@/features/admin/showtimes/utils/calendar-layout";
import {
  describeShowtime,
  formatTimeRange,
  getShowtimeBlockClass,
  getShowtimeStatusMeta,
  isCancelledShowtime,
  SeatCount,
  SeatFillBar,
  ShowtimeTooltipBody,
} from "./showtime-calendar-shared";

export const LANE_HEIGHT = 88;
const BLOCK_GAP = 2;
const MIN_BLOCK_WIDTH = 56;

interface ShowtimeCalendarItemProps {
  placed: PlacedShowtime;
  windowStart: number;
  windowSpan: number;
  onClick?: (showtime: ShowtimeCalendarItemDto) => void;
}

export const ShowtimeCalendarItem = ({
  placed,
  windowStart,
  windowSpan,
  onClick,
}: ShowtimeCalendarItemProps) => {
  const { showtime, startMin, endMin, lane, conflict } = placed;
  const meta = getShowtimeStatusMeta(showtime.status);
  const StatusIcon = meta.icon;
  const cancelled = isCancelledShowtime(showtime);
  const interactive = Boolean(onClick);

  const leftPercent = ((startMin - windowStart) / windowSpan) * 100;
  const widthPercent = ((endMin - startMin) / windowSpan) * 100;

  const sharedProps = {
    "aria-label": describeShowtime(showtime, conflict),
    style: {
      left: `${leftPercent}%`,
      width: `max(calc(${widthPercent}% - ${BLOCK_GAP}px), ${MIN_BLOCK_WIDTH}px)`,
      top: lane * LANE_HEIGHT + 4,
      height: LANE_HEIGHT - 8,
    },
    className: cn(
      "group absolute z-10 flex flex-col justify-between gap-1 overflow-hidden cut-xs px-2 py-1 text-left text-xs transition-colors",
      getShowtimeBlockClass(showtime, conflict),
      interactive
        ? "cursor-pointer hover:bg-foreground/20"
        : "cursor-default",
    ),
  };

  const body = (
    <>
      <span className="flex items-start justify-between gap-1">
        <span
          className={cn(
            "line-clamp-2 text-[13px] leading-[1.2] font-medium",
            cancelled && "line-through",
            interactive && "group-hover:underline",
          )}
        >
          {showtime.movieTitle}
        </span>
        <span className="flex shrink-0 items-center gap-0.5">
          {conflict && !cancelled && (
            <TriangleAlert className="size-3.5" aria-hidden />
          )}
          <StatusIcon className="size-3.5" aria-hidden />
        </span>
      </span>
      <span className="t-code truncate text-xs">{formatTimeRange(showtime)}</span>
      {cancelled ? (
        <span className="text-xs font-medium">{meta.label}</span>
      ) : (
        <span className="flex items-center gap-2">
          <SeatFillBar
            percent={getFillPercent(showtime)}
            barClassName={meta.bar}
          />
          <SeatCount showtime={showtime} />
        </span>
      )}
    </>
  );

  return (
    <Tooltip>
      {interactive ? (
        <TooltipTrigger
          render={<button type="button" />}
          onClick={() => onClick?.(showtime)}
          {...sharedProps}
        >
          {body}
        </TooltipTrigger>
      ) : (
        <TooltipTrigger
          render={<div role="group" tabIndex={0} />}
          {...sharedProps}
        >
          {body}
        </TooltipTrigger>
      )}
      <TooltipContent side="top" className="flex-col items-start gap-0.5">
        <ShowtimeTooltipBody showtime={showtime} conflict={conflict} />
      </TooltipContent>
    </Tooltip>
  );
};
