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
  open: (options: SheetOptions) => void;
  close: () => void;
}

export const useSheetStore = create<SheetStore>((set) => ({
  isOpen: false,
  title: undefined,
  description: undefined,
  content: null,
  side: "right",
  className: undefined,
  open: ({ title, description, content, side = "right", className }) =>
    set({ isOpen: true, title, description, content, side, className }),
  close: () => set({ isOpen: false }),
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
    })),
  );
