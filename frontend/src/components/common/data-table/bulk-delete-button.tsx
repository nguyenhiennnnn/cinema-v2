import { Trash2 } from "lucide-react";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog";

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
}: BulkDeleteButtonProps) => {
  const [open, setOpen] = useState(false);

  const handleConfirm = async () => {
    await onConfirm();
    setOpen(false);
  };

  return (
    <AlertDialog open={open} onOpenChange={setOpen}>
      <AlertDialogTrigger>
        <Button variant="destructive" size="sm" disabled={isPending}>
          <Trash2 className="size-4" />
          <span>
            Xóa {count} {itemLabel}
          </span>
        </Button>
      </AlertDialogTrigger>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>
            Xóa {count} {itemLabel}?
          </AlertDialogTitle>
          <AlertDialogDescription>
            Thao tác này không thể hoàn tác. Các {itemLabel} không xóa được sẽ
            được thông báo riêng.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel>Hủy</AlertDialogCancel>
          <AlertDialogAction
            onClick={handleConfirm}
            disabled={isPending}
            className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
          >
            Xóa
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}
