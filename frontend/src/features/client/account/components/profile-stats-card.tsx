import { MembershipTierBadge } from "@/components/common/status-badge";
import { formatCurrency, formatPoints } from "@/lib/formatters";
import type { UserProfileDto } from "@/features/client/account/types";

interface ProfileStatsCardProps {
  profile: UserProfileDto;
}

export const ProfileStatsCard = ({ profile }: ProfileStatsCardProps) => {
  const progress = profile.nextTier ? Math.min(100, (profile.points / (profile.points + profile.nextTier.pointsNeeded)) * 100) : 100;

  return (
    <div className="border border-border bg-card p-5">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div>
          <p className="text-xs text-muted-foreground">Hạng thành viên</p>
          <div className="mt-1">
            <MembershipTierBadge tierName={profile.tier?.tierName ?? "Thành viên"} />
          </div>
        </div>
        <div className="text-right">
          <p className="text-xs text-muted-foreground">Điểm tích lũy</p>
          <p className="mt-1 text-lg font-bold text-accent">{formatPoints(profile.points)}</p>
        </div>
        <div className="text-right">
          <p className="text-xs text-muted-foreground">Tổng chi tiêu</p>
          <p className="mt-1 text-lg font-bold text-foreground">{formatCurrency(profile.totalSpent)}</p>
        </div>
      </div>

      {profile.nextTier && (
        <div className="mt-4">
          <div className="h-2 w-full overflow-hidden rounded-full bg-muted">
            <div className="h-full rounded-full bg-primary" style={{ width: `${progress}%` }} />
          </div>
          <p className="mt-1.5 text-xs text-muted-foreground">
            Cần thêm {formatPoints(profile.nextTier.pointsNeeded)} điểm để đạt hạng {profile.nextTier.tierName}
          </p>
        </div>
      )}
    </div>
  );
}