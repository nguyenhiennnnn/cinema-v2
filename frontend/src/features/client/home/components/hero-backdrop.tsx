import type { Ref } from "react";
import { cn } from "@/lib/utils";
import type { MovieListItemDto } from "@/features/client/movies/types";

interface HeroBackdropProps {
  movie: MovieListItemDto;
  priority?: boolean;
  animated?: boolean;
  ref?: Ref<HTMLDivElement>;
}

export const HeroBackdrop = ({ movie, priority = false, animated = false, ref }: HeroBackdropProps) => (
  <div
    ref={ref}
    className={cn("absolute inset-0 transform-gpu", animated && "will-change-[clip-path,opacity]")}
    aria-hidden="true"
  >
    {movie.posterUrl && (
      <img
        src={movie.posterUrl}
        alt=""
        decoding="async"
        fetchPriority={priority ? "high" : undefined}
        className="h-full w-full object-cover"
      />
    )}
    <div className="absolute inset-0 bg-scrim/70" />
  </div>
);
