import { useMutation } from "@tanstack/react-query";
import { apiClient, apiUpload } from "@/api/axios";
import { EXPORT_TIMEOUT_MS, IMPORT_TIMEOUT_MS } from "@/config/default-constants";
import {
  getFilenameFromContentDisposition,
  normalizeBlobError,
  saveBlob,
} from "@/lib/download";
import type { ApiResponse } from "@/types/api";
import type { ImportExportJobAccepted } from "@/types/import-export";
import type {
  ImportApplyOutcome,
  ImportApplyRequest,
  ImportApplyResponse,
  ImportPreviewResponse,
} from "@/types/import";
import type { ImportWizardConfig } from "./import-wizard-types";

async function downloadFile(url: string, fallbackFilename: string) {
  try {
    const response = await apiClient.get<Blob>(url, {
      responseType: "blob",
      timeout: EXPORT_TIMEOUT_MS,
    });
    const filename =
      getFilenameFromContentDisposition(
        response.headers["content-disposition"],
      ) ?? fallbackFilename;
    saveBlob(response.data, filename);
  } catch (error) {
    throw await normalizeBlobError(error);
  }
}

export function useImportTemplate(config: ImportWizardConfig) {
  return useMutation({
    meta: { errorTitle: "Không thể tải file mẫu", skipSuccessToast: true },
    mutationFn: () =>
      downloadFile(`${config.baseUrl}/template`, config.templateFilename),
  });
}

export function useImportErrorFile(
  config: ImportWizardConfig,
  previewId: string,
) {
  return useMutation({
    meta: { errorTitle: "Không thể tải file lỗi", skipSuccessToast: true },
    mutationFn: () =>
      downloadFile(
        `${config.baseUrl}/errors/${encodeURIComponent(previewId)}`,
        config.errorsFilename,
      ),
  });
}

export function useImportPreview(config: ImportWizardConfig) {
  return useMutation({
    meta: { skipErrorToast: true, skipSuccessToast: true },
    mutationFn: async ({
      file,
      onProgress,
    }: {
      file: File;
      onProgress: (percent: number) => void;
    }) => {
      const formData = new FormData();
      formData.append("file", file);
      const res = await apiUpload<ApiResponse<ImportPreviewResponse>>(
        `${config.baseUrl}/preview`,
        formData,
        {
          timeout: IMPORT_TIMEOUT_MS,
          onUploadProgress: (event) =>
            onProgress(Math.round((event.progress ?? 0) * 100)),
        },
      );
      return res.data;
    },
  });
}

export function useImportApply(config: ImportWizardConfig) {
  return useMutation({
    meta: {
      invalidates: config.invalidates,
      skipSuccessToast: true,
      errorTitle: "Không thể nhập dữ liệu",
    },
    mutationFn: async (body: ImportApplyRequest): Promise<ImportApplyOutcome> => {
      const res = await apiClient.post<
        ApiResponse<ImportApplyResponse | ImportExportJobAccepted>
      >(`${config.baseUrl}/apply`, body, { timeout: IMPORT_TIMEOUT_MS });
      const data = res.data.data;
      return "jobId" in data
        ? { mode: "background", jobId: data.jobId }
        : { mode: "completed", result: data };
    },
  });
}
