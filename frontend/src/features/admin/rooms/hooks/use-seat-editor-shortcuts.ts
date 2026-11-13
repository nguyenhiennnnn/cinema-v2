import { useEffect, useRef } from "react";
import type { SeatTypeDto } from "@/features/admin/catalog/types";
import type { SeatEditorBrush } from "../types/seat-editor";

interface UseSeatEditorShortcutsParams {
  enabled: boolean;
  seatTypes: SeatTypeDto[];
  hasSelection: boolean;
  onSelectBrush: (brush: SeatEditorBrush) => void;
  onUndo: () => void;
  onRedo: () => void;
  onSave: () => void;
  onSelectAll: () => void;
  onDisableSelection: () => void;
  onEscape: () => void;
}

const isTypingTarget = (target: EventTarget | null) => {
  const el = target as HTMLElement | null;
  if (!el) return false;
  return (
    el.tagName === "INPUT" ||
    el.tagName === "TEXTAREA" ||
    el.tagName === "SELECT" ||
    el.isContentEditable
  );
};

export function useSeatEditorShortcuts(params: UseSeatEditorShortcutsParams) {
  const latest = useRef(params);

  useEffect(() => {
    latest.current = params;
  });

  useEffect(() => {
    if (!params.enabled) return;

    const handler = (e: KeyboardEvent) => {
      if (isTypingTarget(e.target)) return;
      if (document.querySelector("[role='dialog'], [role='alertdialog']")) return;

      const p = latest.current;
      const key = e.key.toLowerCase();
      const mod = e.ctrlKey || e.metaKey;

      if (mod) {
        if (key === "z") {
          e.preventDefault();
          if (e.shiftKey) p.onRedo();
          else p.onUndo();
        } else if (key === "y") {
          e.preventDefault();
          p.onRedo();
        } else if (key === "s") {
          e.preventDefault();
          p.onSave();
        } else if (key === "a") {
          e.preventDefault();
          p.onSelectAll();
        }
        return;
      }
      if (e.altKey) return;

      if (key === "escape") {
        p.onEscape();
      } else if (key === "delete" || key === "backspace") {
        if (p.hasSelection) {
          e.preventDefault();
          p.onDisableSelection();
        }
      } else if (/^[1-9]$/.test(key)) {
        const seatType = p.seatTypes[Number(key) - 1];
        if (seatType) p.onSelectBrush({ type: "seatType", seatTypeId: seatType.seatTypeId });
      }
    };

    window.addEventListener("keydown", handler);
    return () => window.removeEventListener("keydown", handler);
  }, [params.enabled]);
}
