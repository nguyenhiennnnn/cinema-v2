export const DEFAULT_PAGE = 1;
export const DEFAULT_PAGE_SIZE = 10;

export const SEARCH_DEBOUNCE_MS = 300;

export const MAX_IMAGE_SIZE_BYTES = 5 * 1024 * 1024;
export const MAX_VIDEO_SIZE_BYTES = 100 * 1024 * 1024; // 100MB
export const ACCEPTED_IMAGE_TYPES = [
  "image/png",
  "image/jpeg",
  "image/webp",
] as const;
export const ACCEPTED_VIDEO_TYPES = [
  "video/mp4",
  "video/quicktime",
  "video/webm",
] as const;

export const ACCEPTED_IMAGE_ACCEPT = ACCEPTED_IMAGE_TYPES.join(",");
export const ACCEPTED_VIDEO_ACCEPT = ACCEPTED_VIDEO_TYPES.join(",");

export const PAGE_SIZE_OPTIONS = [10, 20, 30, 50];
export const LOOKUP_PAGE_SIZE = 100;
export const SEARCH_LOOKUP_PAGE_SIZE = 20;
export const COUNT_ONLY_PAGE_SIZE = 1;
export const ACCOUNT_LIST_PAGE_LIMIT = 10;
export const HOME_MOVIE_SECTION_LIMIT = 10;
export const HERO_CAROUSEL_MOVIES_LIMIT = 5;
export const DEFAULT_MOVIES_PAGE_LIMIT = 12;
export const TOP_MOVIES_REPORT_LIMIT = 10;

export const API_TIMEOUT_MS = 20_000;
export const QUERY_STALE_TIME_MS = 30 * 1000;
export const QUERY_GC_TIME_MS = 5 * 60 * 1000;
export const QUERY_MAX_RETRIES = 2;
export const MUTATION_MAX_RETRIES = 0;
export const NO_RETRY_STATUS_MIN = 400;
export const NO_RETRY_STATUS_MAX = 500;
export const DASHBOARD_REFETCH_INTERVAL_MS = 60_000;
export const BOOKING_RESULT_POLL_INTERVAL_MS = 3_000;
export const BOOKING_HOLD_POLL_INTERVAL_MS = 10_000;
export const SEAT_SUGGESTIONS_STALE_TIME_MS = 0;
export const SEAT_SUGGESTIONS_GC_TIME_MS = 0;
export const SETTINGS_STALE_TIME_MS = 5 * 60 * 1000;
export const NOW_TICK_INTERVAL_MS = 60_000;
export const QR_SCAN_REPEAT_GUARD_MS = 2500;
export const COMBOBOX_BLUR_CLOSE_DELAY_MS = 150;
export const GOOGLE_SDK_POLL_INTERVAL_MS = 100;
export const SIGNALR_RECONNECT_DELAYS_MS = [0, 1000, 3000, 5000, 10000];

export const FORM_PENDING_LABEL = "Đang lưu...";
export const FORM_CANCEL_LABEL = "Hủy";

export const QUICK_CREATE_NAME_MAX_LENGTH = 50;
export const QUICK_CREATE_AGE_RATING_MAX_LENGTH = 10;
export const QUICK_CREATE_PERSON_NAME_MAX_LENGTH = 150;

export const URL_PLACEHOLDER = "https://...";
export const YOUTUBE_EMBED_BASE_URL = "https://www.youtube.com/embed";
