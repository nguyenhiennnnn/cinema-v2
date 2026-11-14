export const DEFAULT_PAGE = 1;
export const DEFAULT_PAGE_SIZE = 10;

export const SEARCH_DEBOUNCE_MS = 300;

export const MAX_IMAGE_SIZE_BYTES = 5 * 1024 * 1024;
export const MAX_VIDEO_SIZE_BYTES = 100 * 1024 * 1024;
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

export const IMPORT_MAX_FILE_BYTES = 5 * 1024 * 1024;
export const IMPORT_ACCEPTED_EXTENSIONS = [".xlsx", ".csv"] as const;
export const IMPORT_ACCEPT = IMPORT_ACCEPTED_EXTENSIONS.join(",");
export const IMPORT_PREVIEW_PAGE_SIZE = 20;

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
export const EXPORT_TIMEOUT_MS = 60_000;
export const IMPORT_TIMEOUT_MS = 120_000;
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
export const BUSINESS_TIME_ZONE_BOOT_TIMEOUT_MS = 3_000;
export const FALLBACK_OPENING_MINUTES = 8 * 60;
export const FALLBACK_CLOSING_MINUTES = 23 * 60;
export const DEFAULT_PICKER_OFFSET_FROM_OPENING_MINUTES = 2 * 60;
export const QR_SCAN_REPEAT_GUARD_MS = 2500;
export const COMBOBOX_BLUR_CLOSE_DELAY_MS = 150;
export const GOOGLE_SDK_POLL_INTERVAL_MS = 100;
export const SIGNALR_RECONNECT_DELAYS_MS = [0, 1000, 3000, 5000, 10000];
export const IMPORT_EXPORT_RESYNC_INTERVAL_MS = 15_000;
export const IMPORT_EXPORT_PANEL_MAX_ITEMS = 20;
export const HERO_INTERVAL_MS = 7000;
export const HERO_TRANSITION_MS = 480;

export const FORM_PENDING_LABEL = "Đang lưu...";
export const FORM_CANCEL_LABEL = "Hủy";
export const FORM_SAVE_CHANGES_LABEL = "Lưu thay đổi";
export const IRREVERSIBLE_ACTION_MESSAGE = "Hành động này không thể hoàn tác.";

export const QUICK_CREATE_NAME_MAX_LENGTH = 50;
export const QUICK_CREATE_AGE_RATING_MAX_LENGTH = 10;
export const QUICK_CREATE_PERSON_NAME_MAX_LENGTH = 150;

export const NAME_MAX_LENGTH = 100;
export const EMAIL_MAX_LENGTH = 150;
export const PHONE_MAX_LENGTH = 20;
export const PERSON_NAME_MAX_LENGTH = 255;
export const IMAGE_URL_MAX_LENGTH = 500;
export const AGE_RATING_CODE_MAX_LENGTH = 10;
export const AGE_RATING_DESCRIPTION_MAX_LENGTH = 255;
export const COMBO_NAME_MAX_LENGTH = 150;
export const COMBO_DESCRIPTION_MAX_LENGTH = 500;
export const MOVIE_TITLE_MAX_LENGTH = 200;

export const PERCENT_MAX = 100;
export const ROOM_CREATE_MAX_ROWS = 50;
export const ROOM_CREATE_MAX_COLS = 50;
export const SEAT_SUGGESTION_MAX_COUNT = 10;
export const SEAT_GRID_MAX_ROWS = 52;
export const SEAT_GRID_MAX_COLS = 60;
export const SHOWTIME_CLONE_MAX_SOURCE_DAYS = 31;
export const AUTO_SCHEDULE_MAX_SHOWS_PER_MOVIE = 20;

export const VN_PHONE_REGEX = /^(0|\+84)[0-9]{9,10}$/;

export const URL_PLACEHOLDER = "https://...";
export const YOUTUBE_EMBED_BASE_URL = "https://www.youtube.com/embed";

export const MOVIE_LIST_STATUS_QUERY = {
  nowShowing: "now-showing",
  comingSoon: "coming-soon",
} as const;

export const SETTINGS_FIELD_MAX = {
  stopSellingMinutesBefore: 1440,
  roomBufferMinutes: 120,
  maxSeatsPerBooking: 50,
  maxComboQuantity: 50,
  loyaltyWindowMonths: 120,
  movieEndGraceDays: 365,
  showtimeLookaheadDays: 60,
  schedulerStepMinutes: 60,
  schedulerMinSameMovieGapMinutes: 1440,
  schedulerHeatLookbackDays: 365,
  schedulerHeatSalesWeight: 10,
  schedulerHeatRatingWeight: 10,
  schedulerMaxRating: 10,
} as const;
