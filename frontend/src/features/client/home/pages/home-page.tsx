import { HeroCarousel } from "@/features/client/home/components/hero-carousel";
import { MovieSection } from "@/features/client/home/components/movie-section";
import { ComboCarousel } from "@/features/client/home/components/combo-carousel";
import { CinemaExperienceSection } from "@/features/client/home/components/cinema-experience-section";
import { MovieStatus } from "@/types/enum";
import { Seo } from "@/components/common/seo";
import { BRAND } from "@/config/brand";
import { ROUTES } from "@/config/routes";

const homeStructuredData = {
  "@context": "https://schema.org",
  "@type": "WebSite",
  name: BRAND.name,
  url: BRAND.siteUrl,
  description: BRAND.defaultDescription,
  potentialAction: {
    "@type": "SearchAction",
    target: {
      "@type": "EntryPoint",
      urlTemplate: `${BRAND.siteUrl}/movies?search={search_term_string}`,
    },
    "query-input": "required name=search_term_string",
  },
};

const HomePage = () => {
  return (
    <div>
      <Seo
        title={`Lịch chiếu và đặt vé — ${BRAND.name}`}
        description={`${BRAND.name} — đặt vé xem phim trực tuyến, chọn ghế realtime, nhận vé điện tử ngay.`}
        canonical="/"
        structuredData={homeStructuredData}
      />
      <HeroCarousel />

      <div className="mx-auto max-w-7xl space-y-14 px-4 pb-14 sm:px-6 lg:px-8">

        <MovieSection
          title="Phim đang chiếu"
          status={MovieStatus.NowShowing}
          viewAllHref={`${ROUTES.movies.list}?status=${MovieStatus.NowShowing}`}
          emptyMessage="Chưa có phim đang chiếu"
        />

        <MovieSection
          title="Phim sắp chiếu"
          status={MovieStatus.ComingSoon}
          viewAllHref={`${ROUTES.movies.list}?status=${MovieStatus.ComingSoon}`}
          emptyMessage="Chưa có phim sắp chiếu"
        />

        <ComboCarousel />

        <CinemaExperienceSection />
      </div>
    </div>
  );
}

export default HomePage