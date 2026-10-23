import { useClientMovieParams } from "@/hooks/use-query-params";
import { MoviesFilterBar } from "@/features/client/movies/components/movies-filter-bar";
import { MoviesGrid } from "@/features/client/movies/components/movies-grid";
import { useMovies } from "@/features/client/movies/hooks/use-movies";
import { useDebouncedValue } from "@/hooks/use-debounced-value";
import type { MovieStatus } from "@/types/enum";
import { Seo } from "@/components/common/seo";

const MoviesPage = () => {
  const [params, setParams] = useClientMovieParams();
  const debouncedSearch = useDebouncedValue(params.search);

  const { data, isLoading, isFetchingNextPage, hasNextPage, fetchNextPage } = useMovies({
    search: debouncedSearch || undefined,
    status: (params.status as MovieStatus) || undefined,
    genreId: params.genreId ?? undefined,
    languageId: params.languageId ?? undefined,
    ageRatingId: params.ageRatingId ?? undefined,
    limit: params.limit,
  });

  return (
    <div className="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <Seo
        title="Danh sách phim"
        description="Khám phá tất cả các bộ phim đang và sắp chiếu tại Perfo. Lọc theo thể loại, ngôn ngữ và nhiều tiêu chí khác."
        canonical="/movies"
      />
      <div className="mb-6">
        <h1 className="font-heading text-2xl font-bold tracking-tight text-foreground md:text-3xl">Danh sách phim</h1>
        <p className="mt-1 text-sm text-muted-foreground">Khám phá tất cả các bộ phim đang và sắp chiếu tại rạp.</p>
      </div>

      <MoviesFilterBar params={params} setParams={setParams} />

      <div className="mt-6">
        <MoviesGrid
          movies={data?.pages ?? []}
          isLoading={isLoading}
          hasNextPage={hasNextPage}
          isFetchingNextPage={isFetchingNextPage}
          fetchNextPage={fetchNextPage}
          onResetFilters={() => setParams({ search: "", status: null, genreId: null, languageId: null, ageRatingId: null, cursor: null })}
        />
      </div>
    </div>
  );
}

export default MoviesPage;