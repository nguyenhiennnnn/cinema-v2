import { useEffect, useState } from "react";
import { Link } from "react-router";
import { ChevronLeft, ChevronRight, Info, Star, Ticket } from "lucide-react";
import { AgeRatingBadge } from "@/components/common/status-badge";
import { Skeleton } from "@/components/ui/skeleton";
import { formatDuration } from "@/lib/cinema-format";
import { useMovies } from "@/features/client/movies/hooks/use-movies";
import { MovieStatus } from "@/types/enum";
import { cn } from "@/lib/utils";

export const HeroCarousel = () => {
  const { data, isLoading } = useMovies({ status: MovieStatus.NowShowing, limit: 5 });
  const movies = data?.pages ?? [];
  const [index, setIndex] = useState(0);

  useEffect(() => {
    if (movies.length <= 1) return;
    const timer = setInterval(() => setIndex((i) => (i + 1) % movies.length), 6000);
    return () => clearInterval(timer);
  }, [movies.length]);

  if (isLoading) {
    return (
      <div className="relative h-[70vh] min-h-105 w-full overflow-hidden bg-muted">
        <Skeleton className="absolute inset-0 rounded-none" />
        <div className="relative mx-auto flex h-full max-w-7xl flex-col justify-end gap-3 px-4 pb-14 sm:px-6 lg:px-8">
          <Skeleton className="h-3 w-44 bg-foreground/10" />
          <Skeleton className="h-11 w-72 bg-foreground/10 sm:h-16 sm:w-[26rem]" />
          <Skeleton className="h-4 w-56 bg-foreground/10" />
          <div className="mt-3 flex gap-3">
            <Skeleton className="h-10 w-36 bg-foreground/10" />
            <Skeleton className="h-10 w-36 bg-foreground/10" />
          </div>
        </div>
      </div>
    );
  }

  if (movies.length === 0) return null;
  const movie = movies[index];

  return (
    <div className="relative h-[70vh] min-h-105 w-full overflow-hidden bg-black">
      <img src={movie.posterUrl ?? undefined} alt={movie.title} className="absolute inset-0 h-full w-full object-cover" />
      {/* Lớp phủ tối cố định (không phụ thuộc theme) để chữ luôn đọc rõ trên cả light/dark mode,
          thay vì fade về var(--background) trước đây khiến banner bị chói ở theme sáng. */}
      <div className="absolute inset-0 bg-linear-to-t from-black via-black/60 to-black/10" />
      <div className="absolute inset-0 bg-linear-to-r from-black/85 via-black/25 to-transparent" />

      <div className="relative mx-auto flex h-full max-w-7xl flex-col justify-end px-4 pb-14 sm:px-6 lg:px-8">
        <p className="text-eyebrow text-accent">Đang chiếu · Suất chọn lọc</p>

        <div className="mt-3 flex flex-wrap items-center gap-2">
          <AgeRatingBadge code={movie.ageRating?.code} onImage />
          {movie.genres.slice(0, 3).map((g) => (
            <span key={g} className="border border-white/15 bg-white/10 px-2 py-0.5 text-xs font-medium text-white/90 backdrop-blur-sm">
              {g}
            </span>
          ))}
        </div>

        <h1 className="mt-3 max-w-2xl font-heading text-4xl leading-[0.95] font-semibold tracking-tight text-white uppercase md:text-6xl">{movie.title}</h1>

        <div className="mt-4 flex items-center gap-4 text-sm text-white/70">
          <span>{formatDuration(movie.durationMinutes)}</span>
          {movie.averageRating > 0 && <span className="font-semibold text-accent">
            <span className="flex items-center gap-1"><Star className="size-4 fill-accent text-accent" /> <span>{movie.averageRating.toFixed(1)}</span></span>
          </span>}
        </div>

        <div className="mt-6 flex flex-wrap gap-3">
          <Link
            to={`/movies/${movie.movieId}`}
            className="clip-corner inline-flex items-center gap-2 bg-primary px-5 py-2.5 text-sm font-semibold tracking-wide text-primary-foreground uppercase hover:bg-primary/90"
          >
            <Ticket className="size-4" />
            Mua vé ngay
          </Link>
          <Link
            to={`/movies/${movie.movieId}`}
            className="inline-flex items-center gap-2 border border-white/25 bg-white/10 px-5 py-2.5 text-sm font-semibold text-white backdrop-blur-sm hover:bg-white/20"
          >
            <Info className="size-4" />
            Chi tiết phim
          </Link>
        </div>

        {movies.length > 1 && (
          <div className="mt-8 flex items-center gap-4">
            <button onClick={() => setIndex((i) => (i - 1 + movies.length) % movies.length)} className="text-white/60 hover:text-white">
              <ChevronLeft className="size-5" />
            </button>
            <div className="flex gap-1.5">
              {movies.map((m, i) => (
                <button
                  key={m.movieId}
                  onClick={() => setIndex(i)}
                  className={cn("h-1 transition-all", i === index ? "w-7 bg-primary" : "w-3.5 bg-white/30")}
                />
              ))}
            </div>
            <button onClick={() => setIndex((i) => (i + 1) % movies.length)} className="text-white/60 hover:text-white">
              <ChevronRight className="size-5" />
            </button>
          </div>
        )}
      </div>
    </div>
  );
}