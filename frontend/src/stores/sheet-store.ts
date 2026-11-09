import { create } from "zustand";
import { useShallow } from "zustand/react/shallow";
import type { ReactNode } from "react";

type SheetSide = "top" | "right" | "bottom" | "left";

interface SheetOptions {
  title?: string;
  description?: string;
  content: ReactNode;
  side?: SheetSide;
  className?: string;
}

interface SheetStore extends SheetOptions {
  isOpen: boolean;
  dismissLocks: number;
  open: (options: SheetOptions) => void;
  close: () => void;
  blockDismiss: () => void;
  releaseDismiss: () => void;
}

export const useSheetStore = create<SheetStore>((set) => ({
  isOpen: false,
  dismissLocks: 0,
  title: undefined,
  description: undefined,
  content: null,
  side: "right",
  className: undefined,
  open: ({ title, description, content, side = "right", className }) =>
    set({
      isOpen: true,
      dismissLocks: 0,
      title,
      description,
      content,
      side,
      className,
    }),
  close: () => set({ isOpen: false, dismissLocks: 0 }),
  blockDismiss: () => set((s) => ({ dismissLocks: s.dismissLocks + 1 })),
  releaseDismiss: () =>
    set((s) => ({ dismissLocks: Math.max(0, s.dismissLocks - 1) })),
}));

export const useSheetOpen = () => useSheetStore((s) => s.open);
export const useSheetClose = () => useSheetStore((s) => s.close);

export const useSheetState = () =>
  useSheetStore(
    useShallow((s) => ({
      isOpen: s.isOpen,
      title: s.title,
      description: s.description,
      content: s.content,
      side: s.side,
      className: s.className,
      dismissible: s.dismissLocks === 0,
    })),
  );

export const useSheetDismissGuard = () =>
  useSheetStore(
    useShallow((s) => ({
      block: s.blockDismiss,
      release: s.releaseDismiss,
    })),
  );
