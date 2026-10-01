import { TIER_VISUALS } from "./constants";

export function getVisual(index: number) {
  return TIER_VISUALS[index] ?? TIER_VISUALS[TIER_VISUALS.length - 1];
}
