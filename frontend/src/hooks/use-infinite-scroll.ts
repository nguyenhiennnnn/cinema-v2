import { useEffect, useRef, useCallback } from "react";

interface UseInfiniteScrollOptions {
  hasNextPage: boolean | undefined;
  isFetchingNextPage: boolean;
  fetchNextPage: () => void;
  rootMargin?: string;
  threshold?: number;
  disabled?: boolean;
}

export function useInfiniteScroll({
  hasNextPage,
  isFetchingNextPage,
  fetchNextPage,
  rootMargin = "200px",
  threshold = 0,
  disabled = false,
}: UseInfiniteScrollOptions) {
  const triggerRef = useRef<HTMLDivElement | null>(null);

  const handleIntersect = useCallback<IntersectionObserverCallback>(
    (entries) => {
      const entry = entries[0];
      if (
        entry.isIntersecting &&
        hasNextPage &&
        !isFetchingNextPage &&
        !disabled
      ) {
        fetchNextPage();
      }
    },
    [hasNextPage, isFetchingNextPage, fetchNextPage, disabled],
  );

  useEffect(() => {
    const node = triggerRef.current;
    if (!node) return;

    const observer = new IntersectionObserver(handleIntersect, {
      root: null,
      rootMargin,
      threshold,
    });

    observer.observe(node);

    return () => {
      observer.disconnect();
    };
  }, [handleIntersect, rootMargin, threshold]);

  return { triggerRef };
}
