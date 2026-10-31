import { format } from "date-fns";
import { ENV } from "@/config/env";
import { toWallDate } from "@/lib/business-time";

// ─── Locale & currency config (một điểm duy nhất) ───────────────────────────
const LOCALE = ENV.LOCALE;
const CURRENCY_SYMBOL = ENV.CURRENCY_SYMBOL;

// ─── Number / currency formatters ────────────────────────────────────────────

export function formatPoints(points: number) {
  return new Intl.NumberFormat(LOCALE).format(points);
}

export function formatCurrency(amount?: number | null): string {
  if (amount == null || isNaN(amount)) return `0 ${CURRENCY_SYMBOL}`;
  return `${new Intl.NumberFormat(LOCALE).format(Math.round(amount))} ${CURRENCY_SYMBOL}`;
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
  return new Intl.NumberFormat(LOCALE, options).format(value);
}

// ─── Date / time formatters ──────────────────────────────────────────────────

export function formatShowtimeHour(iso: string) {
  return formatTime(iso, iso);
}

function formatWithPattern(
  date: string | Date | null | undefined,
  pattern: string,
  fallback: string,
): string {
  const d = toWallDate(date);
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
  const d = toWallDate(date);
  return d ? format(d, "yyyy-MM-dd") : null;
}

export function toDateTimeInputValue(
  date?: string | Date | null,
): string | null {
  const d = toWallDate(date);
  return d ? format(d, "yyyy-MM-dd'T'HH:mm") : null;
}
