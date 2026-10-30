import { format, isValid, parseISO } from "date-fns";

export function formatPoints(points: number) {
  return new Intl.NumberFormat("vi-VN").format(points);
}

export function formatCurrency(amount?: number | null): string {
  if (amount == null || isNaN(amount)) return "0 VNĐ";
  return `${new Intl.NumberFormat("vi-VN").format(Math.round(amount))} VNĐ`;
}

export function formatPercent(
  value: number,
  opts?: { showSign?: boolean },
): string {
  const sign = opts?.showSign && value > 0 ? "+" : "";
  return `${sign}${value.toFixed(1)}%`;
}

export function formatNumber(
  value?: number | null,
  options?: Intl.NumberFormatOptions,
): string {
  if (value == null || isNaN(value)) return "0";
  return new Intl.NumberFormat("vi-VN", options).format(value);
}

export function formatShowtimeHour(iso: string) {
  const date = parseDate(iso);
  return date
    ? date.toLocaleTimeString("vi-VN", { hour: "2-digit", minute: "2-digit" })
    : iso;
}

export function parseDate(date?: string | Date | null): Date | undefined {
  if (!date) return undefined;

  if (date instanceof Date) {
    return isValid(date) ? date : undefined;
  }

  const trimmed = date.trim();
  if (!trimmed || trimmed.startsWith("0001-01-01")) return undefined;

  const isoParsed = parseISO(trimmed);
  if (isValid(isoParsed)) return isoParsed;

  const nativeParsed = new Date(trimmed);
  return isValid(nativeParsed) ? nativeParsed : undefined;
}

function formatWithPattern(
  date: string | Date | null | undefined,
  pattern: string,
  fallback: string,
): string {
  const d = parseDate(date);
  if (!d) return fallback;
  try {
    return format(d, pattern);
  } catch {
    return fallback;
  }
}

export const formatDate = (date?: string | Date | null, fallback = "—") =>
  formatWithPattern(date, "dd/MM/yyyy", fallback);

export const formatTime = (date?: string | Date | null, fallback = "—") =>
  formatWithPattern(date, "HH:mm", fallback);

export const formatDateTime = (date?: string | Date | null, fallback = "—") =>
  formatWithPattern(date, "dd/MM/yyyy HH:mm", fallback);

export const formatDateTimeFull = (
  date?: string | Date | null,
  fallback = "—",
) => formatWithPattern(date, "dd/MM/yyyy HH:mm:ss", fallback);

export function toDateInputValue(date?: string | Date | null): string | null {
  const d = parseDate(date);
  return d ? format(d, "yyyy-MM-dd") : null;
}

export function toDateTimeInputValue(
  date?: string | Date | null,
): string | null {
  const d = parseDate(date);
  return d ? format(d, "yyyy-MM-dd'T'HH:mm") : null;
}

export const BUSINESS_TIME_ZONE = "Asia/Ho_Chi_Minh";
const BUSINESS_UTC_OFFSET = "+07:00";

function getBusinessParts(date: Date) {
  const parts = new Intl.DateTimeFormat("en-GB", {
    timeZone: BUSINESS_TIME_ZONE,
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
    hourCycle: "h23",
  }).formatToParts(date);
  const pick = (type: string) =>
    parts.find((part) => part.type === type)?.value ?? "00";
  return {
    date: `${pick("year")}-${pick("month")}-${pick("day")}`,
    time: `${pick("hour")}:${pick("minute")}`,
  };
}

export function getBusinessToday(now: Date = new Date()): string {
  return getBusinessParts(now).date;
}

export function toBusinessDateTimeInputValue(
  date?: string | Date | null,
): string | null {
  const d = parseDate(date);
  if (!d) return null;
  const parts = getBusinessParts(d);
  return `${parts.date}T${parts.time}`;
}

export function toBusinessIso(value: string): string {
  const trimmed = value.trim();
  if (/(Z|[+-]\d{2}:?\d{2})$/i.test(trimmed)) return trimmed;
  const withSeconds = /T\d{2}:\d{2}$/.test(trimmed) ? `${trimmed}:00` : trimmed;
  return `${withSeconds}${BUSINESS_UTC_OFFSET}`;
}
