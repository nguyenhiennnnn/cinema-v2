import type { CSSProperties } from "react";
import { Link } from "react-router";
import { Clock, Info, Star, Ticket } from "lucide-react";
import { AgeRatingBadge } from "@/components/common/status-badge";
import { formatDuration } from "@/lib/cinema-format";
import { cn } from "@/lib/utils";
import type { MovieListItemDto } from "@/features/client/movies/types";
import type { HeroDirection } from "@/features/client/home/hooks/use-hero-carousel";
import { ROUTES } from "@/config/routes";

interface HeroSlideProps {
  movie: MovieListItemDto;
  position: number;
  total: number;
  phase: "enter" | "leave" | null;
  direction: HeroDirection | null;
}

export const HeroSlide = ({ movie, position, total, phase, direction }: HeroSlideProps) => {
  const leaving = phase === "leave";
  const entering = phase === "enter";
  const sign = direction === "prev" ? -1 : 1;
  const Title = leaving ? "p" : "h1";

  const style = {
    "--hero-shift": `${32 * sign}px`,
    "--hero-exit": `${-20 * sign}px`,
  } as CSSProperties;

  const reveal = (i: number): { className?: string; style?: CSSProperties } =>
    entering ? { className: "hero-reveal", style: { "--i": i } as CSSProperties } : {};

  const metaReveal = reveal(0);
  const titleReveal = reveal(1);
  const statsReveal = reveal(2);
  const dividerReveal = reveal(3);
  const actionsReveal = reveal(4);
  const posterReveal = reveal(5);
  const href = ROUTES.movies.detail(movie.movieId);

  return (
    <article
      role="group"
      aria-roledescription="slide"
      aria-label={`${position} / ${total}`}
      aria-hidden={leaving || undefined}
      style={style}
      className={cn(
        "grid content-center [grid-area:1/1]",
        "gap-8 md:grid-cols-[minmax(0,1fr)_auto] md:items-center md:gap-12 lg:gap-20",
        (leaving || entering) && "will-change-[transform,opacity]",
        leaving && "hero-leave pointer-events-none",
      )}
    >
      <div className="min-w-0 md:max-w-lg lg:max-w-xl">

        <div
          style={metaReveal.style}
          className={cn("flex flex-wrap items-center gap-2", metaReveal.className)}
        >
          <AgeRatingBadge code={movie.ageRating?.code} onImage />
          {movie.genres.slice(0, 3).map((g) => (
            <span
              key={g}
              className="cut-xs bg-scrim-foreground/15 px-2.5 py-1 text-[13px] font-medium tracking-wide text-scrim-foreground"
            >
              {g}
            </span>
          ))}
        </div>

        <div style={titleReveal.style} className={cn("mt-5", titleReveal.className)}>
          <div className="mb-4 h-[3px] w-10 bg-primary" />
          <Title className="t-display line-clamp-3 pb-1 text-scrim-foreground">
            {movie.title}
          </Title>
        </div>

        <div
          style={statsReveal.style}
          className={cn(
            "mt-6 flex items-center gap-6",
            statsReveal.className,
          )}
        >
          <div className="flex flex-col gap-0.5">
            <span className="t-numeral text-2xl text-scrim-foreground">
              {formatDuration(movie.durationMinutes)}
            </span>
            <span className="flex items-center gap-1 text-[11px] uppercase tracking-widest text-scrim-foreground/50">
              <Clock className="size-3" />
              thời lượng
            </span>
          </div>

          {movie.averageRating > 0 && (
            <div className="h-10 w-px bg-scrim-foreground/20" aria-hidden />
          )}

          {movie.averageRating > 0 && (
            <div className="flex flex-col gap-0.5">
              <span className="flex items-baseline gap-1.5">
                <Star className="size-4 translate-y-px fill-accent text-accent" />
                <span className="t-numeral text-2xl text-accent">
                  {movie.averageRating.toFixed(1)}
                </span>
                <span className="text-[13px] text-scrim-foreground/50">/&nbsp;5</span>
              </span>
              <span className="text-[11px] uppercase tracking-widest text-scrim-foreground/50">
                đánh giá
              </span>
            </div>
          )}
        </div>

        <div
          style={dividerReveal.style}
          className={cn("mt-7 perf-h w-full max-w-[200px]", dividerReveal.className)}
          aria-hidden
        />

        <div
          style={actionsReveal.style}
          className={cn("mt-6 flex flex-wrap gap-3", actionsReveal.className)}
        >
          <Link
            to={href}
            tabIndex={leaving ? -1 : undefined}
            className="cut-sm inline-flex h-12 items-center gap-2 bg-primary px-7 text-[15px] font-semibold text-primary-foreground transition-colors duration-150 hover:bg-primary/90"
          >
            <Ticket className="size-4" />
            Đặt vé
          </Link>
          <Link
            to={href}
            tabIndex={leaving ? -1 : undefined}
            className="cut-sm inline-flex h-12 items-center gap-2 bg-scrim-foreground/12 px-7 text-[15px] font-medium text-scrim-foreground transition-colors duration-150 hover:bg-scrim-foreground/22"
          >
            <Info className="size-4" />
            Chi tiết phim
          </Link>
        </div>
      </div>

      <Link
        to={href}
        tabIndex={leaving ? -1 : undefined}
        aria-label={movie.title}
        style={posterReveal.style}
        className={cn(
          "block aspect-2/3 w-32 cut-md bg-muted sm:w-40",
          "md:order-last md:h-[clamp(16rem,calc(100svh-16rem),32rem)] md:w-auto",
          "transition-opacity duration-150 hover:opacity-90",
          posterReveal.className,
        )}
      >
        {movie.posterUrl ? (
          <img
            src={movie.posterUrl}
            alt=""
            decoding="async"
            className="h-full w-full object-cover"
          />
        ) : (
          <span className="t-code flex h-full w-full items-center justify-center text-2xl text-muted-foreground">
            ?
          </span>
        )}
      </Link>
    </article>
  );
};
