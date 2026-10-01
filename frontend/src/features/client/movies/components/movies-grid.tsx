import { Film } from "lucide-react";
import { MovieCard } from "@/features/client/movies/components/movie-card";
import { MovieCardSkeleton } from "@/features/client/movies/components/movie-card-skeleton";
import { EmptyState } from "@/components/common/empty-state";
import { InfiniteScrollTrigger } from "@/components/common/infinite-scroll-trigger";
import type { MovieListItemDto } from "@/features/client/movies/types";

interface MoviesGridProps {
  movies: MovieListItemDto[];
  isLoading: boolean;
  hasNextPage: boolean | undefined;
  isFetchingNextPage: boolean;
  fetchNextPage: () => void;
  onResetFilters?: () => void;
}

export const MoviesGrid = ({ movies, isLoading, hasNextPage, isFetchingNextPage, fetchNextPage, onResetFilters }: MoviesGridProps) => {
  if (isLoading) {
    return (
      <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5">
        {Array.from({ length: 10 }).map((_, i) => (
          <MovieCardSkeleton key={i} />
        ))}
      </div>
    );
  }

  if (movies.length === 0) {
    return (
      <EmptyState
        icon={Film}
        title="Không tìm thấy phim phù hợp"
        description="Hãy thử điều chỉnh lại bộ lọc hoặc từ khóa tìm kiếm."
        actionLabel={onResetFilters ? "Xóa bộ lọc" : undefined}
        onAction={onResetFilters}
      />
    );
  }

  return (
    <div className="space-y-6">
      <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5">
        {movies.map((movie) => (
          <MovieCard key={movie.movieId} movie={movie} />
        ))}
      </div>
      <InfiniteScrollTrigger hasNextPage={hasNextPage} isFetchingNextPage={isFetchingNextPage} fetchNextPage={fetchNextPage} />
    </div>
  );
}