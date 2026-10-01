export const DEFAULT_PAGE = 1;
export const DEFAULT_PAGE_SIZE = 10;
export const PAGE_SIZE_OPTIONS = [10, 20, 50, 100];

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
