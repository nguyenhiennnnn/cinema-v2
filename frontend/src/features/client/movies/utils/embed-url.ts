import { YOUTUBE_EMBED_BASE_URL } from "@/config/default-constants";

const YOUTUBE_URL_PATTERN = /(?:youtu\.be\/|youtube\.com\/(?:watch\?v=|embed\/))([\w-]+)/;

export function isYoutubeUrl(url: string) {
  return YOUTUBE_URL_PATTERN.test(url);
}

export function toEmbedUrl(url: string, autoplay = true) {
  const match = url.match(YOUTUBE_URL_PATTERN);
  if (!match) return url;
  return `${YOUTUBE_EMBED_BASE_URL}/${match[1]}${autoplay ? "?autoplay=1" : ""}`;
}
