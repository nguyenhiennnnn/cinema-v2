import { format } from "date-fns";
import { toWallDate } from "@/lib/business-time";
import { getCurrencyLabel, getLocale } from "@/lib/regional-settings";

export function formatPoints(points: number) {
  return new Intl.NumberFormat(getLocale()).format(points);
}

export function formatCurrency(amount?: number | null): string {
  const value = amount == null || isNaN(amount) ? 0 : Math.round(amount);
  return `${new Intl.NumberFormat(getLocale()).format(value)} ${getCurrencyLabel()}`.trim();
}

export function formatPercent(
  value: number,
  opts?: { showSign?: boolean },
): string {
  const sign = opts?.showSign && value > 0 ? "+" : "";
  const text = new Intl.NumberFormat(getLocale(), {
    minimumFractionDigits: 1,
    maximumFractionDigits: 1,
  }).format(value);
  return `${sign}${text}%`;
}

export function formatNumber(
  value?: number | null,
  options?: Intl.NumberFormatOptions,
): string {
  if (value == null || isNaN(value)) return "0";
  return new Intl.NumberFormat(getLocale(), options).format(value);
}

export function formatShowtimeHour(iso: string) {
  return formatTime(iso, iso);
}

const DATE_OPTIONS: Intl.DateTimeFormatOptions = {
  day: "2-digit",
  month: "2-digit",
  year: "numeric",
};
const DAY_MONTH_OPTIONS: Intl.DateTimeFormatOptions = {
  day: "2-digit",
  month: "2-digit",
};
const TIME_OPTIONS: Intl.DateTimeFormatOptions = {
  hour: "2-digit",
  minute: "2-digit",
  hourCycle: "h23",
};

function formatWithIntl(
  date: string | Date | null | undefined,
  options: Intl.DateTimeFormatOptions,
  fallback: string,
): string {
  const d = toWallDate(date);
  if (!d) return fallback;
  try {
    return new Intl.DateTimeFormat(getLocale(), options).format(d);
  } catch {
    return fallback;
  }
}

export const formatDate = (date?: string | Date | null, fallback = "—") =>
  formatWithIntl(date, DATE_OPTIONS, fallback);

export const formatDayMonthLocalized = (
  date?: string | Date | null,
  fallback = "",
) => formatWithIntl(date, DAY_MONTH_OPTIONS, fallback);

export const formatTime = (date?: string | Date | null, fallback = "—") =>
  formatWithIntl(date, TIME_OPTIONS, fallback);

export const formatDateTime = (date?: string | Date | null, fallback = "—") =>
  formatWithIntl(date, { ...DATE_OPTIONS, ...TIME_OPTIONS }, fallback);

export function toDateInputValue(date?: string | Date | null): string | null {
  const d = toWallDate(date);
  return d ? format(d, "yyyy-MM-dd") : null;
}

export function toDateTimeInputValue(
  date?: string | Date | null,
): string | null {
  const d = toWallDate(date);
  return d ? format(d, "yyyy-MM-dd'T'HH:mm") : null;
}
