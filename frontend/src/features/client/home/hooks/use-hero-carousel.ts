import { useCallback, useEffect, useState, useSyncExternalStore } from "react";

export type HeroDirection = "next" | "prev";

export interface HeroLeaving {
  index: number;
  direction: HeroDirection;
}

export const HERO_INTERVAL_MS = 7000;
export const HERO_TRANSITION_MS = 480;

const REDUCED_MOTION_QUERY = "(prefers-reduced-motion: reduce)";

function subscribeReducedMotion(onChange: () => void) {
  const mq = window.matchMedia(REDUCED_MOTION_QUERY);
  mq.addEventListener("change", onChange);
  return () => mq.removeEventListener("change", onChange);
}

function subscribeVisibility(onChange: () => void) {
  document.addEventListener("visibilitychange", onChange);
  return () => document.removeEventListener("visibilitychange", onChange);
}

export function usePrefersReducedMotion() {
  return useSyncExternalStore(
    subscribeReducedMotion,
    () => window.matchMedia(REDUCED_MOTION_QUERY).matches,
    () => false,
  );
}

function useDocumentHidden() {
  return useSyncExternalStore(
    subscribeVisibility,
    () => document.hidden,
    () => false,
  );
}

interface CarouselState {
  index: number;
  leaving: HeroLeaving | null;
  direction: HeroDirection | null;
}

export function useHeroCarousel(count: number) {
  const [state, setState] = useState<CarouselState>({
    index: 0,
    leaving: null,
    direction: null,
  });
  const [userPaused, setUserPaused] = useState(false);
  const [hovered, setHovered] = useState(false);
  const [focused, setFocused] = useState(false);

  const reducedMotion = usePrefersReducedMotion();
  const documentHidden = useDocumentHidden();

  const index = count > 0 ? Math.min(state.index, count - 1) : 0;
  const leaving = state.leaving;

  const goTo = useCallback(
    (target: number, direction: HeroDirection) => {
      setState((prev) => {
        if (
          prev.leaving ||
          target === prev.index ||
          target < 0 ||
          target >= count
        ) {
          return prev;
        }
        return {
          index: target,
          leaving: { index: prev.index, direction },
          direction,
        };
      });
    },
    [count],
  );

  useEffect(() => {
    if (!leaving) return;
    const wait = reducedMotion ? 0 : HERO_TRANSITION_MS + 40;
    const id = window.setTimeout(() => {
      setState((prev) => ({ ...prev, leaving: null }));
    }, wait);
    return () => window.clearTimeout(id);
  }, [leaving, reducedMotion]);

  const canAutoplay = count > 1 && !reducedMotion;
  const paused = userPaused || hovered || focused || documentHidden;

  const pauseHandlers = {
    onPointerEnter: (e: React.PointerEvent) => {
      if (e.pointerType === "mouse") setHovered(true);
    },
    onPointerLeave: () => setHovered(false),
    onFocus: (e: React.FocusEvent) => {
      if (e.target.matches(":focus-visible")) setFocused(true);
    },
    onBlur: () => setFocused(false),
  };

  return {
    index,
    leaving,
    direction: state.direction,
    count,
    goTo,
    reducedMotion,
    canAutoplay,
    paused,
    userPaused,
    toggleUserPaused: () => setUserPaused((v) => !v),
    pauseHandlers,
  };
}
