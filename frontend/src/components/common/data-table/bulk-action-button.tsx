import type { LucideIcon } from "lucide-react";
import { Button } from "@/components/ui/button";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";

interface BulkActionButtonProps {
  count: number;
  itemLabel?: string;
  actionLabel: string;
  icon: LucideIcon;
  variant?: "default" | "destructive" | "outline";
  description: string;
  onConfirm: () => Promise<void>;
  isPending?: boolean;
}

export const BulkActionButton = ({
  count,
  itemLabel = "mục",
  actionLabel,
  icon: Icon,
  variant = "default",
  description,
  onConfirm,
  isPending,
}: BulkActionButtonProps) => {
  const openAlertDialog = useAlertDialogOpen();

  const handleClick = () => {
    openAlertDialog({
      title: `${actionLabel} ${count} ${itemLabel}?`,
      description,
      confirmText: actionLabel,
      cancelText: "Hủy",
      variant: variant === "destructive" ? "destructive" : "default",
      onConfirm,
    });
  };

  return (
    <Button variant={variant} size="sm" disabled={isPending} onClick={handleClick}>
      <Icon className="size-4" />
      <span>
        {actionLabel} {count} {itemLabel}
      </span>
    </Button>
  );
};
