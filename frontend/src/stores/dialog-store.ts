import { create } from "zustand";
import { useShallow } from "zustand/react/shallow";
import type { ReactNode } from "react";
import type { DialogSize } from "@/components/ui/dialog";

interface DialogOptions {
  title?: string;
  description?: string;
  content: ReactNode;
  size?: DialogSize;
  className?: string;
  flush?: boolean;
}

interface DialogStore extends DialogOptions {
  isOpen: boolean;
  isDirty: boolean;
  dismissLocks: number;
  open: (options: DialogOptions) => void;
  close: () => void;
  setDirty: (dirty: boolean) => void;
  blockDismiss: () => void;
  releaseDismiss: () => void;
}

export const useDialogStore = create<DialogStore>((set) => ({
  isOpen: false,
  isDirty: false,
  dismissLocks: 0,
  title: undefined,
  description: undefined,
  content: null,
  size: "md",
  className: undefined,
  flush: false,
  open: ({ title, description, content, size = "md", className, flush = false }) =>
    set({
      isOpen: true,
      isDirty: false,
      dismissLocks: 0,
      title,
      description,
      content,
      size,
      className,
      flush,
    }),
  close: () => set({ isOpen: false, isDirty: false, dismissLocks: 0 }),
  setDirty: (dirty) => set({ isDirty: dirty }),
  blockDismiss: () => set((s) => ({ dismissLocks: s.dismissLocks + 1 })),
  releaseDismiss: () =>
    set((s) => ({ dismissLocks: Math.max(0, s.dismissLocks - 1) })),
}));

export const useDialogOpen = () => useDialogStore((s) => s.open);
export const useDialogClose = () => useDialogStore((s) => s.close);
export const useDialogSetDirty = () => useDialogStore((s) => s.setDirty);
const getDialogIsDirty = () => useDialogStore.getState().isDirty;
export { getDialogIsDirty };

export const useDialogState = () =>
  useDialogStore(
    useShallow((s) => ({
      isOpen: s.isOpen,
      title: s.title,
      description: s.description,
      content: s.content,
      size: s.size,
      className: s.className,
      flush: s.flush,
      dismissible: s.dismissLocks === 0,
    })),
  );

export const useDialogDismissGuard = () =>
  useDialogStore(
    useShallow((s) => ({
      block: s.blockDismiss,
      release: s.releaseDismiss,
    })),
  );