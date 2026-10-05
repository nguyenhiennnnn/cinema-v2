import fs from "fs";
import path from "path";
import { fileURLToPath } from "url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));

// ====== CẤU HÌNH ======
const SITE_URL = process.env.VITE_SITE_URL ?? "https://cinema-v2-jade.vercel.app";
const API_BASE_URL =
  process.env.VITE_API_BASE_URL ?? "https://REPLACE_API_BASE_URL/api/v1";
const OUTPUT_PATH = path.resolve(__dirname, "../public/sitemap.xml");
// ======================

interface MovieItem {
  movieId: number;
  title: string;
  updatedAt?: string;
}

interface ApiResponse {
  success: boolean;
  data: {
    items: MovieItem[];
    hasMore: boolean;
    nextCursor: string | null;
  };
}

async function fetchAllMovies(): Promise<MovieItem[]> {
  const movies: MovieItem[] = [];
  let cursor: string | null = null;

  console.log("📡 Fetching movies from API...");

  while (true) {
    const params = new URLSearchParams({ limit: "100" });
    if (cursor) params.set("cursor", cursor);

    const url = `${API_BASE_URL}/movies?${params}`;
    const res = await fetch(url);

    if (!res.ok) {
      console.warn(`⚠️  API responded with ${res.status}, skipping movie pages.`);
      break;
    }

    const json: ApiResponse = await res.json();
    movies.push(...json.data.items);
    console.log(`  ✔ Fetched ${movies.length} movies so far...`);

    if (!json.data.hasMore || !json.data.nextCursor) break;
    cursor = json.data.nextCursor;
  }

  return movies;
}

function formatDate(iso?: string) {
  if (!iso) return new Date().toISOString().split("T")[0];
  return iso.split("T")[0];
}

function buildSitemap(movies: MovieItem[]): string {
  const today = new Date().toISOString().split("T")[0];

  const staticPages = [
    { loc: "/", changefreq: "daily", priority: "1.0", lastmod: today },
    { loc: "/movies", changefreq: "daily", priority: "0.9", lastmod: today },
    { loc: "/schedule", changefreq: "daily", priority: "0.8", lastmod: today },
    { loc: "/membership", changefreq: "weekly", priority: "0.6", lastmod: today },
  ];

  const staticUrls = staticPages
    .map(
      (p) => `
  <url>
    <loc>${SITE_URL}${p.loc}</loc>
    <lastmod>${p.lastmod}</lastmod>
    <changefreq>${p.changefreq}</changefreq>
    <priority>${p.priority}</priority>
  </url>`
    )
    .join("");

  const movieUrls = movies
    .map(
      (m) => `
  <url>
    <loc>${SITE_URL}/movies/${m.movieId}</loc>
    <lastmod>${formatDate(m.updatedAt)}</lastmod>
    <changefreq>weekly</changefreq>
    <priority>0.7</priority>
  </url>`
    )
    .join("");

  return `<?xml version="1.0" encoding="UTF-8"?>
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
${staticUrls}
${movieUrls}
</urlset>`;
}

async function main() {
  try {
    const movies = await fetchAllMovies();
    const sitemap = buildSitemap(movies);
    fs.writeFileSync(OUTPUT_PATH, sitemap, "utf-8");
    console.log(`\n✅ Sitemap generated: ${OUTPUT_PATH}`);
    console.log(`   ${4 + movies.length} URLs total (4 static + ${movies.length} movies)`);
  } catch (err) {
    console.error("❌ Failed to generate sitemap:", err);
    process.exit(1);
  }
}

main();
