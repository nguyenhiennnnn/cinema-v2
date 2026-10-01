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
