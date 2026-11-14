import { Download } from "lucide-react";

import { Spinner } from "@/components/common/spinner";
import { ImportExportJobStatusBadge } from "@/components/common/status-badge";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { formatDateTime, formatNumber } from "@/lib/formatters";
import { useAuthUser } from "@/stores/auth-store";
import { ImportExportJobType } from "@/types/enum";
import { cn } from "@/lib/utils";

import { useDownloadImportExportJob, useImportExportJob } from "../hooks/use-import-export-jobs";
import { isJobActive } from "../lib/job-sync";
import {
  getJobCreatorLabel,
  getJobFormatLabel,
  getJobModuleLabel,
  getJobTypeLabel,
} from "../utils/job-labels";
import { JobProgressBar } from "./job-progress-bar";

interface ImportExportJobDetailProps {
  jobId: string;
}

const DetailItem = ({ label, value }: { label: string; value: string }) => (
  <div className="space-y-0.5">
    <dt className="text-xs text-muted-foreground">{label}</dt>
    <dd className="text-sm text-foreground">{value}</dd>
  </div>
);

export const ImportExportJobDetail = ({ jobId }: ImportExportJobDetailProps) => {
  const { data: job, isLoading } = useImportExportJob(jobId);
  const user = useAuthUser();
  const download = useDownloadImportExportJob();

  if (isLoading || !job) {
    return (
      <div className="space-y-3">
        <Skeleton className="h-6 w-1/3" />
        <Skeleton className="h-24 w-full" />
        <Skeleton className="h-16 w-full" />
      </div>
    );
  }

  const isImport = job.type === ImportExportJobType.Import;
  const hasFailures = job.failedRows > 0;
  const hasError = hasFailures || Boolean(job.errorMessage);

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-center gap-3">
        <ImportExportJobStatusBadge status={job.status} />
        <p className="text-sm font-medium text-foreground">
          {getJobTypeLabel(job.type)} dữ liệu {getJobModuleLabel(job.module).toLowerCase()}
        </p>
      </div>

      {isJobActive(job) && (
        <div className="space-y-1.5">
          <JobProgressBar
            value={job.progress}
            label="Tiến độ tác vụ"
            indeterminate={job.progress === 0}
          />
          <p className="text-xs text-muted-foreground tabular-nums">{job.progress}%</p>
        </div>
      )}

      <div className="grid grid-cols-3 gap-3">
        <div className="border border-border p-3">
          <p className="text-xs text-muted-foreground">Tổng số dòng</p>
          <p className="text-xl font-semibold tabular-nums">{formatNumber(job.totalRows)}</p>
        </div>
        <div className="border border-border p-3">
          <p className="text-xs text-muted-foreground">Thành công</p>
          <p className="text-xl font-semibold text-success tabular-nums">
            {formatNumber(job.successRows)}
          </p>
        </div>
        <div className="border border-border p-3">
          <p className="text-xs text-muted-foreground">Lỗi</p>
          <p
            className={cn(
              "text-xl font-semibold tabular-nums",
              hasFailures && "text-destructive",
            )}
          >
            {formatNumber(job.failedRows)}
          </p>
        </div>
      </div>

      <dl className="grid grid-cols-2 gap-3">
        <DetailItem label="Mã tác vụ" value={job.jobId} />
        <DetailItem label="Định dạng" value={getJobFormatLabel(job.format)} />
        <DetailItem label="Người tạo" value={getJobCreatorLabel(job.createdByUserId, user?.userId)} />
        <DetailItem label="Tên file" value={job.fileName ?? "—"} />
        <DetailItem label="Tạo lúc" value={formatDateTime(job.createdAt)} />
        <DetailItem label="Bắt đầu" value={formatDateTime(job.startedAt)} />
        <DetailItem label="Hoàn tất" value={formatDateTime(job.completedAt)} />
        {!isImport && (
          <DetailItem
            label="Hạn tải"
            value={job.isExpired ? "Đã hết hạn" : formatDateTime(job.expiresAt)}
          />
        )}
      </dl>

      {hasError && (
        <div className="space-y-2">
          <p className="text-sm font-medium">Chi tiết lỗi</p>
          <div className="space-y-1 border border-border p-3">
            {job.errorMessage && (
              <p className="text-sm break-words text-destructive">{job.errorMessage}</p>
            )}
            {hasFailures && (
              <p className="text-xs text-muted-foreground">
                Có {formatNumber(job.failedRows)} dòng chưa được xử lý.
              </p>
            )}
          </div>
        </div>
      )}

      {job.canDownload && (
        <div className="flex justify-end border-t border-border pt-3">
          <Button
            type="button"
            disabled={download.isPending}
            onClick={() =>
              download.mutate({
                jobId: job.jobId,
                fallbackFilename: job.fileName ?? `${job.module}.${job.format}`,
              })
            }
          >
            {download.isPending ? <Spinner size="sm" decorative className="text-current" /> : <Download />}
            <span>Tải về</span>
          </Button>
        </div>
      )}
    </div>
  );
};
