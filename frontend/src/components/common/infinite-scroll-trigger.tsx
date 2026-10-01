"use client";

import { useInfiniteScroll } from "@/hooks/use-infinite-scroll";
import { Loader2 } from "lucide-react";
import type { ReactNode } from "react";

interface InfiniteScrollTriggerProps {
  hasNextPage: boolean | undefined;
  isFetchingNextPage: boolean;
  fetchNextPage: () => void;
  rootMargin?: string;
  disabled?: boolean;
  loader?: ReactNode;
  endMessage?: ReactNode;
  className?: string;
}

export const InfiniteScrollTrigger = ({
  hasNextPage,
  isFetchingNextPage,
  fetchNextPage,
  rootMargin,
  disabled,
  loader,
  endMessage,
  className,
}: InfiniteScrollTriggerProps) => {
  const { triggerRef } = useInfiniteScroll({
    hasNextPage,
    isFetchingNextPage,
    fetchNextPage,
    rootMargin,
    disabled,
  });

  return (
    <div ref={triggerRef} className={className}>
      {isFetchingNextPage &&
        (loader ?? (
          <div className="flex items-center justify-center py-4">
            <Loader2 className="size-5 animate-spin text-muted-foreground" />
          </div>
        ))}
      {!hasNextPage && !isFetchingNextPage && endMessage}
    </div>
  );
};
