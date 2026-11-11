import { enUS, vi } from "date-fns/locale";
import type { Locale } from "date-fns";

const LOCALES: Record<string, Locale> = { vi, en: enUS };

export function getDateFnsLocale(tag: string): Locale | undefined {
  const language = tag.split("-")[0]?.toLowerCase() ?? "";
  return LOCALES[language];
}
