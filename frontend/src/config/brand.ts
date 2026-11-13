import { ENV } from "./env";

export const BRAND = {
  name: "Perfo",
  siteName: "Perfo",
  tagline: "Đặt vé xem phim trực tuyến",
  defaultDescription:
    "Đặt vé xem phim nhanh chóng: chọn phim, suất chiếu, chọn ghế và thanh toán online chỉ trong vài phút.",
  defaultOgImage: "/brand/banner/perfo-og-1200x630.svg",
  locale: "vi_VN",
  supportEmail: "cskh@perfo.vn",
  supportHotline: "1900 xxxx (8h–22h)",
  adminEmailPlaceholder: "admin@perfo.vn",
  siteUrl: ENV.SITE_URL,
  storageKeys: {
    sessionHint: "perfo_session",
    theme: "vite-ui-theme",
  },
  legacyStorageKeys: {
    purge: ["perfo_refresh_token", "cinema_refresh_token", "cinema_auth"],
  },
} as const;
