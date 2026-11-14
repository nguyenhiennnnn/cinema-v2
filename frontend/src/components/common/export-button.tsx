import { useMutation } from "@tanstack/react-query";
import { Download } from "lucide-react";
import { apiClient } from "@/api/axios";
import { Spinner } from "@/components/common/spinner";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { EXPORT_TIMEOUT_MS } from "@/config/default-constants";
import { importExportApi } from "@/features/admin/import-export/api/import-export-api";
import { trackImportExportJob } from "@/features/admin/import-export/lib/job-sync";
import { extractApiError } from "@/lib/error";
import {
  getFilenameFromContentDisposition,
  normalizeBlobError,
  saveBlob,
} from "@/lib/download";
import { cn } from "@/lib/utils";

export type ExportFormat = "xlsx" | "csv" | "pdf";

export type ExportParams = Record<string, string | number | boolean | null | undefined>;

const FORMAT_LABELS: Record<ExportFormat, string> = {
  xlsx: "Excel (.xlsx)",
  csv: "CSV (.csv)",
  pdf: "PDF (.pdf)",
};

const DEFAULT_FORMATS: ExportFormat[] = ["xlsx", "csv"];

const BACKGROUND_EXPORT_STATUS = 422;

interface ExportButtonProps {
  endpoint: string;
  jobEndpoint?: string;
  params?: ExportParams;
  formats?: ExportFormat[];
  fallbackFilename: string;
  label?: string;
  disabled?: boolean;
  size?: "sm" | "default";
  className?: string;
}

export const ExportButton = ({
  endpoint,
  jobEndpoint,
  params,
  formats = DEFAULT_FORMATS,
  fallbackFilename,
  label = "Xuất file",
  disabled = false,
  size = "sm",
  className,
}: ExportButtonProps) => {
  const { mutate, isPending } = useMutation({
    meta: { errorTitle: "Không thể xuất báo cáo", skipSuccessToast: true },
    mutationFn: async (format: ExportFormat): Promise<string | null> => {
      try {
        const response = await apiClient.get<Blob>(endpoint, {
          params: { ...params, format },
          responseType: "blob",
          timeout: EXPORT_TIMEOUT_MS,
        });
        const filename =
          getFilenameFromContentDisposition(
            response.headers["content-disposition"],
          ) ?? `${fallbackFilename}.${format}`;
        saveBlob(response.data, filename);
        return null;
      } catch (error) {
        const normalized = await normalizeBlobError(error);
        if (
          jobEndpoint &&
          extractApiError(normalized).statusCode === BACKGROUND_EXPORT_STATUS
        ) {
          const accepted = await importExportApi.createExportJob(
            jobEndpoint,
            params ?? {},
            format,
          );
          return accepted.data.jobId;
        }
        throw normalized;
      }
    },
    onSuccess: (jobId) => {
      if (jobId) void trackImportExportJob(jobId);
    },
  });

  return (
    <DropdownMenu>
      <DropdownMenuTrigger
        render={
          <Button
            variant="outline"
            size={size}
            disabled={disabled || isPending}
            className={cn("gap-2", className)}
          />
        }
      >
        {isPending ? <Spinner size="sm" decorative /> : <Download />}
        <span>{isPending ? "Đang xuất" : label}</span>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" className="w-44">
        {formats.map((format) => (
          <DropdownMenuItem key={format} onClick={() => mutate(format)}>
            {FORMAT_LABELS[format]}
          </DropdownMenuItem>
        ))}
      </DropdownMenuContent>
    </DropdownMenu>
  );
};
