import { useEffect, useState } from "react";
import { CalendarX, TriangleAlert } from "lucide-react";

import { EmptyState } from "@/components/common/empty-state";
import { Badge } from "@/components/ui/badge";
import { getBusinessNow, getBusinessToday } from "@/lib/business-time";
import { toDateInputValue } from "@/lib/formatters";
import type {
  ShowtimeCalendarDayDto,
  ShowtimeCalendarItemDto,
} from "@/features/admin/showtimes/types";
import {
  formatGapLength,
  formatMinutes,
  parseCalendarDay,
  type DayLayout,
} from "@/features/admin/showtimes/utils/calendar-layout";
import { ShowtimeCalendarAgenda } from "./showtime-calendar-agenda";
import { LANE_HEIGHT, ShowtimeCalendarItem } from "./showtime-calendar-item";

import { NOW_TICK_INTERVAL_MS } from "@/config/default-constants";

const MIN_HOUR_WIDTH = 88;
const ROOM_COLUMN_WIDTH = 144;
const GAP_LABEL_MIN_PERCENT = 6;

const currentMinutes = () => {
  const now = getBusinessNow();
  return now.getHours() * 60 + now.getMinutes();
};

function useNowMinutes(enabled: boolean) {
  const [now, setNow] = useState(currentMinutes);
  useEffect(() => {
    if (!enabled) return;
    const id = window.setInterval(() => setNow(currentMinutes()), NOW_TICK_INTERVAL_MS);
    return () => window.clearInterval(id);
  }, [enabled]);
  return now;
}

interface ShowtimeCalendarDayProps {
  day: ShowtimeCalendarDayDto;
  layout: DayLayout;
  onSelectShowtime?: (showtime: ShowtimeCalendarItemDto) => void;
}

