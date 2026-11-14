import { createColumnHelper } from "@tanstack/react-table";
import { Download, Eye, MoreHorizontal, Upload } from "lucide-react";

import { DataTableColumnHeader } from "@/components/common/data-table/data-table-column-header";
import type { DataTableFeatures } from "@/components/common/data-table/data-table-features";
import { Spinner } from "@/components/common/spinner";
import { ImportExportJobStatusBadge } from "@/components/common/status-badge";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { formatDateTime, formatNumber } from "@/lib/formatters";
import { cn } from "@/lib/utils";
import { ImportExportJobType } from "@/types/enum";
import type { ImportExportJobDto } from "@/types/import-export";

import { isJobActive } from "../lib/job-sync";
import {
  getJobCreatorLabel,
  getJobFormatLabel,
  getJobModuleLabel,
  getJobTypeLabel,
} from "../utils/job-labels";
import { JobProgressBar } from "./job-progress-bar";

interface ImportExportJobColumnsOptions {
  currentUserId?: number | null;
  downloadingJobId?: string | null;
  onViewDetail: (job: ImportExportJobDto) => void;
  onDownload: (job: ImportExportJobDto) => void;
}

const columnHelper = createColumnHelper<DataTableFeatures, ImportExportJobDto>();

export const createImportExportJobColumns = ({
  currentUserId,
  downloadingJobId,
  onViewDetail,
  onDownload,
}: ImportExportJobColumnsOptions) =>
  columnHelper.columns([
    columnHelper.accessor("type", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Loại" />,
      cell: ({ row }) => {
        const Icon = row.original.type === ImportExportJobType.Import ? Upload : Download;
        return (
          <span className="inline-flex items-center gap-2 text-sm text-foreground">
            <Icon className="size-4 text-muted-foreground" />
            {getJobTypeLabel(row.original.type)}
          </span>
        );
      },
    }),
    columnHelper.accessor("module", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Module" />,
      cell: ({ row }) => (
        <span className="text-sm text-foreground">{getJobModuleLabel(row.original.module)}</span>
      ),
    }),
    columnHelper.accessor("format", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Định dạng" />,
      cell: ({ row }) => (
        <span className="t-code text-foreground">{getJobFormatLabel(row.original.format)}</span>
      ),
      enableSorting: false,
    }),
    columnHelper.accessor("status", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Trạng thái" />,
      cell: ({ row }) => <ImportExportJobStatusBadge status={row.original.status} />,
      enableSorting: false,
    }),
    columnHelper.accessor("progress", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Tiến độ" />,
      cell: ({ row }) => (
        <div className="flex w-32 items-center gap-2">
          <JobProgressBar
            value={row.original.progress}
            label="Tiến độ tác vụ"
            indeterminate={isJobActive(row.original) && row.original.progress === 0}
          />
          <span className="w-9 shrink-0 text-right text-xs text-muted-foreground tabular-nums">
            {row.original.progress}%
          </span>
        </div>
      ),
      enableSorting: false,
    }),
    columnHelper.display({
      id: "rows",
      header: () => <div>Thành công / lỗi</div>,
      cell: ({ row }) => (
        <span className="text-sm tabular-nums">
          <span className="text-success">{formatNumber(row.original.successRows)}</span>
          <span className="text-muted-foreground"> / </span>
          <span
            className={cn(
              row.original.failedRows > 0 ? "text-destructive" : "text-muted-foreground",
            )}
          >
            {formatNumber(row.original.failedRows)}
          </span>
        </span>
      ),
    }),
    columnHelper.accessor("createdByUserId", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Người tạo" />,
      cell: ({ row }) => (
        <span className="text-sm text-foreground">
          {getJobCreatorLabel(row.original.createdByUserId, currentUserId)}
        </span>
      ),
      enableSorting: false,
    }),
    columnHelper.accessor("createdAt", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Thời gian" />,
      cell: ({ row }) => (
        <span className="text-sm tabular-nums text-muted-foreground">
          {formatDateTime(row.original.createdAt)}
        </span>
      ),
    }),
    columnHelper.accessor("expiresAt", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Hạn tải" />,
      cell: ({ row }) => {
        const job = row.original;
        if (job.type !== ImportExportJobType.Export || !job.expiresAt) {
          return <span className="text-sm text-muted-foreground">—</span>;
        }
        return job.isExpired ? (
          <span className="text-sm text-destructive">Đã hết hạn</span>
        ) : (
          <span className="text-sm tabular-nums text-muted-foreground">
            {formatDateTime(job.expiresAt)}
          </span>
        );
      },
      enableSorting: false,
    }),
    columnHelper.display({
      id: "actions",
      header: () => <div className="text-right">Hành động</div>,
      cell: ({ row }) => {
        const job = row.original;
        const isDownloading = downloadingJobId === job.jobId;
        return (
          <div className="flex items-center justify-end gap-1">
            {job.canDownload && !job.isExpired && (
              <Button
                variant="outline"
                size="sm"
                disabled={isDownloading}
                onClick={() => onDownload(job)}
              >
                {isDownloading ? <Spinner size="sm" decorative /> : <Download />}
                <span>Tải file</span>
              </Button>
            )}
            <DropdownMenu>
              <DropdownMenuTrigger>
                <Button variant="ghost" size="icon" className="size-8">
                  <span className="sr-only">Mở menu</span>
                  <MoreHorizontal className="size-4" />
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end">
                <DropdownMenuItem onClick={() => onViewDetail(job)}>
                  <Eye /> Xem chi tiết
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
          </div>
        );
      },
      enableHiding: false,
    }),
  ]);
