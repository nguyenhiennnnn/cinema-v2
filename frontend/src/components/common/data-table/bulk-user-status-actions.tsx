import { Power, PowerOff } from "lucide-react";
import { BulkActionButton } from "./bulk-action-button";

interface BulkUserStatusActionsProps {
  count: number;
  itemLabel: string;
  unlockLabel: string;
  lockLabel: string;
  unlockDescription: string;
  lockDescription: string;
  onSetActive: (isActive: boolean) => Promise<void>;
  isPending?: boolean;
}

export const BulkUserStatusActions = ({
  count,
  itemLabel,
  unlockLabel,
  lockLabel,
  unlockDescription,
  lockDescription,
  onSetActive,
  isPending,
}: BulkUserStatusActionsProps) => (
  <>
    <BulkActionButton
      count={count}
      itemLabel={itemLabel}
      actionLabel={unlockLabel}
      icon={Power}
      variant="outline"
      description={unlockDescription}
      onConfirm={() => onSetActive(true)}
      isPending={isPending}
    />
    <BulkActionButton
      count={count}
      itemLabel={itemLabel}
      actionLabel={lockLabel}
      icon={PowerOff}
      variant="destructive"
      description={lockDescription}
      onConfirm={() => onSetActive(false)}
      isPending={isPending}
    />
  </>
);
