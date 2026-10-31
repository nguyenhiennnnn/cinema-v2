import { format } from "date-fns";
import { toWallDate } from "@/lib/business-time";
import {
  formatTime as formatTimeUtil,
  toDateInputValue,
} from "@/lib/formatters";

export function toDateKey(date: Date | string | null | undefined) {
  return toDateInputValue(date) ?? "";
}

export function formatWeekdayShort(date: Date) {
  const day = date.getDay();
  return day === 0 ? "CN" : `Th ${day + 1}`;
}

export function formatDayMonth(value: Date | string) {
  const date = toWallDate(value);
  return date ? format(date, "dd/MM") : "";
}

export function formatShowtimeTime(value: string) {
  if (/^\d{2}:\d{2}$/.test(value)) return value;
  return formatTimeUtil(value);
}

export function formatShowtimeRange(startTime: string, endTime: string) {
  return `${formatShowtimeTime(startTime)} ~ ${formatShowtimeTime(endTime)}`;
}

export function formatDuration(minutes: number) {
  const h = Math.floor(minutes / 60);
  const m = minutes % 60;
  return h > 0 ? `${h}h${m > 0 ? ` ${m}p` : ""}` : `${m} phút`;
}
