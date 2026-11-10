import { create } from "zustand";
import { useShallow } from "zustand/react/shallow";
import type { ReactNode } from "react";
import type { SheetSide, SheetSize } from "@/components/ui/sheet";

interface SheetOptions {
  title?: string;
  description?: string;
  content: ReactNode;
  side?: SheetSide;
  size?: SheetSize;
  className?: string;
}

interface SheetStore extends SheetOptions {
  isOpen: boolean;
  isDirty: boolean;
  dismissLocks: number;
  open: (options: SheetOptions) => void;
  close: () => void;
  setDirty: (dirty: boolean) => void;
  blockDismiss: () => void;
  releaseDismiss: () => void;
}

export const useSheetStore = create<SheetStore>((set) => ({
  isOpen: false,
  isDirty: false,
  dismissLocks: 0,
  title: undefined,
  description: undefined,
  content: null,
  side: "right",
  size: "lg",
  className: undefined,
  open: ({
    title,
    description,
    content,
    side = "right",
    size = "lg",
    className,
  }) =>
    set({
      isOpen: true,
      isDirty: false,
      dismissLocks: 0,
      title,
      description,
      content,
      side,
      size,
      className,
    }),
  close: () => set({ isOpen: false, isDirty: false, dismissLocks: 0 }),
  setDirty: (dirty) => set({ isDirty: dirty }),
  blockDismiss: () => set((s) => ({ dismissLocks: s.dismissLocks + 1 })),
  releaseDismiss: () =>
    set((s) => ({ dismissLocks: Math.max(0, s.dismissLocks - 1) })),
}));

export const useSheetOpen = () => useSheetStore((s) => s.open);
export const useSheetClose = () => useSheetStore((s) => s.close);
export const useSheetSetDirty = () => useSheetStore((s) => s.setDirty);
const getSheetIsDirty = () => useSheetStore.getState().isDirty;
export { getSheetIsDirty };

export const useSheetState = () =>
  useSheetStore(
    useShallow((s) => ({
      isOpen: s.isOpen,
      title: s.title,
      description: s.description,
      content: s.content,
      side: s.side,
      size: s.size,
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
