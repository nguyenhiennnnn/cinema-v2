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
import {
  getFilenameFromContentDisposition,
  normalizeBlobError,
  saveBlob,
} from "@/lib/download";
import { cn } from "@/lib/utils";

export type ExportFormat = "xlsx" | "csv";

export type ExportParams = Record<string, string | number | boolean | null | undefined>;

const FORMAT_OPTIONS: { value: ExportFormat; label: string }[] = [
  { value: "xlsx", label: "Excel (.xlsx)" },
  { value: "csv", label: "CSV (.csv)" },
];

interface ExportButtonProps {
  endpoint: string;
  params?: ExportParams;
  fallbackFilename: string;
  label?: string;
  disabled?: boolean;
  size?: "sm" | "default";
  className?: string;
}

export const ExportButton = ({
  endpoint,
  params,
  fallbackFilename,
  label = "Xuất file",
  disabled = false,
  size = "sm",
  className,
}: ExportButtonProps) => {
  const { mutate, isPending } = useMutation({
    meta: { errorTitle: "Không thể xuất báo cáo", skipSuccessToast: true },
    mutationFn: async (format: ExportFormat) => {
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
      } catch (error) {
        throw await normalizeBlobError(error);
      }
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
        {FORMAT_OPTIONS.map((option) => (
          <DropdownMenuItem key={option.value} onClick={() => mutate(option.value)}>
            {option.label}
          </DropdownMenuItem>
        ))}
      </DropdownMenuContent>
    </DropdownMenu>
  );
};
