import { useState } from "react";
import type { SeatEditorSnapshot } from "../types/seat-editor";

const HISTORY_LIMIT = 40;

export function useSeatEditorHistory(initial: () => SeatEditorSnapshot) {
  const [present, setPresent] = useState<SeatEditorSnapshot>(initial);
  const [past, setPast] = useState<SeatEditorSnapshot[]>([]);
  const [future, setFuture] = useState<SeatEditorSnapshot[]>([]);
  const [isDirty, setIsDirty] = useState(false);

  const commit = (next: SeatEditorSnapshot) => {
    setPast((prev) => [...prev, present].slice(-HISTORY_LIMIT));
    setFuture([]);
    setPresent(next);
    setIsDirty(true);
  };

  const undo = (): boolean => {
    if (past.length === 0) return false;
    const previous = past[past.length - 1];
    setPast((prev) => prev.slice(0, -1));
    setFuture((prev) => [present, ...prev]);
    setPresent(previous);
    setIsDirty(true);
    return true;
  };

  const redo = (): boolean => {
    if (future.length === 0) return false;
    const next = future[0];
    setFuture((prev) => prev.slice(1));
    setPast((prev) => [...prev, present]);
    setPresent(next);
    setIsDirty(true);
    return true;
  };

  return {
    present,
    commit,
    undo,
    redo,
    canUndo: past.length > 0,
    canRedo: future.length > 0,
    isDirty,
    markSaved: () => setIsDirty(false),
  };
}
