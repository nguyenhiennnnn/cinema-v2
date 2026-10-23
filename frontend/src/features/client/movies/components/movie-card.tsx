import { Link } from "react-router";
import { Star } from "lucide-react";
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
  const genreText = movie.genres.slice(0, 2).join(", ");
  const durationText = formatDuration(movie.durationMinutes);

  return (
    <div
      className={cn(
        "group relative flex flex-col cut-md bg-card transition-colors duration-150",
        className,
      )}
    >
      <Link
        to={href}
        className="relative block aspect-2/3 w-full overflow-hidden cut-md bg-muted"
        aria-label={movie.title}
      >
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

        {movie.ageRating?.code && (
          <div className="pointer-events-none absolute top-2 left-2">
            <AgeRatingBadge code={movie.ageRating.code} onImage />
          </div>
        )}
      </Link>

      <div className="flex flex-1 flex-col justify-between p-3.5">
        <div>
          <h3 className="t-h3 text-sm text-foreground line-clamp-2 group-hover:underline">
            <Link to={href}>{movie.title}</Link>
          </h3>
          <p className="mt-1 line-clamp-1 text-[13px] text-muted-foreground">
            {genreText ? `${genreText}, ${durationText}` : durationText}
          </p>
        </div>

        {movie.averageRating > 0 && (
          <div className="mt-2.5 flex items-center gap-1 text-xs font-semibold text-accent">
            <Star className="size-3.5 fill-accent text-accent" />
            <span className="t-numeral text-sm font-bold">{movie.averageRating.toFixed(1)}</span>
          </div>
        )}
      </div>
    </div>
  );
};
