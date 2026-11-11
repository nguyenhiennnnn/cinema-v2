import { ENV } from "@/config/env";

interface RegionalSettings {
  locale?: string | null;
  currencyCode?: string | null;
  currencySymbol?: string | null;
}

const detectSystemLocale = () => Intl.DateTimeFormat().resolvedOptions().locale;

let locale = ENV.LOCALE ?? detectSystemLocale();
let currencyCode = ENV.CURRENCY_CODE ?? "";
let currencySymbol = ENV.CURRENCY_SYMBOL ?? "";

const pick = (value: string | null | undefined, current: string) =>
  value?.trim() || current;

export function setRegionalSettings(settings: RegionalSettings) {
  locale = pick(settings.locale, locale);
  currencyCode = pick(settings.currencyCode, currencyCode);
  currencySymbol = pick(settings.currencySymbol, currencySymbol);
}

export const getLocale = () => locale;

export const getCurrencyLabel = () => currencySymbol || currencyCode;
