import type { ShowtimeCalendarItemDto } from "../types";

export interface RoomRow {
  roomId: number;
  roomName: string;
  screenType?: string | null;
  showtimes: ShowtimeCalendarItemDto[];
}

export function groupByRoom(showtimes: ShowtimeCalendarItemDto[]): RoomRow[] {
  const map = new Map<number, RoomRow>();

  for (const showtime of showtimes) {
    const row = map.get(showtime.roomId);
    if (row) {
      row.showtimes.push(showtime);
    } else {
      map.set(showtime.roomId, {
        roomId: showtime.roomId,
        roomName: showtime.roomName,
        screenType: showtime.screenType,
        showtimes: [showtime],
      });
    }
  }

  return Array.from(map.values()).sort((a, b) =>
    a.roomName.localeCompare(b.roomName, "vi"),
  );
}
