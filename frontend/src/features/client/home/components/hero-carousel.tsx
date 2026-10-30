import { useCallback, useLayoutEffect, useRef } from "react";
import type { CSSProperties, Ref } from "react";
import { Link } from "react-router";
import { ChevronLeft, ChevronRight, Info, Pause, Play, Star, Ticket } from "lucide-react";
import { AgeRatingBadge } from "@/components/common/status-badge";
import { Skeleton } from "@/components/ui/skeleton";
import { formatDuration } from "@/lib/cinema-format";
import { cn } from "@/lib/utils";
import { useMovies } from "@/features/client/movies/hooks/use-movies";
import type { MovieListItemDto } from "@/features/client/movies/types";
import {
  HERO_INTERVAL_MS,
  HERO_TRANSITION_MS,
  useHeroCarousel,
  type HeroDirection,
} from "@/features/client/home/hooks/use-hero-carousel";
import { HeroThumbnails } from "@/features/client/home/components/hero-thumbnails";
import { MovieStatus } from "@/types/enum";

const HERO_HEIGHT = "min-h-[min(calc(100svh-4rem),52rem)]";
const EASING = "cubic-bezier(0.2, 0.8, 0.2, 1)";
const THUMB_CUT = 6;

interface Inset {
  top: number;
  right: number;
  bottom: number;
  left: number;
}

function measureInset(hero: DOMRect, el: DOMRect): Inset {
  return {
    top: el.top - hero.top,
    left: el.left - hero.left,
    right: hero.right - el.right,
    bottom: hero.bottom - el.bottom,
  };
}

function buildClipFrames(width: number, height: number, o: Inset) {
  const c = THUMB_CUT;
  const x1 = o.left;
  const y1 = o.top;
  const x2 = width - o.right;
  const y2 = height - o.bottom;
  const from = `polygon(${x1 + c}px ${y1}px, ${x2}px ${y1}px, ${x2}px ${y2 - c}px, ${x2 - c}px ${y2}px, ${x1}px ${y2}px, ${x1}px ${y1 + c}px)`;
  const to = `polygon(0px 0px, ${width}px 0px, ${width}px ${height}px, ${width}px ${height}px, 0px ${height}px, 0px 0px)`;
  return [{ clipPath: from }, { clipPath: to }];
}

interface HeroBackdropProps {
  movie: MovieListItemDto;
  priority?: boolean;
  ref?: Ref<HTMLDivElement>;
}

const HeroBackdrop = ({ movie, priority = false, ref }: HeroBackdropProps) => (
  <div ref={ref} className="absolute inset-0" aria-hidden="true">
    {movie.posterUrl && (
      <img
        src={movie.posterUrl}
        alt=""
        decoding="async"
        fetchPriority={priority ? "high" : undefined}
        className="h-full w-full object-cover"
      />
    )}
    <div className="absolute inset-0 bg-scrim/70" />
  </div>
);

interface HeroSlideProps {
  movie: MovieListItemDto;
  position: number;
  total: number;
  phase: "enter" | "leave" | null;
  direction: HeroDirection | null;
}

const HeroSlide = ({ movie, position, total, phase, direction }: HeroSlideProps) => {
  const leaving = phase === "leave";
  const entering = phase === "enter";
  const sign = direction === "prev" ? -1 : 1;
  const Title = leaving ? "p" : "h1";

  const style = {
    "--hero-shift": `${24 * sign}px`,
    "--hero-exit": `${-24 * sign}px`,
  } as CSSProperties;

  const reveal = (i: number): { className?: string; style?: CSSProperties } =>
    entering ? { className: "hero-reveal", style: { "--i": i } as CSSProperties } : {};

  const posterReveal = reveal(0);
  const metaReveal = reveal(1);
  const titleReveal = reveal(2);
  const infoReveal = reveal(3);
  const actionsReveal = reveal(4);
  const href = `/movies/${movie.movieId}`;

  return (
    <article
      role="group"
      aria-roledescription="slide"
      aria-label={`${position} / ${total}`}
      aria-hidden={leaving || undefined}
      style={style}
      className={cn(
        "grid content-end gap-6 [grid-area:1/1] md:grid-cols-[minmax(0,1fr)_auto] md:items-end md:gap-12",
        leaving && "hero-leave pointer-events-none",
      )}
    >
      <Link
        to={href}
        tabIndex={leaving ? -1 : undefined}
        aria-label={movie.title}
        style={posterReveal.style}
        className={cn(
          "block aspect-2/3 w-28 cut-md bg-muted sm:w-36 md:order-last md:h-[clamp(12rem,calc(100svh-19rem),28rem)] md:w-auto",
          posterReveal.className,
        )}
      >
        {movie.posterUrl ? (
          <img src={movie.posterUrl} alt="" decoding="async" className="h-full w-full object-cover" />
        ) : (
          <span className="t-code flex h-full w-full items-center justify-center text-2xl text-muted-foreground">?</span>
        )}
      </Link>

      <div className="min-w-0 md:max-w-2xl">
        <div style={metaReveal.style} className={cn("flex flex-wrap items-center gap-2", metaReveal.className)}>
          <AgeRatingBadge code={movie.ageRating?.code} onImage />
          {movie.genres.slice(0, 3).map((g) => (
            <span key={g} className="cut-xs bg-scrim-foreground/15 px-2 py-0.5 text-[13px] font-medium text-scrim-foreground">
              {g}
            </span>
          ))}
        </div>

        <Title
          style={titleReveal.style}
          className={cn(
            "mt-3 line-clamp-3 pb-1 t-h1 text-scrim-foreground md:text-5xl lg:text-6xl",
            titleReveal.className,
          )}
        >
          {movie.title}
        </Title>

        <div
          style={infoReveal.style}
          className={cn("mt-4 flex items-center gap-4 text-sm text-scrim-foreground/70", infoReveal.className)}
        >
          <span>{formatDuration(movie.durationMinutes)}</span>
          {movie.averageRating > 0 && (
            <span className="flex items-center gap-1 font-semibold text-accent">
              <Star className="size-4 fill-accent text-accent" />
              <span className="t-numeral text-base">{movie.averageRating.toFixed(1)}</span>
            </span>
          )}
        </div>

        <div style={actionsReveal.style} className={cn("mt-6 flex flex-wrap gap-3", actionsReveal.className)}>
          <Link
            to={href}
            tabIndex={leaving ? -1 : undefined}
            className="cut-sm inline-flex h-11 items-center gap-2 bg-primary px-5 text-sm font-medium text-primary-foreground hover:bg-primary/90"
          >
            <Ticket className="size-4" />
            Đặt vé
          </Link>
          <Link
            to={href}
            tabIndex={leaving ? -1 : undefined}
            className="cut-sm inline-flex h-11 items-center gap-2 bg-scrim-foreground/15 px-5 text-sm font-medium text-scrim-foreground hover:bg-scrim-foreground/25"
          >
            <Info className="size-4" />
            Chi tiết phim
          </Link>
        </div>
      </div>
    </article>
  );
};

