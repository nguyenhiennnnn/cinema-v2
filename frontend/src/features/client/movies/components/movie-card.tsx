import { Link } from "react-router";
import { Clock, Star } from "lucide-react";
import { AgeRatingBadge } from "@/components/common/status-badge";
import { formatDuration } from "@/lib/cinema-format";
import { cn } from "@/lib/utils";
import type { MovieListItemDto } from "@/features/client/movies/types";

interface MovieCardProps {
  movie: MovieListItemDto;
  className?: string;
}

export const MovieCard = ({ movie, className }: MovieCardProps) => {
  const href = `/movies/${movie.movieId}`;

  return (
    <div
      className={cn(
        "group relative flex flex-col overflow-hidden cut-md bg-card transition-colors duration-150",
        className,
      )}
    >
      <Link to={href} className="relative block aspect-2/3 w-full overflow-hidden bg-muted" aria-label={movie.title}>
        {movie.posterUrl ? (
          <img
            src={movie.posterUrl}
            alt={movie.title}
            loading="lazy"
            className="h-full w-full object-cover"
          />
        ) : (
          <div className="flex h-full w-full items-center justify-center text-muted-foreground">
            <span className="t-code text-2xl">?</span>
          </div>
        )}

        {/* Scrim tối nhẹ để badge đọc được */}
        <div className="pointer-events-none absolute inset-0" style={{ background: "linear-gradient(to top, rgba(0,0,0,0.7) 0%, rgba(0,0,0,0) 50%)" }} />

        <div className="pointer-events-none absolute top-2 left-2 flex flex-wrap items-center gap-1">
          <AgeRatingBadge code={movie.ageRating?.code} onImage className="text-[10px]" />
          {movie.genres.slice(0, 1).map((genre) => (
            <span
              key={genre}
              className="cut-xs border border-white/15 bg-black/60 px-1.5 py-0.5 text-[10px] font-medium text-white/90"
            >
              {genre}
            </span>
          ))}
        </div>

        {movie.averageRating > 0 && (
          <div className="pointer-events-none absolute top-2 right-2 flex items-center gap-1 cut-xs border border-white/15 bg-black/60 px-1.5 py-0.5 text-xs font-semibold text-accent">
            <Star className="size-3 fill-accent text-accent" />
            <span className="tabular-nums">{movie.averageRating.toFixed(1)}</span>
          </div>
        )}

        <div className="pointer-events-none absolute bottom-2 left-2 flex items-center gap-1 text-xs text-white/80">
          <Clock className="size-3" />
          <span className="tabular-nums">{formatDuration(movie.durationMinutes)}</span>
        </div>
      </Link>

      <div className="p-3">
        <h3 className="t-h3 text-sm text-foreground line-clamp-2 group-hover:underline">
          <Link to={href}>{movie.title}</Link>
        </h3>
        <p className="mt-1 line-clamp-1 text-xs text-muted-foreground">{movie.genres.join(", ")}</p>
      </div>
    </div>
  );
};
