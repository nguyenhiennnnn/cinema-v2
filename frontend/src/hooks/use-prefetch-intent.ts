import type { PointerEvent } from "react";

interface PrefetchIntentProps {
  onPointerEnter: (event: PointerEvent) => void;
  onFocus: () => void;
  onTouchStart: () => void;
}

export const usePrefetchIntent = (prefetch: () => void): PrefetchIntentProps => ({
  onPointerEnter: (event) => {
    if (event.pointerType === "mouse") prefetch();
  },
  onFocus: prefetch,
  onTouchStart: prefetch,
});
