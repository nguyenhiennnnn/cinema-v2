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
  open: (options: DialogOptions) => void;
  close: () => void;
}

export const useDialogStore = create<DialogStore>((set) => ({
  isOpen: false,
  title: undefined,
  description: undefined,
  content: null,
  className: undefined,
  open: ({ title, description, content, className }) =>
    set({ isOpen: true, title, description, content, className }),
  close: () => set({ isOpen: false }),
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
    })),
  );
