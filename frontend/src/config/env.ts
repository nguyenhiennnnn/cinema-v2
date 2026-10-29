export const ENV = {
  IS_DEV: import.meta.env.DEV,
  API_BASE_URL: (import.meta.env.VITE_API_BASE_URL as string | undefined) ?? "",
  GOOGLE_CLIENT_ID: import.meta.env.VITE_GOOGLE_CLIENT_ID as string | undefined,
  SITE_URL: (import.meta.env.VITE_SITE_URL as string | undefined) ?? "",
} as const;