export const ShowtimeCalendarDay = ({
  day,
  layout,
  onSelectShowtime,
}: ShowtimeCalendarDayProps) => {
  const parsedDay = parseCalendarDay(day.date);
  const isToday =
    parsedDay !== null && toDateInputValue(parsedDay) === getBusinessToday();
  const nowMinutes = useNowMinutes(isToday);

  if (layout.rooms.length === 0) {
    return (
      <EmptyState
        icon={CalendarX}
        title="Không có suất chiếu"
        description={`Ngày ${day.date} chưa có suất chiếu nào.`}
      />
    );
  }

  const { windowStart, windowEnd } = layout;
  const windowSpan = windowEnd - windowStart;
  const hourCount = windowSpan / 60;
  const hourMarks = Array.from({ length: hourCount + 1 }, (_, i) => i);
  const toPercent = (minute: number) =>
    ((minute - windowStart) / windowSpan) * 100;
  const showNow =
    isToday && nowMinutes >= windowStart && nowMinutes <= windowEnd;
  const fillPercent =
    layout.totalSeats > 0
      ? Math.round((layout.soldSeats / layout.totalSeats) * 100)
      : 0;

  return (
    <div className="flex flex-col gap-3">
      <p className="flex flex-wrap items-center gap-x-3 gap-y-1 text-sm text-muted-foreground">
        <span>
          <span className="t-code text-foreground">{layout.activeShowtimes}</span>{" "}
          suất chiếu trong{" "}
          <span className="t-code text-foreground">{layout.rooms.length}</span>{" "}
          phòng, lấp đầy{" "}
          <span className="t-code text-foreground">{fillPercent}%</span> ghế
        </span>
        {layout.cancelledShowtimes > 0 && (
          <Badge variant="destructive">
            {layout.cancelledShowtimes} suất đã hủy
          </Badge>
        )}
        {layout.conflictCount > 0 && (
          <Badge variant="warning">
            <TriangleAlert data-icon="inline-start" />
            {layout.conflictCount} suất trùng giờ
          </Badge>
        )}
      </p>

      <div className="hidden overflow-x-auto bg-card cut-md md:block">
        <div
          style={{ minWidth: ROOM_COLUMN_WIDTH + hourCount * MIN_HOUR_WIDTH }}
        >
          <div className="flex border-b border-border">
            <div
              className="sticky left-0 z-20 shrink-0 bg-card px-3 py-2 text-[13px] font-semibold text-muted-foreground"
              style={{ width: ROOM_COLUMN_WIDTH }}
            >
              Phòng chiếu
            </div>
            <div className="relative h-9 flex-1" aria-hidden>
              {hourMarks.map((hour) => (
                <span
                  key={hour}
                  className="t-code absolute inset-y-0 flex items-center border-l border-border pl-1.5 text-xs text-muted-foreground"
                  style={{ left: `${(hour / hourCount) * 100}%` }}
                >
                  {hour < hourCount && formatMinutes(windowStart + hour * 60)}
                </span>
              ))}
            </div>
          </div>

          <div className="relative">
            {showNow && (
              <div
                className="pointer-events-none absolute inset-y-0 z-10"
                style={{ left: ROOM_COLUMN_WIDTH, right: 0 }}
                aria-hidden
              >
                <div
                  className="absolute inset-y-0 w-0.5 bg-primary"
                  style={{ left: `${toPercent(nowMinutes)}%` }}
                />
              </div>
            )}

            {layout.rooms.map((room) => {
              const rowHeight = room.laneCount * LANE_HEIGHT;
              return (
                <div
                  key={room.roomId}
                  role="group"
                  aria-label={`${room.roomName}, ${room.items.length} suất chiếu`}
                  className="flex border-b border-border last:border-b-0"
                >
                  <div
                    className="sticky left-0 z-20 flex shrink-0 flex-col justify-center gap-1 bg-card px-3 py-2"
                    style={{ width: ROOM_COLUMN_WIDTH }}
                  >
                    <p className="line-clamp-2 text-sm leading-tight font-semibold">
                      {room.roomName}
                    </p>
                    {room.screenType && (
                      <p className="text-xs text-muted-foreground">
                        {room.screenType}
                      </p>
                    )}
                    {room.conflictCount > 0 && (
                      <Badge variant="warning" className="w-fit">
                        <TriangleAlert data-icon="inline-start" />
                        Trùng giờ
                      </Badge>
                    )}
                  </div>

                  <div className="relative flex-1" style={{ height: rowHeight }}>
                    {hourMarks.map((hour) => (
                      <span
                        key={hour}
                        aria-hidden
                        className="pointer-events-none absolute inset-y-0 border-l border-border"
                        style={{ left: `${(hour / hourCount) * 100}%` }}
                      />
                    ))}

                    {room.gaps.map((gap) => {
                      const width =
                        ((gap.endMin - gap.startMin) / windowSpan) * 100;
                      return (
                        <span
                          key={gap.startMin}
                          role="img"
                          aria-label={`Trống ${formatGapLength(gap.endMin - gap.startMin)}, từ ${formatMinutes(gap.startMin)} đến ${formatMinutes(gap.endMin)}`}
                          className="absolute flex items-center justify-center border border-dashed border-border bg-foreground/5 px-1 text-center text-xs leading-tight text-muted-foreground"
                          style={{
                            left: `${toPercent(gap.startMin)}%`,
                            width: `${width}%`,
                            top: 4,
                            height: rowHeight - 8,
                          }}
                        >
                          {width >= GAP_LABEL_MIN_PERCENT && (
                            <span aria-hidden>
                              Trống {formatGapLength(gap.endMin - gap.startMin)}
                            </span>
                          )}
                        </span>
                      );
                    })}

                    {room.items.map((placed) => (
                      <ShowtimeCalendarItem
                        key={placed.showtime.showtimeId}
                        placed={placed}
                        windowStart={windowStart}
                        windowSpan={windowSpan}
                        onClick={onSelectShowtime}
                      />
                    ))}
                  </div>
                </div>
              );
            })}
          </div>
        </div>
      </div>

      <ShowtimeCalendarAgenda
        layout={layout}
        onSelectShowtime={onSelectShowtime}
        className="md:hidden"
      />
    </div>
  );
};
