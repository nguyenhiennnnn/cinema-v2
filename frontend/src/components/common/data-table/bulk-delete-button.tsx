import { Trash2 } from "lucide-react";
import { BulkActionButton } from "./bulk-action-button";

interface BulkDeleteButtonProps {
  count: number;
  itemLabel?: string;
  onConfirm: () => Promise<void>;
  isPending?: boolean;
}

export const BulkDeleteButton = ({
  count,
  itemLabel = "mục",
  onConfirm,
  isPending,
}: BulkDeleteButtonProps) => (
  <BulkActionButton
    count={count}
    itemLabel={itemLabel}
    actionLabel="Xóa"
    icon={Trash2}
    variant="destructive"
    description={`Thao tác này không thể hoàn tác. Các ${itemLabel} không xóa được sẽ được thông báo riêng.`}
    onConfirm={onConfirm}
    isPending={isPending}
  />
);
