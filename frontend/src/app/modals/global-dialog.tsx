import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
} from "@/components/ui/dialog";
import { DismissGuardContext } from "@/hooks/use-block-dialog-close";
import { cn } from "@/lib/utils";
import {
  useDialogClose,
  useDialogDismissGuard,
  useDialogState,
} from "@/stores/dialog-store";

export const GlobalDialog = () => {
  const { isOpen, title, description, content, className, dismissible } =
    useDialogState();
  const close = useDialogClose();
  const dismissGuard = useDialogDismissGuard();

  return (
    <Dialog open={isOpen} onOpenChange={(open) => !open && dismissible && close()}>
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
        <DismissGuardContext.Provider value={dismissGuard}>
          <div className="flex-1 overflow-y-auto overflow-x-hidden">
            {content}
          </div>
        </DismissGuardContext.Provider>
      </DialogContent>
    </Dialog>
  );
};