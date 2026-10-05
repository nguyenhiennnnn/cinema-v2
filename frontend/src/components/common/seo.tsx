import { Helmet } from "react-helmet-async";

const SITE_NAME = "Cinema";
const DEFAULT_TITLE = "Cinema - Đặt vé xem phim trực tuyến";
const DEFAULT_DESCRIPTION =
  "Đặt vé xem phim nhanh chóng: chọn phim, suất chiếu, chọn ghế và thanh toán online chỉ trong vài phút.";
const DEFAULT_OG_IMAGE = "/favicon.svg";
const SITE_URL = import.meta.env.VITE_SITE_URL ?? "";

interface SeoProps {
  /** Tiêu đề trang (sẽ được thêm " | Cinema" tự động nếu không có) */
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
