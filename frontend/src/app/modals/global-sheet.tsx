import { useMemo } from "react";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetDescription,
} from "@/components/ui/sheet";
import {
  DiscardChangesDialog,
  ModalGuardContext,
  useModalGuardController,
} from "@/components/common/modal-guard";
import { DismissGuardContext } from "@/hooks/use-block-dialog-close";
import { cn } from "@/lib/utils";
import {
  getSheetIsDirty,
  useSheetClose,
  useSheetDismissGuard,
  useSheetSetDirty,
  useSheetState,
} from "@/stores/sheet-store";

export const GlobalSheet = () => {
  const {
    isOpen,
    title,
    description,
    content,
    side,
    size,
    className,
    dismissible,
  } = useSheetState();
  const close = useSheetClose();
  const dismissGuard = useSheetDismissGuard();
  const setDirty = useSheetSetDirty();
  const { askDiscard, onOpenChange, discardOpen, keepEditing, discard } =
    useModalGuardController({
      isOpen,
      dismissible,
      close,
      getIsDirty: getSheetIsDirty,
    });
  const modalGuard = useMemo(() => ({ setDirty, askDiscard }), [setDirty, askDiscard]);

  return (
    <Sheet open={isOpen} onOpenChange={onOpenChange}>
      <SheetContent
        side={side}
        size={size}
        className={cn("flex flex-col p-0 gap-0", className)}
      >
        {(title || description) && (
          <SheetHeader className="shrink-0 px-6 pt-6 pb-4 border-b border-border/40">
            {title && <SheetTitle>{title}</SheetTitle>}
            {description && <SheetDescription>{description}</SheetDescription>}
          </SheetHeader>
        )}
        <ModalGuardContext.Provider value={modalGuard}>
          <DismissGuardContext.Provider value={dismissGuard}>
            <div className="flex-1 overflow-y-auto p-6 [--modal-gutter:1.5rem] [--modal-surface:var(--background)]">
              {content}
            </div>
          </DismissGuardContext.Provider>
        </ModalGuardContext.Provider>
        <DiscardChangesDialog
          open={discardOpen}
          onKeepEditing={keepEditing}
          onDiscard={discard}
        />
      </SheetContent>
    </Sheet>
  );
};