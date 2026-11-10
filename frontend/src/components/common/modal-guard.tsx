import { createContext, useCallback, useContext, useEffect, useState } from "react";

import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";

export interface ModalGuard {
  setDirty: (dirty: boolean) => void;
  askDiscard: (action: () => void) => void;
}

export const ModalGuardContext = createContext<ModalGuard | null>(null);

export const useUnsavedChangesGuard = (isDirty: boolean) => {
  const guard = useContext(ModalGuardContext);

  useEffect(() => {
    if (!guard) return;
    guard.setDirty(isDirty);
    return () => guard.setDirty(false);
  }, [guard, isDirty]);
};

export const useDiscardConfirm = (isDirty: boolean) => {
  const guard = useContext(ModalGuardContext);

  return useCallback(
    (action: () => void) => {
      if (!isDirty || !guard) {
        action();
        return;
      }
      guard.askDiscard(action);
    },
    [guard, isDirty],
  );
};

interface UseModalGuardControllerOptions {
  isOpen: boolean;
  dismissible: boolean;
  close: () => void;
  getIsDirty: () => boolean;
}


export const useModalGuardController = ({
  isOpen,
  dismissible,
  close,
  getIsDirty,
}: UseModalGuardControllerOptions) => {
  const [pending, setPending] = useState<(() => void) | null>(null);

  if (!isOpen && pending) setPending(null);

  const askDiscard = useCallback((action: () => void) => {
    setPending(() => action);
  }, []);

  const onOpenChange = (open: boolean) => {
    if (open || !dismissible) return;
    if (getIsDirty()) askDiscard(close);
    else close();
  };

  const keepEditing = () => setPending(null);

  const discard = () => {
    const action = pending;
    setPending(null);
    action?.();
  };

  return {
    askDiscard,
    onOpenChange,
    discardOpen: pending !== null,
    keepEditing,
    discard,
  };
};

interface DiscardChangesDialogProps {
  open: boolean;
  onKeepEditing: () => void;
  onDiscard: () => void;
}

export const DiscardChangesDialog = ({
  open,
  onKeepEditing,
  onDiscard,
}: DiscardChangesDialogProps) => (
  <AlertDialog open={open} onOpenChange={(next) => !next && onKeepEditing()}>
    <AlertDialogContent>
      <AlertDialogHeader>
        <AlertDialogTitle>Bỏ thay đổi chưa lưu?</AlertDialogTitle>
        <AlertDialogDescription>
          Những gì bạn vừa nhập sẽ mất nếu đóng bây giờ.
        </AlertDialogDescription>
      </AlertDialogHeader>
      <AlertDialogFooter>
        <AlertDialogCancel onClick={onKeepEditing}>
          Tiếp tục chỉnh sửa
        </AlertDialogCancel>
        <AlertDialogAction variant="destructive" onClick={onDiscard}>
          Bỏ thay đổi
        </AlertDialogAction>
      </AlertDialogFooter>
    </AlertDialogContent>
  </AlertDialog>
);
