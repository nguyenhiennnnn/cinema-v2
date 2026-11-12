import { getBusinessMinutesOfDay } from "@/lib/business-time";
import { ShowtimeStatus } from "@/types/enum";
import type { ShowtimeCalendarItemDto } from "../types";
import { groupByRoom } from "./group-by-room";

export const MINUTES_IN_DAY = 24 * 60;
export const MIN_GAP_MINUTES = 30;

export interface PlacedShowtime {
  showtime: ShowtimeCalendarItemDto;
  startMin: number;
  endMin: number;
  lane: number;
  conflict: boolean;
}

export interface FreeGap {
  startMin: number;
  endMin: number;
}

export interface RoomLayout {
  roomId: number;
  roomName: string;
  screenType?: string | null;
  items: PlacedShowtime[];
  laneCount: number;
  gaps: FreeGap[];
  conflictCount: number;
}

export interface DayLayout {
  rooms: RoomLayout[];
  windowStart: number;
  windowEnd: number;
  totalShowtimes: number;
  activeShowtimes: number;
  cancelledShowtimes: number;
  conflictCount: number;
  soldSeats: number;
  totalSeats: number;
}

const isCancelled = (s: ShowtimeCalendarItemDto) =>
  s.status === ShowtimeStatus.Cancelled;

function toRange(s: ShowtimeCalendarItemDto) {
  const startMin = getBusinessMinutesOfDay(s.startTime);
  const rawEnd = getBusinessMinutesOfDay(s.endTime);
  return { startMin, endMin: rawEnd > startMin ? rawEnd : MINUTES_IN_DAY };
}

function layoutRoom(
  room: ReturnType<typeof groupByRoom>[number],
): RoomLayout {
  const items: PlacedShowtime[] = room.showtimes
    .map((showtime) => ({
      showtime,
      ...toRange(showtime),
      lane: 0,
      conflict: false,
    }))
    .sort((a, b) => a.startMin - b.startMin || a.endMin - b.endMin);

  const lanes: PlacedShowtime[][] = [];
  const byPriority = [
    ...items.filter((i) => !isCancelled(i.showtime)),
    ...items.filter((i) => isCancelled(i.showtime)),
  ];
  for (const item of byPriority) {
    let lane = lanes.findIndex((placed) =>
      placed.every((p) => p.endMin <= item.startMin || p.startMin >= item.endMin),
    );
    if (lane === -1) {
      lane = lanes.length;
      lanes.push([]);
    }
    lanes[lane].push(item);
    item.lane = lane;
  }

  const live = items.filter((i) => !isCancelled(i.showtime));
  let reach = -1;
  let reachItem: PlacedShowtime | undefined;
  for (const item of live) {
    if (reachItem && item.startMin < reach) {
      item.conflict = true;
      reachItem.conflict = true;
    }
    if (item.endMin > reach) {
      reach = item.endMin;
      reachItem = item;
    }
  }

  const gaps: FreeGap[] = [];
  let cursor: number | null = null;
  for (const item of live) {
    if (cursor !== null && item.startMin - cursor >= MIN_GAP_MINUTES) {
      gaps.push({ startMin: cursor, endMin: item.startMin });
    }
    cursor = cursor === null ? item.endMin : Math.max(cursor, item.endMin);
  }

  return {
    roomId: room.roomId,
    roomName: room.roomName,
    screenType: room.screenType,
    items,
    laneCount: Math.max(lanes.length, 1),
    gaps,
    conflictCount: live.filter((i) => i.conflict).length,
  };
}

export function buildDayLayout(
  showtimes: ShowtimeCalendarItemDto[],
): DayLayout {
  const rooms = groupByRoom(showtimes).map(layoutRoom);
  const placed = rooms.flatMap((r) => r.items);

  const minStart = placed.length
    ? Math.min(...placed.map((i) => i.startMin))
    : 8 * 60;
  const maxEnd = placed.length
    ? Math.max(...placed.map((i) => i.endMin))
    : 22 * 60;

  let windowStart = Math.max(Math.floor(minStart / 60) * 60, 0);
  let windowEnd = Math.min(Math.ceil(maxEnd / 60) * 60, MINUTES_IN_DAY);
  while (windowEnd - windowStart < 4 * 60) {
    if (windowEnd < MINUTES_IN_DAY) windowEnd += 60;
    else if (windowStart > 0) windowStart -= 60;
    else break;
  }

  const live = placed.filter((i) => !isCancelled(i.showtime));
  return {
    rooms,
    windowStart,
    windowEnd,
    totalShowtimes: placed.length,
    activeShowtimes: live.length,
    cancelledShowtimes: placed.length - live.length,
    conflictCount: rooms.reduce((sum, r) => sum + r.conflictCount, 0),
    soldSeats: live.reduce(
      (sum, i) => sum + (i.showtime.totalSeats - i.showtime.availableSeats),
      0,
    ),
    totalSeats: live.reduce((sum, i) => sum + i.showtime.totalSeats, 0),
  };
}

export function getSoldSeats(s: ShowtimeCalendarItemDto) {
  return Math.max(s.totalSeats - s.availableSeats, 0);
}

export function getFillPercent(s: ShowtimeCalendarItemDto) {
  return s.totalSeats > 0
    ? Math.min(Math.round((getSoldSeats(s) / s.totalSeats) * 100), 100)
    : 0;
}

export function parseCalendarDay(value: string): Date | null {
  const m = /^(\d{2})\/(\d{2})\/(\d{4})$/.exec(value);
  if (!m) return null;
  return new Date(Number(m[3]), Number(m[2]) - 1, Number(m[1]));
}

export function formatMinutes(min: number) {
  const h = Math.floor(min / 60) % 24;
  const m = min % 60;
  return `${String(h).padStart(2, "0")}:${String(m).padStart(2, "0")}`;
}

export function formatGapLength(minutes: number) {
  const h = Math.floor(minutes / 60);
  const m = minutes % 60;
  if (h === 0) return `${m} phút`;
  return m === 0 ? `${h} giờ` : `${h} giờ ${m} phút`;
}