const controlClass =
  "cut-sm flex size-11 items-center justify-center bg-scrim-foreground/15 text-scrim-foreground transition-colors duration-150 hover:bg-scrim-foreground/25";

export const HeroCarousel = () => {
  const { data, isLoading } = useMovies({ status: MovieStatus.NowShowing, limit: 5 });
  const movies = data?.pages ?? [];
  const count = movies.length;

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
  const thumbRefs = useRef(new Map<number, HTMLElement>());
  const originRef = useRef<Inset | null>(null);
  const touchStartX = useRef<number | null>(null);

  const registerThumb = useCallback((movieId: number, el: HTMLElement | null) => {
    if (el) thumbRefs.current.set(movieId, el);
    else thumbRefs.current.delete(movieId);
  }, []);

  const requestMove = (target: number, dir: HeroDirection) => {
    if (leaving || count < 2 || target === index) return;
    const hero = heroRef.current;
    const targetId = movies[target]?.movieId;
    const thumb = targetId === undefined ? undefined : thumbRefs.current.get(targetId);
    originRef.current = hero && thumb ? measureInset(hero.getBoundingClientRect(), thumb.getBoundingClientRect()) : null;
    goTo(target, dir);
  };

  const next = () => requestMove((index + 1) % count, "next");
  const prev = () => requestMove((index - 1 + count) % count, "prev");

  const leavingMovieId = leaving ? movies[leaving.index]?.movieId : undefined;

  useLayoutEffect(() => {
    if (!leaving || reducedMotion) return;
    const hero = heroRef.current;
    const backdrop = backdropRef.current;
    if (!hero || !backdrop) return;

    const options = { duration: HERO_TRANSITION_MS, easing: EASING };
    const animations: Animation[] = [];

    const { width, height } = hero.getBoundingClientRect();
    const origin = originRef.current;
    animations.push(
      backdrop.animate(origin ? buildClipFrames(width, height, origin) : [{ opacity: 0 }, { opacity: 1 }], options),
    );

    const strip = stripRef.current;
    const first = strip?.children[0];
    if (strip && first) {
      const second = strip.children[1];
      const slot = second
        ? second.getBoundingClientRect().left - first.getBoundingClientRect().left
        : first.getBoundingClientRect().width + 8;
      const dx = leaving.direction === "next" ? slot : -slot;
      animations.push(strip.animate([{ transform: `translateX(${dx}px)` }, { transform: "translateX(0)" }], options));

      const leftThumb = leavingMovieId === undefined ? undefined : thumbRefs.current.get(leavingMovieId);
      if (leftThumb) animations.push(leftThumb.animate([{ opacity: 0 }, { opacity: 1 }], { ...options, fill: "backwards" }));
    }

    return () => animations.forEach((a) => a.cancel());
  }, [leaving, leavingMovieId, reducedMotion]);

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
      <HeroBackdrop key={`bg-${current.movieId}`} ref={backdropRef} movie={current} priority={direction === null} />

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
            <div className="flex items-center gap-2">
              <button type="button" onClick={prev} className={controlClass} aria-label="Phim trước">
                <ChevronLeft className="size-5" />
              </button>
              <button type="button" onClick={next} className={controlClass} aria-label="Phim tiếp theo">
                <ChevronRight className="size-5" />
              </button>
              {canAutoplay && (
                <button
                  type="button"
                  onClick={toggleUserPaused}
                  className={controlClass}
                  aria-label={userPaused ? "Tiếp tục tự chuyển phim" : "Tạm dừng tự chuyển phim"}
                >
                  {userPaused ? <Play className="size-5" /> : <Pause className="size-5" />}
                </button>
              )}
              <span className="t-code ml-2 text-sm text-scrim-foreground/70" aria-hidden="true">
                {index + 1} / {count}
              </span>
            </div>

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
          className="hero-progress absolute inset-x-0 bottom-0 z-20 h-0.5 bg-scrim-foreground/60"
        />
      )}
    </section>
  );
};
