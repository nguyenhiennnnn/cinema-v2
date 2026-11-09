import { create } from "zustand";
import { useShallow } from "zustand/react/shallow";
import type { ReactNode } from "react";

interface DialogOptions {
  title?: string;
  description?: string;
  content: ReactNode;
  className?: string;
}

interface DialogStore extends DialogOptions {
  isOpen: boolean;
  dismissLocks: number;
  open: (options: DialogOptions) => void;
  close: () => void;
  blockDismiss: () => void;
  releaseDismiss: () => void;
}

export const useDialogStore = create<DialogStore>((set) => ({
  isOpen: false,
  dismissLocks: 0,
  title: undefined,
  description: undefined,
  content: null,
  className: undefined,
  open: ({ title, description, content, className }) =>
    set({
      isOpen: true,
      dismissLocks: 0,
      title,
      description,
      content,
      className,
    }),
  close: () => set({ isOpen: false, dismissLocks: 0 }),
  blockDismiss: () => set((s) => ({ dismissLocks: s.dismissLocks + 1 })),
  releaseDismiss: () =>
    set((s) => ({ dismissLocks: Math.max(0, s.dismissLocks - 1) })),
}));

export const useDialogOpen = () => useDialogStore((s) => s.open);
export const useDialogClose = () => useDialogStore((s) => s.close);

export const useDialogState = () =>
  useDialogStore(
    useShallow((s) => ({
      isOpen: s.isOpen,
      title: s.title,
      description: s.description,
      content: s.content,
      className: s.className,
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
