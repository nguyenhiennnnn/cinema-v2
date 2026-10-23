import { MembershipTierBadge } from "@/components/common/status-badge";
import type { AuthUser } from "@/stores/auth-store";

interface AccountUserCardProps {
  user: AuthUser | null | undefined;
}

export const AccountUserCard = ({ user }: AccountUserCardProps) => (
  <div className="border border-border bg-card p-4">
    <div className="flex items-center gap-3">
      <img
        src={user?.avatar ?? "/icons.svg"}
        alt={user?.fullName}
        className="size-12 rounded-full border border-border object-cover"
      />
      <div className="min-w-0">
        <p className="truncate font-medium text-foreground">{user?.fullName}</p>
        <p className="truncate text-xs text-muted-foreground">{user?.email}</p>
      </div>
    </div>
    <div className="mt-3 flex items-center justify-between">
      <MembershipTierBadge tierName={user?.membershipTierName ?? "Thành viên"} />
      <span className="text-xs font-medium text-muted-foreground">{user?.points ?? 0} điểm</span>
    </div>
  </div>
);