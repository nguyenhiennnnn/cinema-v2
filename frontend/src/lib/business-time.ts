import { isValid, parseISO } from "date-fns";
import { ENV } from "@/config/env";
import { getServerNow } from "@/lib/server-clock";

export const DEFAULT_BUSINESS_TIME_ZONE = ENV.BUSINESS_TIME_ZONE;

const ZONE_DESIGNATOR = /T.*(Z|[+-]\d{2}:?\d{2})$/i;

function createPartsFormatter(timeZone: string) {
  return new Intl.DateTimeFormat("en-GB", {
    timeZone,
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
    second: "2-digit",
    hourCycle: "h23",
  });
}

function isValidTimeZone(id: string): boolean {
  try {
    createPartsFormatter(id);
    return true;
  } catch {
    return false;
  }
}

let businessTimeZone = DEFAULT_BUSINESS_TIME_ZONE;
let partsFormatter = createPartsFormatter(
  isValidTimeZone(businessTimeZone) ? businessTimeZone : "Asia/Ho_Chi_Minh",
);
const timeZoneListeners = new Set<() => void>();

export const getBusinessTimeZone = () => businessTimeZone;

export function subscribeBusinessTimeZone(listener: () => void) {
  timeZoneListeners.add(listener);
  return () => {
    timeZoneListeners.delete(listener);
  };
}

export function setBusinessTimeZone(id: string | null | undefined): boolean {
  const next = id?.trim();
  if (!next || !isValidTimeZone(next)) {
    console.warn(
      `[business-time] Múi giờ không hợp lệ "${id ?? ""}", giữ nguyên "${businessTimeZone}".`,
    );
    return false;
  }
  if (next === businessTimeZone) return true;

  businessTimeZone = next;
  partsFormatter = createPartsFormatter(next);
  timeZoneListeners.forEach((listener) => listener());
  return true;
}

export const hasZoneDesignator = (value: string) =>
  ZONE_DESIGNATOR.test(value.trim());

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

function getBusinessParts(instant: Date) {
  const parts = partsFormatter.formatToParts(instant);
  const pick = (type: string) =>
    Number(parts.find((part) => part.type === type)?.value ?? 0);
  return {
    year: pick("year"),
    month: pick("month"),
    day: pick("day"),
    hour: pick("hour"),
    minute: pick("minute"),
    second: pick("second"),
  };
}

export function instantToBusinessWall(instant: Date): Date {
  const p = getBusinessParts(instant);
  return new Date(p.year, p.month - 1, p.day, p.hour, p.minute, p.second);
}

export function toWallDate(value?: string | Date | null): Date | undefined {
  const parsed = parseDate(value);
  if (!parsed) return undefined;
  return typeof value === "string" && hasZoneDesignator(value)
    ? instantToBusinessWall(parsed)
    : parsed;
}

function getBusinessOffsetMinutes(instant: Date): number {
  const p = getBusinessParts(instant);
  const asUtc = Date.UTC(
    p.year,
    p.month - 1,
    p.day,
    p.hour,
    p.minute,
    p.second,
  );
  const instantSeconds = Math.floor(instant.getTime() / 1000) * 1000;
  return Math.round((asUtc - instantSeconds) / 60000);
}

function formatOffset(minutes: number): string {
  const sign = minutes >= 0 ? "+" : "-";
  const abs = Math.abs(minutes);
  const hh = String(Math.floor(abs / 60)).padStart(2, "0");
  const mm = String(abs % 60).padStart(2, "0");
  return `${sign}${hh}:${mm}`;
}

export function toBusinessIso(value: string): string {
  const trimmed = value.trim();
  if (hasZoneDesignator(trimmed)) return trimmed;

  const [datePart, timePart = "00:00"] = trimmed.split("T");
  const [hh = "00", mm = "00", ss = "00"] = timePart.split(":");
  const normalized = `${datePart}T${hh.padStart(2, "0")}:${mm.padStart(2, "0")}:${ss.padStart(2, "0")}`;

  const naiveUtc = Date.parse(`${normalized}Z`);
  if (Number.isNaN(naiveUtc)) return trimmed;

  const firstGuess = getBusinessOffsetMinutes(new Date(naiveUtc));
  const offset = getBusinessOffsetMinutes(
    new Date(naiveUtc - firstGuess * 60000),
  );
  return `${normalized}${formatOffset(offset)}`;
}

export const getBusinessNow = (now: Date = new Date(getServerNow())): Date =>
  instantToBusinessWall(now);

export function getBusinessTodayDate(
  now: Date = new Date(getServerNow()),
): Date {
  const wall = instantToBusinessWall(now);
  return new Date(wall.getFullYear(), wall.getMonth(), wall.getDate());
}

export function getBusinessToday(now: Date = new Date(getServerNow())): string {
  const p = getBusinessParts(now);
  return `${p.year}-${String(p.month).padStart(2, "0")}-${String(p.day).padStart(2, "0")}`;
}

export function getBusinessMinutesOfDay(value: string | Date): number {
  const wall = toWallDate(value);
  return wall ? wall.getHours() * 60 + wall.getMinutes() : 0;
}
