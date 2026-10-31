import type { ShowtimeCalendarDayDto, ShowtimeCalendarItemDto } from "../../types";
import { groupByRoom } from "../../utils/group-by-room";
import { ShowtimeCalendarItem } from "./showtime-calendar-item";

const HOUR_MARKS = Array.from({ length: 13 }, (_, i) => i * 2); // 0, 2, 4, ..., 24

interface ShowtimeCalendarDayProps {
  day: ShowtimeCalendarDayDto;
  onSelectShowtime?: (showtime: ShowtimeCalendarItemDto) => void;
}

export const ShowtimeCalendarDay = ({
  day,
  onSelectShowtime,
}: ShowtimeCalendarDayProps) => {
  const rooms = groupByRoom(day.showtimes);

  if (rooms.length === 0) {
    return (
      <div className="flex h-40 items-center justify-center border border-dashed text-sm text-muted-foreground">
        Không có suất chiếu nào trong ngày {day.date}.
      </div>
    );
  }

  return (
    <div className="overflow-x-auto border">
      <div className="min-w-225">
        <div className="flex border-b bg-muted/40">
          <div className="w-32 shrink-0 border-r px-3 py-2 text-xs font-medium text-muted-foreground">
            Phòng chiếu
          </div>
          <div className="relative flex-1">
            <div className="flex h-8">
              {HOUR_MARKS.map((hour) => (
                <div
                  key={hour}
                  className="flex-1 border-l text-xs text-muted-foreground first:border-l-0"
                >
                  <span className="ml-1">{hour}h</span>
                </div>
              ))}
            </div>
          </div>
        </div>

        {rooms.map((room) => (
          <div key={room.roomId} className="flex border-b last:border-b-0">
            <div className="w-32 shrink-0 border-r px-3 py-2">
              <p className="truncate text-sm font-medium">{room.roomName}</p>
              {room.screenType && (
                <p className="truncate text-xs text-muted-foreground">
                  {room.screenType}
                </p>
              )}
            </div>
            <div className="relative h-16 flex-1">
              <div className="pointer-events-none absolute inset-0 flex">
                {HOUR_MARKS.map((hour) => (
                  <div key={hour} className="flex-1 border-l first:border-l-0" />
                ))}
              </div>
              {room.showtimes.map((showtime) => (
                <ShowtimeCalendarItem
                  key={showtime.showtimeId}
                  showtime={showtime}
                  onClick={onSelectShowtime}
                />
              ))}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};
