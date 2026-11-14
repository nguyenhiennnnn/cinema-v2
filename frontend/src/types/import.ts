import type { BulkActionResponse } from "@/types/bulk";

export interface ImportCellError {
  column: string;
  message: string;
}

export interface ImportPreviewRow {
  rowNumber: number;
  data: Record<string, string | null>;
  errors: ImportCellError[];
}

export interface ImportPreviewResponse {
  previewId: string;
  totalRows: number;
  validRows: number;
  invalidRows: number;
  rows: ImportPreviewRow[];
  warnings?: string[] | null;
}

export interface ImportApplyRequest {
  previewId: string;
  skipInvalidRows: boolean;
}

export type ImportApplyResponse = BulkActionResponse;

export interface ImportResultMetric {
  label: string;
  value: number;
}

export interface ImportSummary {
  message: string;
  metrics: ImportResultMetric[];
}

export type ImportApplyOutcome =
  | { mode: "completed"; result: ImportApplyResponse }
  | { mode: "summary"; summary: ImportSummary }
  | { mode: "background"; jobId: string };
