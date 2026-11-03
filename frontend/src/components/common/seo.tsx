import { Helmet } from "react-helmet-async";
import { BRAND } from "@/config/brand";

const DEFAULT_TITLE = `${BRAND.name} — ${BRAND.tagline}`;

function toAbsoluteUrl(pathOrUrl: string): string {
  if (/^https?:\/\//i.test(pathOrUrl)) return pathOrUrl;
  return `${BRAND.siteUrl}${pathOrUrl.startsWith("/") ? "" : "/"}${pathOrUrl}`;
}

interface SeoProps {
  title?: string;
  description?: string;
  canonical?: string;
  ogImage?: string;
  ogType?: "website" | "article";
  noIndex?: boolean;
  structuredData?: object;
}

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
  const canonicalUrl =
    canonical && BRAND.siteUrl
      ? toAbsoluteUrl(canonical.split(/[?#]/)[0])
      : undefined;
  const ogImageUrl = toAbsoluteUrl(ogImage);

  return (
    <Helmet>
      <title>{pageTitle}</title>
      <meta name="description" content={description} />
      {noIndex && <meta name="robots" content="noindex, nofollow" />}
      {canonicalUrl && <link rel="canonical" href={canonicalUrl} />}

      <meta property="og:title" content={pageTitle} />
      <meta property="og:description" content={description} />
      <meta property="og:type" content={ogType} />
      <meta property="og:image" content={ogImageUrl} />
      {canonicalUrl && <meta property="og:url" content={canonicalUrl} />}
      <meta property="og:site_name" content={BRAND.siteName} />
      <meta property="og:locale" content={BRAND.locale} />
      <meta property="og:image:alt" content={pageTitle} />

      <meta name="twitter:card" content="summary_large_image" />
      <meta name="twitter:title" content={pageTitle} />
      <meta name="twitter:description" content={description} />
      <meta name="twitter:image" content={ogImageUrl} />

      {structuredData && (
        <script type="application/ld+json">
          {JSON.stringify(structuredData)}
        </script>
      )}
    </Helmet>
  );
}
