import { useState } from "react";
import { Star, Clock, Languages, CalendarDays, PlayCircle, Ticket } from "lucide-react";
import { Button } from "@/components/ui/button";
import { AgeRatingBadge } from "@/components/common/status-badge";
import { TrailerDialog } from "@/features/client/movies/components/trailer-dialog";
import { formatDuration } from "@/lib/cinema-format";
import { formatDate } from "@/lib/formatters";
import type { MovieDetailDto } from "@/features/client/movies/types";

interface MovieHeroProps {
  movie: MovieDetailDto;
}

export const MovieHero = ({ movie }: MovieHeroProps) => {
  const [trailerOpen, setTrailerOpen] = useState(false);

  return (
    <div className="relative overflow-hidden bg-card cut-md">
      {movie.posterUrl && (
        <div className="absolute inset-0 overflow-hidden">
          <img src={movie.posterUrl} alt="" className="h-full w-full scale-110 object-cover opacity-10" />
          <div className="absolute inset-0" style={{ background: "linear-gradient(to top, var(--color-card) 30%, color-mix(in oklch, var(--color-card) 80%, transparent) 70%, color-mix(in oklch, var(--color-card) 50%, transparent) 100%)" }} />
        </div>
      )}

      <div className="relative flex flex-col gap-6 p-5 sm:flex-row sm:p-8">
        <div className="mx-auto aspect-2/3 w-40 shrink-0 overflow-hidden cut-md sm:mx-0 sm:w-56">
          {movie.posterUrl ? (
            <img src={movie.posterUrl} alt={movie.title} className="h-full w-full object-cover" />
          ) : (
            <div className="flex h-full w-full items-center justify-center bg-secondary text-muted-foreground">
              <Ticket className="size-10" />
            </div>
          )}
        </div>

        <div className="flex flex-1 flex-col justify-end">
          <div className="flex flex-wrap items-center gap-2">
            <AgeRatingBadge code={movie.ageRating?.code} />
            {movie.genres.map((g) => (
              <span key={g.genreId} className="cut-xs border border-border bg-secondary px-2 py-0.5 text-xs font-medium text-secondary-foreground">
                {g.name}
              </span>
            ))}
          </div>

          <h1 className="mt-3 t-h1 text-foreground">{movie.title}</h1>
          {movie.originalTitle && <p className="mt-1 text-sm text-muted-foreground">{movie.originalTitle}</p>}

          <div className="mt-4 flex flex-wrap items-center gap-x-5 gap-y-2 text-sm text-muted-foreground">
            <span className="flex items-center gap-1.5">
              <Clock className="size-4" />
              {formatDuration(movie.durationMinutes)}
            </span>
            {movie.language && (
              <span className="flex items-center gap-1.5">
                <Languages className="size-4" />
                {movie.language.name}
              </span>
            )}
            {movie.releaseDate && (
              <span className="flex items-center gap-1.5">
                <CalendarDays className="size-4" />
                {formatDate(movie.releaseDate)}
              </span>
            )}
            {movie.averageRating > 0 && (
              <span className="flex items-center gap-1.5 font-semibold text-accent">
                <Star className="size-4 fill-accent text-accent" />
                {movie.averageRating.toFixed(1)}/5.0
              </span>
            )}
          </div>

          <div className="mt-6 flex flex-wrap gap-3">
            <Button
              size="lg"
              onClick={() => document.getElementById("showtimes")?.scrollIntoView({ behavior: "smooth" })}
            >
              <Ticket className="size-4" />
              <span>Đặt vé</span>
            </Button>
            {movie.trailerUrl && (
              <Button variant="outline" size="lg" onClick={() => setTrailerOpen(true)}>
                <PlayCircle className="size-4" />
                <span>Xem trailer</span>
              </Button>
            )}
          </div>
        </div>
      </div>

      {movie.trailerUrl && (
        <TrailerDialog open={trailerOpen} onOpenChange={setTrailerOpen} trailerUrl={movie.trailerUrl} title={movie.title} />
      )}
    </div>
  );
}
