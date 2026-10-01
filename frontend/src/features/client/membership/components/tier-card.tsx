import { cn } from "@/lib/utils";
import { getVisual } from "../utils";
import { formatPoints } from "@/lib/formatters";
import type { MembershipTierDto } from "@/features/admin/catalog/types";

interface TierCardProps {
  tier: MembershipTierDto;
  index: number;
}

export const TierCard = ({ tier, index }: TierCardProps) => {
  const visual = getVisual(index);
  const Icon = visual.icon;

  return (
    <div
      className={cn(
        "group relative flex flex-col overflow-hidden rounded-xl border border-border bg-card p-5 transition-colors",
        visual.ring,
      )}
    >
      <div
        className={cn(
          "mb-4 flex size-11 items-center justify-center rounded-lg bg-muted",
          visual.accent,
        )}
      >
        <Icon className="size-5" />
      </div>

      <h3 className="font-heading text-lg font-semibold text-foreground">
        Hạng {tier.tierName}
      </h3>

      <p className="mt-1 text-xs text-muted-foreground">
        Yêu cầu{" "}
        <span className="font-medium text-foreground">
          {formatPoints(tier.minPoints)} điểm
        </span>
      </p>

      <div className="mt-4 flex items-baseline gap-1">
        <span
          className={cn(
            "font-heading text-3xl font-bold tabular",
            visual.accent,
          )}
        >
          {tier.discountPercent}%
        </span>
        <span className="text-xs text-muted-foreground">chiết khấu</span>
      </div>

      <p className="mt-4 text-xs leading-relaxed text-muted-foreground">
        {visual.perk}
      </p>
    </div>
  );
};
