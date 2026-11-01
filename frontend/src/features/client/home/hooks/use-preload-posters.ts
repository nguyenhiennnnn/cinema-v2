import { useEffect, useRef } from "react";
import type { MovieListItemDto } from "@/features/client/movies/types";

export const usePreloadPosters = (movies: MovieListItemDto[]) => {
  const cache = useRef<HTMLImageElement[]>([]);
  const key = movies.map((m) => m.posterUrl ?? "").join("|");

  useEffect(() => {
    cache.current = movies.flatMap((m) => {
      if (!m.posterUrl) return [];
      const img = new Image();
      img.decoding = "async";
      img.src = m.posterUrl;
      img.decode?.().catch(() => { });
      return [img];
    });
  }, [key]);
};
