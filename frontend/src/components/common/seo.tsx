import { Helmet } from "react-helmet-async";
import { BRAND } from "@/config/brand";

const DEFAULT_TITLE = `${BRAND.name} — ${BRAND.tagline}`;

interface SeoProps {
  /** Tiêu đề trang (sẽ được thêm " | Perfo" tự động nếu không có) */
  title?: string;
  /** Mô tả trang */
  description?: string;
  /** URL canonical của trang */
  canonical?: string;
  /** URL ảnh Open Graph */
  ogImage?: string;
  /** Loại Open Graph: website | article */
  ogType?: "website" | "article";
  /** Không index trang này */
  noIndex?: boolean;
  /** JSON-LD Structured Data */
  structuredData?: object;
}

/**
 * Component SEO dùng react-helmet-async để inject meta tags vào <head>.
 * Sử dụng trong từng page component để tuỳ chỉnh SEO per-page.
 *
 * @example
 * <Seo
 *   title="Danh sách phim"
 *   description="Khám phá tất cả phim đang và sắp chiếu tại rạp."
 *   canonical="/movies"
 * />
 */
export function Seo({
  title,
  description = BRAND.defaultDescription,
  canonical,
  ogImage = BRAND.defaultOgImage,
  ogType = "website",
  noIndex = false,
  structuredData,
}: SeoProps) {
  const pageTitle = title ? `${title} | ${BRAND.siteName}` : DEFAULT_TITLE;
  const canonicalUrl = canonical ? `${BRAND.siteUrl}${canonical}` : undefined;
  const ogImageUrl = ogImage.startsWith("http")
    ? ogImage
    : `${BRAND.siteUrl}${ogImage}`;

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
      <meta property="og:site_name" content={BRAND.siteName} />

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
