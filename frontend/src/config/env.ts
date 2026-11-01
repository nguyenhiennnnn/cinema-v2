export const ENV = {
  IS_DEV: import.meta.env.DEV,
  API_BASE_URL: (import.meta.env.VITE_API_BASE_URL as string | undefined) ?? "",
  GOOGLE_CLIENT_ID: import.meta.env.VITE_GOOGLE_CLIENT_ID as string | undefined,
  SITE_URL: (
    (import.meta.env.VITE_SITE_URL as string | undefined) ?? ""
  ).replace(/\/+$/, ""),
  BUSINESS_TIME_ZONE:
    (import.meta.env.VITE_BUSINESS_TIME_ZONE as string | undefined) ??
    "Asia/Ho_Chi_Minh",
  LOCALE: (import.meta.env.VITE_LOCALE as string | undefined) ?? "vi-VN",
  CURRENCY_CODE:
    (import.meta.env.VITE_CURRENCY_CODE as string | undefined) ?? "VND",
  CURRENCY_SYMBOL:
    (import.meta.env.VITE_CURRENCY_SYMBOL as string | undefined) ?? "VNĐ",
} as const;
