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

const regionalListeners = new Set<() => void>();

export const getRegionalKey = () =>
  `${locale}|${currencyCode}|${currencySymbol}`;

export function subscribeRegionalSettings(listener: () => void) {
  regionalListeners.add(listener);
  return () => {
    regionalListeners.delete(listener);
  };
}

export function setRegionalSettings(settings: RegionalSettings) {
  const before = getRegionalKey();
  locale = pick(settings.locale, locale);
  currencyCode = pick(settings.currencyCode, currencyCode);
  currencySymbol = pick(settings.currencySymbol, currencySymbol);
  if (getRegionalKey() !== before) regionalListeners.forEach((l) => l());
}

export const getLocale = () => locale;

export const getCurrencyLabel = () => currencySymbol || currencyCode;
