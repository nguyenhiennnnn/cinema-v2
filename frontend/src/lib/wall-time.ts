import { set } from "date-fns";

export function combineDateAndTime(date: Date, time: string): Date {
  const [hours, minutes] = time.split(":").map((n) => Number(n) || 0);
  return set(date, { hours, minutes, seconds: 0, milliseconds: 0 });
}

export function toTimeValue(date?: Date | null): string {
  if (!date) return "";
  return `${String(date.getHours()).padStart(2, "0")}:${String(date.getMinutes()).padStart(2, "0")}`;
}
