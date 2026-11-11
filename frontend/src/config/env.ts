const optionalEnv = (value: unknown): string | undefined =>
  typeof value === "string" && value.trim() ? value.trim() : undefined;

export const ENV = {
  IS_DEV: import.meta.env.DEV,
  API_BASE_URL: (import.meta.env.VITE_API_BASE_URL as string | undefined) ?? "",
  GOOGLE_CLIENT_ID: import.meta.env.VITE_GOOGLE_CLIENT_ID as string | undefined,
  SITE_URL: (
    (import.meta.env.VITE_SITE_URL as string | undefined) ?? ""
  ).replace(/\/+$/, ""),
  BUSINESS_TIME_ZONE: optionalEnv(import.meta.env.VITE_BUSINESS_TIME_ZONE),
  LOCALE: optionalEnv(import.meta.env.VITE_LOCALE),
  CURRENCY_CODE: optionalEnv(import.meta.env.VITE_CURRENCY_CODE),
  CURRENCY_SYMBOL: optionalEnv(import.meta.env.VITE_CURRENCY_SYMBOL),
} as const;
