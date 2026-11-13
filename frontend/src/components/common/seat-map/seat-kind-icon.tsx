import { Crown, Heart } from "lucide-react";
import type { SeatVisualKind } from "./seat-map-types";

export const SeatKindIcon = ({ kind, className }: { kind?: SeatVisualKind; className?: string }) => {
  if (kind === "vip") return <Crown className={className} />;
  if (kind === "couple") return <Heart className={className} />;
  return null;
};
