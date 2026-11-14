import { ChevronDown, ChevronUp, Download, ListChecks, Upload, X } from "lucide-react";
import { Link } from "react-router";

import { Spinner } from "@/components/common/spinner";
import { Button } from "@/components/ui/button";
import { ROUTES } from "@/config/routes";
import { formatNumber } from "@/lib/formatters";
import { cn } from "@/lib/utils";
import {
  useImportExportJobStore,
  type TrackedImportExportJob,
} from "@/stores/import-export-job-store";
import { ImportExportJobStatus, ImportExportJobType } from "@/types/enum";
import type { ImportExportJobDto } from "@/types/import-export";

import { useImportExportJobDetailDialog } from "../hooks/use-import-export-job-detail-dialog";
import { useDownloadImportExportJob } from "../hooks/use-import-export-jobs";
import { isJobActive } from "../lib/job-sync";
import { getJobTitle } from "../utils/job-labels";
import { JobProgressBar } from "./job-progress-bar";

const describeJob = (job: ImportExportJobDto): string => {
  const isImport = job.type === ImportExportJobType.Import;

  if (job.status === ImportExportJobStatus.Queued) return "Đang chờ xử lý";
  if (job.status === ImportExportJobStatus.Running) return `Đang xử lý ${job.progress}%`;
  if (job.status === ImportExportJobStatus.Failed) {
    return job.errorMessage ?? "Tác vụ thất bại";
  }
  if (!isImport) {
    return job.isExpired
      ? "File đã hết hạn"
      : `Đã xuất ${formatNumber(job.totalRows)} dòng`;
  }
  return job.failedRows > 0
    ? `Đã nhập ${formatNumber(job.successRows)} dòng, ${formatNumber(job.failedRows)} dòng lỗi`
    : `Đã nhập ${formatNumber(job.successRows)} dòng`;
};

const JobPanelRow = ({ item }: { item: TrackedImportExportJob }) => {
  const { job } = item;
  const dismiss = useImportExportJobStore((s) => s.dismiss);
  const download = useDownloadImportExportJob();
  const openDetail = useImportExportJobDetailDialog();

  if (!job) {
    return (
      <li className="space-y-2 px-3 py-3">
        <p className="text-sm font-medium text-foreground">Đang khởi tạo tác vụ</p>
        <JobProgressBar value={0} indeterminate label="Tiến độ tác vụ" />
      </li>
    );
  }

  const Icon = job.type === ImportExportJobType.Import ? Upload : Download;
  const active = isJobActive(job);
  const failed = job.status === ImportExportJobStatus.Failed;
  const canDownload = job.canDownload && !job.isExpired;
  const showResult =
    job.status === ImportExportJobStatus.Completed && job.type === ImportExportJobType.Import;

  return (
    <li className="space-y-2 px-3 py-3">
      <div className="flex items-start gap-2">
        <Icon className="mt-0.5 size-4 shrink-0 text-muted-foreground" />
        <div className="min-w-0 flex-1">
          <p className="truncate text-sm font-medium text-foreground">{getJobTitle(job)}</p>
          <p
            className={cn(
              "line-clamp-2 text-xs text-muted-foreground",
              failed && "text-destructive",
            )}
          >
            {describeJob(job)}
          </p>
        </div>
        {!active && (
          <Button
            variant="ghost"
            size="icon-xs"
            aria-label="Ẩn tác vụ"
            onClick={() => dismiss(job.jobId)}
          >
            <X />
          </Button>
        )}
      </div>

      {active && (
        <JobProgressBar
          value={job.progress}
          indeterminate={job.status === ImportExportJobStatus.Queued}
          label={`Tiến độ ${getJobTitle(job)}`}
        />
      )}

      {!active && (canDownload || showResult || failed) && (
        <div className="flex gap-2">
          {canDownload && (
            <Button
              size="sm"
              disabled={download.isPending}
              onClick={() =>
                download.mutate({
                  jobId: job.jobId,
                  fallbackFilename: job.fileName ?? `${job.module}.${job.format}`,
                })
              }
            >
              {download.isPending ? (
                <Spinner size="sm" decorative className="text-current" />
              ) : (
                <Download />
              )}
              <span>Tải về</span>
            </Button>
          )}
          {(showResult || failed) && (
            <Button variant="outline" size="sm" onClick={() => openDetail(job.jobId)}>
              {failed ? "Xem chi tiết" : "Xem kết quả"}
            </Button>
          )}
        </div>
      )}
    </li>
  );
};

export const JobProgressPanel = () => {
  const items = useImportExportJobStore((s) => s.items);
  const expanded = useImportExportJobStore((s) => s.expanded);
  const setExpanded = useImportExportJobStore((s) => s.setExpanded);
  const dismissFinished = useImportExportJobStore((s) => s.dismissFinished);

  if (items.length === 0) return null;

  const activeJobs = items.filter((item) => !item.job || isJobActive(item.job));
  const hasFinished = activeJobs.length < items.length;
  const firstActive = activeJobs[0]?.job ?? null;
  const ToggleIcon = expanded ? ChevronDown : ChevronUp;

  return (
    <section
      aria-label="Tác vụ nền"
      className="fixed right-4 bottom-4 z-60 w-[calc(100vw-2rem)] max-w-sm cut-md border border-border bg-card text-card-foreground"
    >
      <header className="flex items-center gap-1 px-3 py-1.5">
        <button
          type="button"
          aria-expanded={expanded}
          onClick={() => setExpanded(!expanded)}
          className="flex min-w-0 flex-1 items-center gap-2 py-1.5 text-left text-sm font-medium text-foreground"
        >
          {activeJobs.length > 0 ? (
            <Spinner size="sm" decorative />
          ) : (
            <ListChecks className="size-4 shrink-0" />
          )}
          <span className="truncate">
            {activeJobs.length > 0
              ? `${activeJobs.length} tác vụ đang chạy`
              : "Tác vụ nền đã xong"}
          </span>
          <ToggleIcon className="ml-auto size-4 shrink-0 text-muted-foreground" />
        </button>
        <Button
          variant="ghost"
          size="icon-sm"
          aria-label="Ẩn các tác vụ đã xong"
          disabled={!hasFinished}
          onClick={dismissFinished}
        >
          <X />
        </Button>
      </header>

      {!expanded && firstActive && (
        <div className="px-3 pb-3">
          <JobProgressBar
            value={firstActive.progress}
            indeterminate={firstActive.status === ImportExportJobStatus.Queued}
            label="Tiến độ tác vụ"
          />
        </div>
      )}

      {expanded && (
        <>
          <ul className="max-h-80 divide-y divide-border overflow-y-auto border-t border-border">
            {items.map((item) => (
              <JobPanelRow key={item.jobId} item={item} />
            ))}
          </ul>
          <div className="border-t border-border px-3 py-2">
            <Link
              to={ROUTES.admin.importExportHistory}
              className="text-sm text-primary underline-offset-4 hover:underline"
            >
              Xem lịch sử Import/Export
            </Link>
          </div>
        </>
      )}
    </section>
  );
};
