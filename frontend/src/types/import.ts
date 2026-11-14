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
}

export interface ImportApplyRequest {
  previewId: string;
  skipInvalidRows: boolean;
}

export type ImportApplyResponse = BulkActionResponse;
