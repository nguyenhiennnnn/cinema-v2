import { useCallback, useLayoutEffect, useRef, type RefObject } from "react";
import type { MovieListItemDto } from "@/features/client/movies/types";
import { buildClipFrames, measureInset, type Inset } from "../utils/hero-clip";
import { HERO_TRANSITION_MS, type HeroDirection, type HeroLeaving } from "./use-hero-carousel";

const EASING = "cubic-bezier(0.22, 1, 0.36, 1)";
const THUMB_CUT = 6;

interface UseHeroTransitionParams {
  movies: MovieListItemDto[];
  index: number;
  leaving: HeroLeaving | null;
  reducedMotion: boolean;
  goTo: (target: number, direction: HeroDirection) => void;
  heroRef: RefObject<HTMLElement | null>;
  backdropRef: RefObject<HTMLDivElement | null>;
  stripRef: RefObject<HTMLDivElement | null>;
}

/** Đo vị trí thumbnail đích và chạy animation clip-path/strip khi chuyển slide. */
export function useHeroTransition({
  movies,
  index,
  leaving,
  reducedMotion,
  goTo,
  heroRef,
  backdropRef,
  stripRef,
}: UseHeroTransitionParams) {
  const count = movies.length;
  const thumbRefs = useRef(new Map<number, HTMLElement>());
  const originRef = useRef<Inset | null>(null);

  const registerThumb = useCallback((movieId: number, el: HTMLElement | null) => {
    if (el) thumbRefs.current.set(movieId, el);
    else thumbRefs.current.delete(movieId);
  }, []);

  const requestMove = (target: number, dir: HeroDirection) => {
    if (leaving || count < 2 || target === index) return;
    const hero = heroRef.current;
    const targetId = movies[target]?.movieId;
    const thumb = targetId === undefined ? undefined : thumbRefs.current.get(targetId);
    originRef.current =
      hero && thumb ? measureInset(hero.getBoundingClientRect(), thumb.getBoundingClientRect()) : null;
    goTo(target, dir);
  };

  const leavingMovieId = leaving ? movies[leaving.index]?.movieId : undefined;

  useLayoutEffect(() => {
    if (!leaving || reducedMotion) return;
    const hero = heroRef.current;
    const backdrop = backdropRef.current;
    if (!hero || !backdrop) return;

    const options: KeyframeAnimationOptions = { duration: HERO_TRANSITION_MS, easing: EASING, fill: "both" };
    const animations: Animation[] = [];

    const { width, height } = hero.getBoundingClientRect();
    const origin = originRef.current;
    animations.push(
      backdrop.animate(origin ? buildClipFrames(width, height, origin, THUMB_CUT) : [{ opacity: 0 }, { opacity: 1 }], options),
    );

    const strip = stripRef.current;
    const first = strip?.children[0];
    if (strip && first) {
      strip.style.willChange = "transform";
      const second = strip.children[1];
      const slot = second
        ? second.getBoundingClientRect().left - first.getBoundingClientRect().left
        : first.getBoundingClientRect().width + 8;
      const dx = leaving.direction === "next" ? slot : -slot;
      animations.push(strip.animate([{ transform: `translate3d(${dx}px,0,0)` }, { transform: "translate3d(0,0,0)" }], options));

      const leftThumb = leavingMovieId === undefined ? undefined : thumbRefs.current.get(leavingMovieId);
      if (leftThumb) animations.push(leftThumb.animate([{ opacity: 0 }, { opacity: 1 }], options));
    }

    return () => {
      animations.forEach((a) => a.cancel());
      if (strip) strip.style.willChange = "";
    };
  }, [leaving, leavingMovieId, reducedMotion]);

  return { registerThumb, requestMove };
}
