import type { CSSProperties } from "react";
import { Link } from "react-router";
import { Info, Star, Ticket } from "lucide-react";
import { AgeRatingBadge } from "@/components/common/status-badge";
import { formatDuration } from "@/lib/cinema-format";
import { cn } from "@/lib/utils";
import type { MovieListItemDto } from "@/features/client/movies/types";
import type { HeroDirection } from "@/features/client/home/hooks/use-hero-carousel";

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
        (leaving || entering) && "will-change-[transform,opacity]",
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
