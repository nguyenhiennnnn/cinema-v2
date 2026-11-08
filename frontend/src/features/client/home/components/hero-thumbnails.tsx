import type { Ref } from "react";
import type { MovieListItemDto } from "@/features/client/movies/types";

export interface HeroThumbnailItem {
  movie: MovieListItemDto;
  index: number;
}

interface HeroThumbnailsProps {
  items: HeroThumbnailItem[];
  onSelect: (index: number) => void;
  registerThumb: (movieId: number, el: HTMLElement | null) => void;
  ref?: Ref<HTMLDivElement>;
}

export const HeroThumbnails = ({ items, onSelect, registerThumb, ref }: HeroThumbnailsProps) => {
  if (items.length === 0) return null;

  return (
    <div ref={ref} className="flex gap-2">
      {items.map(({ movie, index }) => (
        <button
          key={movie.movieId}
          ref={(el) => registerThumb(movie.movieId, el)}
          type="button"
          onClick={() => onSelect(index)}
          aria-label={`Chuyển đến phim ${movie.title}`}
          className="relative block aspect-2/3 w-14 shrink-0 cut-sm bg-muted text-scrim-foreground opacity-70 transition-opacity duration-150 hover:opacity-100 sm:w-16 lg:w-18"
        >
          {movie.posterUrl ? (
            <img
              src={movie.posterUrl}
              alt=""
              decoding="async"
              className="h-full w-full object-cover"
            />
          ) : (
            <span className="t-code flex h-full w-full items-center justify-center text-muted-foreground">?</span>
          )}
        </button>
      ))}
    </div>
  );
};
