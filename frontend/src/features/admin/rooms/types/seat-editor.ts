export interface EditorCell {
  seatRow: string;
  seatNumber: number;
  seatTypeId: number | null;
  isActive: boolean;
  isAisle: boolean;
  coupleId: string | null;
}

export type SeatEditorBrush =
  | { type: "seatType"; seatTypeId: number }
  | { type: "toggleActive" }
  | { type: "toggleAisle" }
  | { type: "pairCouple" };

export interface ValidationIssue {
  level: "error" | "warning";
  message: string;
}

export interface AutoGenerateOptions {
  rows: number;
  cols: number;
  standardSeatTypeId: number | null;
  vipSeatTypeId: number | null;
  vipRowCount: number;
  coupleRowCount: number;
  aisleCols: number[];
}

export type EditorCellMap = Map<string, EditorCell>;

export interface SeatEditorSnapshot {
  rows: number;
  cols: number;
  cells: EditorCellMap;
}

export interface CopiedRow {
  sourceRow: string;
  cells: EditorCell[];
}
