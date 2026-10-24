import { Link } from "react-router";
import { Film } from "lucide-react";
import { MovieCard } from "@/features/client/movies/components/movie-card";
import { MovieCardSkeleton } from "@/features/client/movies/components/movie-card-skeleton";
import { EmptyState } from "@/components/common/empty-state";
import { useMovies } from "@/features/client/movies/hooks/use-movies";
import type { MovieStatus } from "@/types/enum";

interface MovieSectionProps {
  title: string;
  status: MovieStatus;
  viewAllHref: string;
  emptyMessage: string;
}

export const MovieSection = ({ title, status, viewAllHref, emptyMessage }: MovieSectionProps) => {
  const { data, isLoading } = useMovies({ status, limit: 10 });
  const movies = data?.pages ?? [];

  return (
    <section className="mt-10">
      <div className="flex items-end justify-between gap-3 border-b border-border pb-3">
        <h2 className="t-h2 text-foreground">{title}</h2>
        <Link
          to={viewAllHref}
          className="shrink-0 text-sm font-medium text-primary hover:underline"
        >
          Xem tất cả
        </Link>
      </div>

      <div className="mt-5">
        {isLoading ? (
          <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5">
            {Array.from({ length: 5 }).map((_, i) => (
              <MovieCardSkeleton key={i} />
            ))}
          </div>
        ) : movies.length === 0 ? (
          <EmptyState icon={Film} title={emptyMessage} />
        ) : (
          <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5">
            {movies.map((movie) => (
              <MovieCard key={movie.movieId} movie={movie} />
            ))}
          </div>
        )}
      </div>
    </section>
  );
};
