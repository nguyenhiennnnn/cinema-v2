import {
  keepPreviousData,
  useMutation,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";
import { queryKeys } from "@/api/query-keys";
import {
  getFilenameFromContentDisposition,
  normalizeBlobError,
  saveBlob,
} from "@/lib/download";
import type { ImportExportJobListParams } from "@/types/import-export";
import { importExportApi } from "../api/import-export-api";

export function useImportExportJobs(params: ImportExportJobListParams) {
  return useQuery({
    queryKey: queryKeys.adminImportExport.list(params),
    queryFn: () => importExportApi.getJobs(params),
    placeholderData: keepPreviousData,
    select: (res) => res.data,
  });
}

export function useImportExportJob(jobId: string) {
  return useQuery({
    queryKey: queryKeys.adminImportExport.detail(jobId),
    queryFn: () => importExportApi.getJob(jobId),
    enabled: jobId !== "",
    select: (res) => res.data,
  });
}

export function useDownloadImportExportJob() {
  const queryClient = useQueryClient();

  return useMutation({
    meta: { errorTitle: "Không thể tải file", skipSuccessToast: true },
    mutationFn: async ({
      jobId,
      fallbackFilename,
    }: {
      jobId: string;
      fallbackFilename: string;
    }) => {
      try {
        const response = await importExportApi.downloadJob(jobId);
        const filename =
          getFilenameFromContentDisposition(
            response.headers["content-disposition"],
          ) ?? fallbackFilename;
        saveBlob(response.data, filename);
      } catch (error) {
        throw await normalizeBlobError(error);
      }
    },
    onError: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminImportExport.all(),
      }),
  });
}
