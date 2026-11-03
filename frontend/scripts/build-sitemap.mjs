import { existsSync, mkdirSync, readFileSync, writeFileSync } from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

const root = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "..");
const distDir = path.join(root, "dist");

const DISALLOW = ["/admin", "/auth", "/account", "/booking"];
const STATIC_PAGES = [
  { path: "/", changefreq: "daily", priority: "1.0" },
  { path: "/movies", changefreq: "daily", priority: "0.9" },
  { path: "/schedule", changefreq: "daily", priority: "0.8" },
  { path: "/membership", changefreq: "monthly", priority: "0.5" },
  { path: "/policies/terms", changefreq: "yearly", priority: "0.3" },
  { path: "/policies/privacy", changefreq: "yearly", priority: "0.3" },
  { path: "/policies/refund", changefreq: "yearly", priority: "0.3" },
  { path: "/policies/age-rating", changefreq: "yearly", priority: "0.3" },
];
const PAGE_LIMIT = 50; 
const MAX_PAGES = 200; 
const FETCH_TIMEOUT_MS = 15_000;

function loadEnvFiles() {
  const result = {};
  for (const name of [".env", ".env.local", ".env.production"]) {
    const file = path.join(root, name);
    if (!existsSync(file)) continue;
    for (const line of readFileSync(file, "utf8").split(/\r?\n/)) {
      const match = line.match(/^\s*(VITE_[A-Z0-9_]+)\s*=\s*(.*)\s*$/);
      if (match) result[match[1]] = match[2].replace(/^["']|["']$/g, "");
    }
  }
  return result;
}

const fileEnv = loadEnvFiles();
const readEnv = (key) => (process.env[key] || fileEnv[key] || "").trim();

const siteUrl = readEnv("VITE_SITE_URL").replace(/\/+$/, "");
const apiBase = readEnv("VITE_API_BASE_URL").replace(/\/+$/, "");

const escapeXml = (value) =>
  value
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&apos;");

async function fetchMovieIds() {
  if (!apiBase) {
    console.warn("[sitemap] Thiếu VITE_API_BASE_URL — bỏ qua trang chi tiết phim.");
    return [];
  }
  const ids = [];
  let cursor;
  for (let page = 0; page < MAX_PAGES; page += 1) {
    const url = new URL(`${apiBase}/movies`);
    url.searchParams.set("limit", String(PAGE_LIMIT));
    if (cursor) url.searchParams.set("cursor", cursor);

    const res = await fetch(url, {
      headers: { Accept: "application/json" },
      signal: AbortSignal.timeout(FETCH_TIMEOUT_MS),
    });
    if (!res.ok) throw new Error(`GET ${url} → HTTP ${res.status}`);

    const { data } = await res.json();
    for (const item of data?.items ?? []) ids.push(item.movieId);
    if (!data?.hasMore || !data?.nextCursor) break;
    cursor = data.nextCursor;
  }
  return [...new Set(ids)];
}

async function main() {
  if (!siteUrl) {
    console.warn("[sitemap] Thiếu VITE_SITE_URL — bỏ qua sitemap.xml / robots.txt.");
    return;
  }

  let movieIds = [];
  try {
    movieIds = await fetchMovieIds();
  } catch (error) {
    console.warn(
      `[sitemap] Không lấy được danh sách phim, chỉ xuất trang tĩnh: ${error.message}`,
    );
  }

  const urls = [
    ...STATIC_PAGES,
    ...movieIds.map((id) => ({
      path: `/movies/${id}`,
      changefreq: "weekly",
      priority: "0.7",
    })),
  ];

  const sitemap = [
    '<?xml version="1.0" encoding="UTF-8"?>',
    '<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">',
    ...urls.map(
      (u) =>
        `  <url><loc>${escapeXml(siteUrl + u.path)}</loc>` +
        `<changefreq>${u.changefreq}</changefreq><priority>${u.priority}</priority></url>`,
    ),
    "</urlset>",
    "",
  ].join("\n");

  const robots = [
    "User-agent: *",
    "Allow: /",
    ...DISALLOW.map((p) => `Disallow: ${p}`),
    "",
    `Sitemap: ${siteUrl}/sitemap.xml`,
    "",
  ].join("\n");

  mkdirSync(distDir, { recursive: true });
  writeFileSync(path.join(distDir, "sitemap.xml"), sitemap);
  writeFileSync(path.join(distDir, "robots.txt"), robots);
  console.log(`[sitemap] Đã ghi dist/sitemap.xml (${urls.length} URL) và dist/robots.txt`);
}

main().catch((error) => {
  // Không để lỗi SEO làm hỏng deploy.
  console.warn(`[sitemap] Bỏ qua do lỗi: ${error.message}`);
});
