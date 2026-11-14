import { apiClient, apiGet, apiPost } from "@/api/axios";
import { ENDPOINTS } from "@/api/endpoints";
import { EXPORT_TIMEOUT_MS } from "@/config/default-constants";
import type { ApiResponse, OffsetPageResult } from "@/types/api";
import type {
  ImportExportJobAccepted,
  ImportExportJobDto,
  ImportExportJobListParams,
} from "@/types/import-export";

type ExportJobParams = Record<
  string,
  string | number | boolean | null | undefined
>;

export const importExportApi = {
  getJobs: (params: ImportExportJobListParams) =>
    apiGet<ApiResponse<OffsetPageResult<ImportExportJobDto>>>(
      ENDPOINTS.admin.importExport.jobs,
      { params },
    ),

  getJob: (jobId: string) =>
    apiGet<ApiResponse<ImportExportJobDto>>(
      ENDPOINTS.admin.importExport.jobById(jobId),
    ),

  createExportJob: (endpoint: string, params: ExportJobParams, format: string) =>
    apiPost<ApiResponse<ImportExportJobAccepted>>(endpoint, undefined, {
      params: { ...params, format },
    }),

  downloadJob: (jobId: string) =>
    apiClient.get<Blob>(ENDPOINTS.admin.importExport.jobDownload(jobId), {
      responseType: "blob",
      timeout: EXPORT_TIMEOUT_MS,
    }),
};
