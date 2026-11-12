import { useRef } from "react";
import type { CSSProperties } from "react";
import { Skeleton } from "@/components/ui/skeleton";
import { cn } from "@/lib/utils";
import { useMovies } from "@/features/client/movies/hooks/use-movies";
import { useHeroCarousel } from "@/features/client/home/hooks/use-hero-carousel";
import { HeroThumbnails } from "@/features/client/home/components/hero-thumbnails";
import { MovieStatus } from "@/types/enum";
import { HERO_CAROUSEL_MOVIES_LIMIT, HERO_INTERVAL_MS } from "@/config/default-constants";
import { usePreloadPosters } from "../hooks/use-preload-posters";
import { useHeroTransition } from "../hooks/use-hero-transition";
import { HeroBackdrop } from "./hero-backdrop";
import { HeroControls } from "./hero-controls";
import { HeroSlide } from "./hero-slide";

const HERO_HEIGHT = "min-h-[min(calc(100svh-4rem),56rem)]";

export const HeroCarousel = () => {
  const { data, isLoading } = useMovies({ status: MovieStatus.NowShowing, limit: HERO_CAROUSEL_MOVIES_LIMIT });
  const movies = data?.pages ?? [];
  const count = movies.length;

  usePreloadPosters(movies);

  const {
    index,
    leaving,
    direction,
    goTo,
    reducedMotion,
    canAutoplay,
    paused,
    userPaused,
    toggleUserPaused,
    pauseHandlers,
  } = useHeroCarousel(count);

  const heroRef = useRef<HTMLElement>(null);
  const backdropRef = useRef<HTMLDivElement>(null);
  const stripRef = useRef<HTMLDivElement>(null);
  const touchStartX = useRef<number | null>(null);

  const { registerThumb, requestMove } = useHeroTransition({
    movies,
    index,
    leaving,
    reducedMotion,
    goTo,
    heroRef,
    backdropRef,
    stripRef,
  });

  const next = () => requestMove((index + 1) % count, "next");
  const prev = () => requestMove((index - 1 + count) % count, "prev");

  if (isLoading) {
    return <Skeleton className={cn(HERO_HEIGHT, "w-full")} />;
  }

  if (count === 0) return null;

  const current = movies[index];
  const leavingMovie = leaving && !reducedMotion ? movies[leaving.index] : undefined;
  const thumbs = Array.from({ length: count - 1 }, (_, k) => {
    const i = (index + 1 + k) % count;
    return { movie: movies[i], index: i };
  });

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === "ArrowRight") next();
    else if (e.key === "ArrowLeft") prev();
  };

  const handleTouchEnd = (e: React.TouchEvent) => {
    const start = touchStartX.current;
    touchStartX.current = null;
    if (start === null) return;
    const dx = e.changedTouches[0].clientX - start;
    if (Math.abs(dx) < 50) return;
    if (dx < 0) next();
    else prev();
  };

  return (
    <section
      ref={heroRef}
      aria-roledescription="carousel"
      aria-label="Phim đang chiếu"
      className={cn("relative isolate touch-pan-y overflow-hidden bg-scrim", HERO_HEIGHT)}
      onKeyDown={handleKeyDown}
      onTouchStart={(e) => (touchStartX.current = e.touches[0].clientX)}
      onTouchEnd={handleTouchEnd}
      {...pauseHandlers}
    >
      {leavingMovie && <HeroBackdrop key={`bg-${leavingMovie.movieId}`} movie={leavingMovie} />}
      <HeroBackdrop
        key={`bg-${current.movieId}`}
        ref={backdropRef}
        movie={current}
        priority={direction === null}
        animated={!!leaving}
      />

      <div className={cn("relative z-10 mx-auto flex max-w-7xl flex-col px-4 py-8 sm:px-6 lg:px-8 lg:py-12", HERO_HEIGHT)}>
        <div className="grid flex-1" aria-live={canAutoplay && !paused ? "off" : "polite"}>
          {leavingMovie && leaving && (
            <HeroSlide
              key={`slide-${leavingMovie.movieId}`}
              movie={leavingMovie}
              position={leaving.index + 1}
              total={count}
              phase="leave"
              direction={leaving.direction}
            />
          )}
          <HeroSlide
            key={`slide-${current.movieId}`}
            movie={current}
            position={index + 1}
            total={count}
            phase={direction === null ? null : "enter"}
            direction={direction}
          />
        </div>

        {count > 1 && (
          <div className="mt-8 flex flex-wrap items-end justify-between gap-4">
            <HeroControls
              index={index}
              count={count}
              canAutoplay={canAutoplay}
              userPaused={userPaused}
              onPrev={prev}
              onNext={next}
              onTogglePause={toggleUserPaused}
            />

            <HeroThumbnails ref={stripRef} items={thumbs} onSelect={(i) => requestMove(i, "next")} registerThumb={registerThumb} />
          </div>
        )}
      </div>

      {canAutoplay && (
        <div
          key={`progress-${index}`}
          aria-hidden="true"
          data-paused={paused}
          style={{ "--hero-interval": `${HERO_INTERVAL_MS}ms` } as CSSProperties}
          onAnimationEnd={(e) => {
            if (e.animationName === "hero-progress") next();
          }}
          className="hero-progress absolute inset-x-0 bottom-0 z-20 h-[3px] bg-primary/80"
        />
      )}
    </section>
  );
};