import type {
  ImportExportJobStatus,
  ImportExportJobType,
} from "@/types/enum";

export interface ImportExportJobDto {
  jobId: string;
  type: ImportExportJobType;
  module: string;
  format: string;
  status: ImportExportJobStatus;
  progress: number;
  totalRows: number;
  successRows: number;
  failedRows: number;
  fileName: string | null;
  errorMessage: string | null;
  createdByUserId: number;
  createdAt: string;
  startedAt: string | null;
  completedAt: string | null;
  expiresAt: string | null;
  isExpired: boolean;
  canDownload: boolean;
}

export interface ImportExportJobAccepted {
  jobId: string;
}

export interface ImportExportJobListParams {
  type?: string;
  module?: string;
  status?: string;
  page?: number;
  pageSize?: number;
}
