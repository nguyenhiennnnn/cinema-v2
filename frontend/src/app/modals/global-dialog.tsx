import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
} from "@/components/ui/dialog";
import { cn } from "@/lib/utils";
import { useDialogClose, useDialogState } from "@/stores/dialog-store";

export const GlobalDialog = () => {
  const { isOpen, title, description, content, className } = useDialogState();
  const close = useDialogClose();

  return (
    <Dialog open={isOpen} onOpenChange={(open) => !open && close()}>
      <DialogContent
        className={cn("flex max-h-[85vh] flex-col", className)}
      >
        {(title || description) && (
          <DialogHeader className="shrink-0">
            {title && <DialogTitle>{title}</DialogTitle>}
            {description && (
              <DialogDescription>{description}</DialogDescription>
            )}
          </DialogHeader>
        )}
        <div className="flex-1 overflow-y-auto overflow-x-hidden">{content}</div>
      </DialogContent>
    </Dialog>
  );
};