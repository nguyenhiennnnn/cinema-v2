import { HeroCarousel } from "@/features/client/home/components/hero-carousel";
import { MovieSection } from "@/features/client/home/components/movie-section";
import { ComboCarousel } from "@/features/client/home/components/combo-carousel";
import { CinemaExperienceSection } from "@/features/client/home/components/cinema-experience-section";
import { MovieStatus } from "@/types/enum";
import { Seo } from "@/components/common/seo";

const homeStructuredData = {
  "@context": "https://schema.org",
  "@type": "WebSite",
  name: "Perfo",
  url: import.meta.env.VITE_SITE_URL ?? "",
  description:
    "Đặt vé xem phim nhanh chóng: chọn phim, suất chiếu, chọn ghế và thanh toán online chỉ trong vài phút.",
  potentialAction: {
    "@type": "SearchAction",
    target: {
      "@type": "EntryPoint",
      urlTemplate: `${import.meta.env.VITE_SITE_URL ?? ""}/movies?search={search_term_string}`,
    },
    "query-input": "required name=search_term_string",
  },
};

const HomePage = () => {
  return (
    <div>
      <Seo
        title="Lịch chiếu và đặt vé — Perfo"
        description="Perfo — đặt vé xem phim trực tuyến, chọn ghế realtime, nhận vé điện tử ngay."
        canonical="/"
        structuredData={homeStructuredData}
      />
      <HeroCarousel />

      <div className="mx-auto max-w-7xl space-y-14 px-4 pb-14 sm:px-6 lg:px-8">

        <MovieSection
          title="Phim đang chiếu"
          status={MovieStatus.NowShowing}
          viewAllHref="/movies?status=NowShowing"
          emptyMessage="Chưa có phim đang chiếu"
        />

        <MovieSection
          title="Phim sắp chiếu"
          status={MovieStatus.ComingSoon}
          viewAllHref="/movies?status=ComingSoon"
          emptyMessage="Chưa có phim sắp chiếu"
        />

        <ComboCarousel />

        <CinemaExperienceSection />
      </div>
    </div>
  );
}

export default HomePage