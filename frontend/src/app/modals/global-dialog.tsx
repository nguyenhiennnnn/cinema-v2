import { useMemo } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
} from "@/components/ui/dialog";
import {
  DiscardChangesDialog,
  ModalGuardContext,
  useModalGuardController,
} from "@/components/common/modal-guard";
import { DismissGuardContext } from "@/hooks/use-block-dialog-close";
import { cn } from "@/lib/utils";
import {
  getDialogIsDirty,
  useDialogClose,
  useDialogDismissGuard,
  useDialogSetDirty,
  useDialogState,
} from "@/stores/dialog-store";

export const GlobalDialog = () => {
  const { isOpen, title, description, content, size, className, flush, dismissible } =
    useDialogState();
  const close = useDialogClose();
  const dismissGuard = useDialogDismissGuard();
  const setDirty = useDialogSetDirty();
  const { askDiscard, onOpenChange, discardOpen, keepEditing, discard } =
    useModalGuardController({
      isOpen,
      dismissible,
      close,
      getIsDirty: getDialogIsDirty,
    });
  const modalGuard = useMemo(() => ({ setDirty, askDiscard }), [setDirty, askDiscard]);

  return (
    <Dialog open={isOpen} onOpenChange={onOpenChange}>
      <DialogContent
        size={size}
        className={cn(
          "flex max-h-[85vh] flex-col overflow-hidden",
          flush && "gap-0 border-none bg-scrim p-0",
          className,
        )}
      >
        {(title || description) && (
          <DialogHeader className="shrink-0">
            {title && <DialogTitle>{title}</DialogTitle>}
            {description && (
              <DialogDescription>{description}</DialogDescription>
            )}
          </DialogHeader>
        )}
        <ModalGuardContext.Provider value={modalGuard}>
          <DismissGuardContext.Provider value={dismissGuard}>
            <div
              className={cn(
                "min-h-0 flex-1 overflow-y-auto overflow-x-hidden overscroll-contain [--modal-gutter:1.5rem] [--modal-surface:var(--card)]",
                !flush && "-mx-6 -mb-6 px-6 pb-6 has-data-[slot=form-actions]:pb-0",
              )}
            >
              {content}
            </div>
          </DismissGuardContext.Provider>
        </ModalGuardContext.Provider>
        <DiscardChangesDialog
          open={discardOpen}
          onKeepEditing={keepEditing}
          onDiscard={discard}
        />
      </DialogContent>
    </Dialog>
  );
};