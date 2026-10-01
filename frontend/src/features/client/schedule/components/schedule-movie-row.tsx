import { Link } from "react-router";
import { Ticket } from "lucide-react";
import { ShowtimeGroupList } from "@/components/common/showtime-group-list";
import type { WholeCinemaMovieDto } from "@/features/client/showtimes/types";

interface ScheduleMovieRowProps {
  movie: WholeCinemaMovieDto;
}

export const ScheduleMovieRow = ({ movie }: ScheduleMovieRowProps) => {
  return (
    <div className="flex flex-col gap-4 rounded-xl border border-border/60 bg-card p-4 sm:flex-row">
      <Link to={`/movies/${movie.movieId}`} className="mx-auto aspect-2/3 w-28 shrink-0 overflow-hidden rounded-lg bg-muted sm:mx-0">
        {movie.moviePosterUrl ? (
          <img src={movie.moviePosterUrl} alt={movie.movieTitle} className="h-full w-full object-cover" />
        ) : (
          <div className="flex h-full w-full items-center justify-center text-muted-foreground">
            <Ticket className="size-6" />
          </div>
        )}
      </Link>

      <div className="flex-1">
        <Link to={`/movies/${movie.movieId}`} className="font-heading text-base font-semibold text-foreground hover:text-primary md:text-lg">
          {movie.movieTitle}
        </Link>

        <div className="mt-3">
          <ShowtimeGroupList
            showtimes={movie.showtimes}
          />
        </div>
      </div>
    </div>
  );
}