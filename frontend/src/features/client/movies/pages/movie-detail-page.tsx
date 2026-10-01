import { useParams } from "react-router";
import { MovieHero } from "@/features/client/movies/components/movie-hero";
import { MovieCastCrew } from "@/features/client/movies/components/movie-cast-crew";
import { MovieShowtimeSelector } from "@/features/client/movies/components/movie-showtime-selector";
import { MovieReviews } from "@/features/client/movies/components/movie-reviews";
import { ExpandableText } from "@/components/common/expandable-text";
import { useMovieDetail } from "@/features/client/movies/hooks/use-movies";
import { Skeleton } from "@/components/ui/skeleton";

const MovieDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const movieId = Number(id);

  const { data: movie, isLoading } = useMovieDetail(movieId);

  if (isLoading || !movie) {
    return (
      <div className="mx-auto max-w-6xl space-y-6 px-4 py-8 sm:px-6 lg:px-8">
        <Skeleton className="h-96 w-full rounded-2xl" />
        <Skeleton className="h-40 w-full rounded-2xl" />
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-6xl space-y-10 px-4 py-8 sm:px-6 lg:px-8">
      <MovieHero movie={movie} />

      <div className="grid gap-10 lg:grid-cols-3">
        <div className="space-y-8 lg:col-span-2">
          {movie.description && (
            <div>
              <h2 className="font-heading text-2xl font-semibold tracking-tight text-foreground uppercase md:text-3xl">Nội dung phim</h2>
              <ExpandableText text={movie.description} className="mt-3" />
            </div>
          )}
          <MovieCastCrew director={movie.director} cast={movie.cast} />
        </div>
      </div>

      <MovieShowtimeSelector showtimes={movie.upcomingShowtimes} />

      <MovieReviews movieId={movie.movieId} />
    </div>
  );
}

export default MovieDetailPage