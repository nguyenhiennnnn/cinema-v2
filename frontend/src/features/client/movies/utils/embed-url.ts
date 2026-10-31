import { YOUTUBE_EMBED_BASE_URL } from "@/config/default-constants";

export function toEmbedUrl(url: string) {
  const match = url.match(/(?:youtu\.be\/|youtube\.com\/(?:watch\?v=|embed\/))([\w-]+)/);
  return match ? `${YOUTUBE_EMBED_BASE_URL}/${match[1]}?autoplay=1` : url;
}
