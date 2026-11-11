import { set } from "date-fns";

export function combineDateAndTime(date: Date, time: string): Date {
  const [hours, minutes] = time.split(":").map((n) => Number(n) || 0);
  return set(date, { hours, minutes, seconds: 0, milliseconds: 0 });
}

export function toTimeValue(date?: Date | null): string {
  if (!date) return "";
  return `${String(date.getHours()).padStart(2, "0")}:${String(date.getMinutes()).padStart(2, "0")}`;
}

const MINUTES_PER_DAY = 24 * 60;

export function timeToMinutes(value?: string | null): number | null {
  const match = value?.match(/^(\d{1,2}):(\d{2})/);
  return match ? Number(match[1]) * 60 + Number(match[2]) : null;
}

export function minutesToTimeValue(totalMinutes: number): string {
  const wrapped =
    ((totalMinutes % MINUTES_PER_DAY) + MINUTES_PER_DAY) % MINUTES_PER_DAY;
  const hours = String(Math.floor(wrapped / 60)).padStart(2, "0");
  const minutes = String(wrapped % 60).padStart(2, "0");
  return `${hours}:${minutes}`;
}

export function isWithinOperatingWindow(
  minutes: number,
  openingMinutes: number,
  closingMinutes: number,
): boolean {
  return openingMinutes < closingMinutes
    ? minutes >= openingMinutes && minutes < closingMinutes
    : minutes >= openingMinutes || minutes < closingMinutes;
}
