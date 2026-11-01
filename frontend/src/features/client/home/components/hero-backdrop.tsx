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
        className="hero-backdrop-img h-full w-full object-cover object-top"
      />
    )}
    {/* Lớp tối đồng đều để đảm bảo tương phản */}
    <div className="absolute inset-0 bg-scrim/60" />
    {/* Gradient trái: tạo vùng đọc chữ rõ nét */}
    <div className="absolute inset-0 hero-backdrop-left" />
    {/* Gradient dưới: xử lý vùng controls */}
    <div className="absolute inset-x-0 bottom-0 h-64 hero-backdrop-bottom" />
  </div>
);
