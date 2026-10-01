import { HeroCarousel } from "@/features/client/home/components/hero-carousel";
import { MovieSection } from "@/features/client/home/components/movie-section";
import { ComboCarousel } from "@/features/client/home/components/combo-carousel";
import { CinemaExperienceSection } from "@/features/client/home/components/cinema-experience-section";
import { MovieStatus } from "@/types/enum";

const HomePage = () => {
  return (
    <div>
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