import { ENV } from "./env";

export const BRAND = {
  name: "Perfo",
  siteName: "Perfo",
  tagline: "Đặt vé xem phim trực tuyến",
  defaultDescription:
    "Đặt vé xem phim nhanh chóng: chọn phim, suất chiếu, chọn ghế và thanh toán online chỉ trong vài phút.",
  defaultOgImage: "/favicon.svg",
  supportEmail: "cskh@perfo.vn",
  supportHotline: "1900 xxxx (8h–22h)",
  adminEmailPlaceholder: "admin@perfo.vn",
  siteUrl: ENV.SITE_URL,
  storageKeys: {
    refreshToken: "perfo_refresh_token",
    theme: "vite-ui-theme",
  },
  legacyStorageKeys: {
    refreshToken: "cinema_refresh_token",
    obsolete: ["cinema_auth"],
  },
} as const;
