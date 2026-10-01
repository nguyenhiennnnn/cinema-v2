import { Link } from "react-router";
import { Clock, Star, Ticket } from "lucide-react";
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
        "group relative flex flex-col overflow-hidden border border-border bg-card transition-colors duration-200 hover:border-primary",
        className,
      )}
    >
      <div className="relative aspect-2/3 w-full overflow-hidden bg-muted">
        {movie.posterUrl ? (
          <img
            src={movie.posterUrl}
            alt={movie.title}
            loading="lazy"
            className="h-full w-full object-cover transition-transform duration-300 ease-out group-hover:scale-[1.03]"
          />
        ) : (
          <div className="flex h-full w-full items-center justify-center text-muted-foreground">
            <Ticket className="size-8" />
          </div>
        )}

        {/* Scrim: chỉ để đảm bảo chữ/badge đọc được trên ảnh, không phải trang trí */}
        <div className="pointer-events-none absolute inset-0 bg-linear-to-t from-black/85 via-black/5 to-transparent" />

        <div className="pointer-events-none absolute top-2.5 left-2.5 flex flex-wrap items-center gap-1.5">
          <AgeRatingBadge code={movie.ageRating?.code} onImage className="text-[10px]" />
          {movie.genres.slice(0, 2).map((genre) => (
            <span
              key={genre}
              className="rounded-sm border border-white/10 bg-black/60 px-1.5 py-0.5 text-[10px] font-medium text-white backdrop-blur-sm"
            >
              {genre}
            </span>
          ))}
        </div>

        {movie.averageRating > 0 && (
          <div className="pointer-events-none absolute top-2.5 right-2.5 flex items-center gap-1 rounded-sm border border-white/10 bg-black/60 px-1.5 py-0.5 text-xs font-semibold text-accent backdrop-blur-sm">
            <Star className="size-3 fill-accent text-accent" />
            <span className="tabular">{movie.averageRating.toFixed(1)}</span>
          </div>
        )}

        <div className="pointer-events-none absolute bottom-2.5 left-2.5 flex items-center gap-1 text-xs font-medium text-white/90">
          <Clock className="size-3.5" />
          <span className="tabular">{formatDuration(movie.durationMinutes)}</span>
        </div>

        <Link
          to={href}
          className="clip-corner absolute inset-x-3 bottom-3 z-10 translate-y-2 bg-primary py-2 text-center text-xs font-semibold tracking-wide text-primary-foreground uppercase opacity-0 transition-all duration-200 group-hover:translate-y-0 group-hover:opacity-100 hover:bg-primary/90"
        >
          Mua vé
        </Link>

        <Link to={href} className="absolute inset-0" aria-label={movie.title} />
      </div>

      <div className="p-3.5">
        <h3 className="font-heading text-sm font-semibold text-foreground line-clamp-1 transition-colors group-hover:text-primary">
          <Link to={href}>{movie.title}</Link>
        </h3>
        <p className="mt-1 line-clamp-1 text-xs text-muted-foreground">{movie.genres.join(", ")}</p>
      </div>
    </div>
  );
};