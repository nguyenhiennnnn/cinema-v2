/**
 * Cấu hình SEO dùng chung.
 *
 * Danh sách tiền tố dưới đây PHẢI khớp với `Disallow` trong `public/robots.txt`,
 * `scripts/build-sitemap.mjs` và header `X-Robots-Tag` ở `vercel.json` / `render.yaml`.
 */
export const NOINDEX_PATH_PREFIXES = [
  "/admin",
  "/auth",
  "/account",
  "/booking",
  "/403",
] as const;

export function isNoIndexPath(pathname: string): boolean {
  return NOINDEX_PATH_PREFIXES.some(
    (prefix) => pathname === prefix || pathname.startsWith(`${prefix}/`),
  );
}
