import {
  IMPORT_ACCEPTED_EXTENSIONS,
  IMPORT_MAX_FILE_BYTES,
} from "@/config/default-constants";
import { extractApiError, getDisplayMessage } from "@/lib/error";
import type { ImportApplyResponse } from "@/types/import";

const BYTES_PER_KB = 1024;
const BYTES_PER_MB = BYTES_PER_KB * BYTES_PER_KB;
const MAX_LISTED_ROWS = 20;
const UNKNOWN_REASON = "Không rõ lý do.";

export interface ImportFailureGroup {
  reason: string;
  rows: number[];
}

export function formatFileSize(bytes: number): string {
  if (bytes >= BYTES_PER_MB) return `${(bytes / BYTES_PER_MB).toFixed(1)} MB`;
  return `${Math.max(1, Math.round(bytes / BYTES_PER_KB))} KB`;
}

export function validateImportFile(file: File): string | null {
  const name = file.name.toLowerCase();
  if (!IMPORT_ACCEPTED_EXTENSIONS.some((extension) => name.endsWith(extension))) {
    return `Chỉ hỗ trợ file ${IMPORT_ACCEPTED_EXTENSIONS.join(", ")}.`;
  }
  if (file.size === 0) return "File trống.";
  if (file.size > IMPORT_MAX_FILE_BYTES) {
    return `File vượt quá giới hạn ${IMPORT_MAX_FILE_BYTES / BYTES_PER_MB}MB.`;
  }
  return null;
}

export function describeImportError(error: unknown): string {
  const { errors } = extractApiError(error);
  return (
    errors?.file?.[0] ?? getDisplayMessage(error, { preferServerMessage: true })
  );
}

export function groupImportFailures(
  results: ImportApplyResponse["results"],
): ImportFailureGroup[] {
  const groups = new Map<string, number[]>();
  for (const item of results) {
    if (item.success) continue;
    const reason = item.reason?.trim() || UNKNOWN_REASON;
    const rows = groups.get(reason) ?? [];
    if (typeof item.rowNumber === "number") rows.push(item.rowNumber);
    groups.set(reason, rows);
  }
  return [...groups.entries()].map(([reason, rows]) => ({ reason, rows }));
}

export function formatRowList(rows: number[]): string {
  if (rows.length === 0) return "";
  const shown = rows.slice(0, MAX_LISTED_ROWS).join(", ");
  const rest = rows.length - MAX_LISTED_ROWS;
  return rest > 0 ? `Dòng ${shown} và ${rest} dòng khác` : `Dòng ${shown}`;
}
