import { Helmet } from "react-helmet-async";

const SITE_NAME = "Perfo";
const DEFAULT_TITLE = "Perfo — ─Éß║╖t v├⌐ xem phim trß╗▒c tuyß║┐n";
const DEFAULT_DESCRIPTION =
  "─Éß║╖t v├⌐ xem phim nhanh ch├│ng: chß╗ìn phim, suß║Ñt chiß║┐u, chß╗ìn ghß║┐ v├á thanh to├ín online chß╗ë trong v├ái ph├║t.";
const DEFAULT_OG_IMAGE = "/favicon.svg";
const SITE_URL = import.meta.env.VITE_SITE_URL ?? "";

interface SeoProps {
  /** Ti├¬u ─æß╗ü trang (sß║╜ ─æ╞░ß╗úc th├¬m " | Perfo" tß╗▒ ─æß╗Öng nß║┐u kh├┤ng c├│) */
  title?: string;
  /** M├┤ tß║ú trang */
  description?: string;
  /** URL canonical cß╗ºa trang */
  canonical?: string;
  /** URL ß║únh Open Graph */
  ogImage?: string;
  /** Loß║íi Open Graph: website | article */
  ogType?: "website" | "article";
  /** Kh├┤ng index trang n├áy */
  noIndex?: boolean;
  /** JSON-LD Structured Data */
  structuredData?: object;
}

/**
 * Component SEO d├╣ng react-helmet-async ─æß╗â inject meta tags v├áo <head>.
 * Sß╗¡ dß╗Ñng trong tß╗½ng page component ─æß╗â tuß╗│ chß╗ënh SEO per-page.
 *
 * @example
 * <Seo
 *   title="Danh s├ích phim"
 *   description="Kh├ím ph├í tß║Ñt cß║ú phim ─æang v├á sß║»p chiß║┐u tß║íi rß║íp."
 *   canonical="/movies"
 * />
 */
export function Seo({
  title,
  description = DEFAULT_DESCRIPTION,
  canonical,
  ogImage = DEFAULT_OG_IMAGE,
  ogType = "website",
  noIndex = false,
  structuredData,
}: SeoProps) {
  const pageTitle = title ? `${title} | ${SITE_NAME}` : DEFAULT_TITLE;
  const canonicalUrl = canonical ? `${SITE_URL}${canonical}` : undefined;
  const ogImageUrl = ogImage.startsWith("http")
    ? ogImage
    : `${SITE_URL}${ogImage}`;

  return (
    <Helmet>
      {/* Primary Meta */}
      <title>{pageTitle}</title>
      <meta name="description" content={description} />
      {noIndex && <meta name="robots" content="noindex, nofollow" />}
      {canonicalUrl && <link rel="canonical" href={canonicalUrl} />}

      {/* Open Graph */}
      <meta property="og:title" content={pageTitle} />
      <meta property="og:description" content={description} />
      <meta property="og:type" content={ogType} />
      <meta property="og:image" content={ogImageUrl} />
      {canonicalUrl && <meta property="og:url" content={canonicalUrl} />}
      <meta property="og:site_name" content={SITE_NAME} />

      {/* Twitter Card */}
      <meta name="twitter:card" content="summary_large_image" />
      <meta name="twitter:title" content={pageTitle} />
      <meta name="twitter:description" content={description} />
      <meta name="twitter:image" content={ogImageUrl} />

      {/* JSON-LD Structured Data */}
      {structuredData && (
        <script type="application/ld+json">
          {JSON.stringify(structuredData)}
        </script>
      )}
    </Helmet>
  );
}