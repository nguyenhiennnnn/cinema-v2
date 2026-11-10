import { useParams } from "react-router";
import { MovieHero } from "@/features/client/movies/components/movie-hero";
import { MovieCastCrew } from "@/features/client/movies/components/movie-cast-crew";
import { MovieShowtimeSelector } from "@/features/client/movies/components/movie-showtime-selector";
import { MovieReviews } from "@/features/client/movies/components/movie-reviews";
import { ExpandableText } from "@/components/common/expandable-text";
import { useMovieDetail } from "@/features/client/movies/hooks/use-movies";
import { Skeleton } from "@/components/ui/skeleton";
import { Seo } from "@/components/common/seo";
import { BRAND } from "@/config/brand";
import { ROUTES } from "@/config/routes";

const MovieDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const movieId = Number(id);

  const { data: movie, isLoading } = useMovieDetail(movieId);

  if (isLoading || !movie) {
    return (
      <div className="mx-auto max-w-6xl space-y-6 px-4 py-8 sm:px-6 lg:px-8">
        <Skeleton className="h-96 w-full" />
        <Skeleton className="h-40 w-full" />
      </div>
    );
  }

  const movieStructuredData = {
    "@context": "https://schema.org",
    "@type": "Movie",
    name: movie.title,
    description: movie.description,
    image: movie.posterUrl,
    director: movie.director
      ? { "@type": "Person", name: movie.director.fullName }
      : undefined,
    actor: movie.cast.map((member) => ({
      "@type": "Person",
      name: member.fullName,
      ...(member.characterName ? { characterName: member.characterName } : {}),
    })),
    genre: movie.genres.map((g) => g.name),
    duration: movie.durationMinutes
      ? `PT${movie.durationMinutes}M`
      : undefined,
  };

  const seoDescription = movie.description
    ? movie.description.slice(0, 160)
    : `Xem thông tin và đặt vé phim ${movie.title} tại ${BRAND.name}. Chọn suất chiếu, ghế ngồi và thanh toán online nhanh chóng.`;

  return (
    <div className="mx-auto max-w-6xl space-y-10 px-4 py-8 sm:px-6 lg:px-8">
      <Seo
        title={movie.title}
        description={seoDescription}
        canonical={ROUTES.movies.detail(movie.movieId)}
        ogImage={movie.posterUrl ?? undefined}
        ogType="article"
        structuredData={movieStructuredData}
      />

      <MovieHero movie={movie} />

      <div className="grid gap-10 lg:grid-cols-3">
        <div className="space-y-8 lg:col-span-2">
          {movie.description && (
            <div>
              <h2 className="t-h2 text-foreground">Nội dung phim</h2>
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