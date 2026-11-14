import { CircleCheck, OctagonX, TriangleAlert } from "lucide-react";
import { useNavigate } from "react-router";

import { Spinner } from "@/components/common/spinner";
import { Button } from "@/components/ui/button";
import { ROUTES } from "@/config/routes";
import { JobProgressBar } from "@/features/admin/import-export/components/job-progress-bar";
import { useImportExportJob } from "@/features/admin/import-export/hooks/use-import-export-jobs";
import { isJobActive } from "@/features/admin/import-export/lib/job-sync";
import { formatNumber } from "@/lib/formatters";
import { cn } from "@/lib/utils";
import { ImportExportJobStatus } from "@/types/enum";

import { ImportWizardFooter } from "./import-wizard-footer";
import type { ImportWizardConfig } from "./import-wizard-types";

interface ImportBackgroundStepProps {
  config: ImportWizardConfig;
  jobId: string;
  onRestart: () => void;
  onClose: () => void;
}

export const ImportBackgroundStep = ({
  config,
  jobId,
  onRestart,
  onClose,
}: ImportBackgroundStepProps) => {
  const { data: job } = useImportExportJob(jobId);
  const navigate = useNavigate();

  const isActive = !job || isJobActive(job);
  const isFailed = job?.status === ImportExportJobStatus.Failed;
  const hasFailures = (job?.failedRows ?? 0) > 0;
  const Icon = isFailed ? OctagonX : hasFailures ? TriangleAlert : CircleCheck;

  const message = isActive
    ? "Dữ liệu lớn nên việc nhập được xử lý nền. Bạn có thể đóng cửa sổ này, hệ thống sẽ báo khi hoàn tất."
    : isFailed
      ? (job?.errorMessage ?? "Không thể hoàn tất việc nhập dữ liệu.")
      : hasFailures
        ? `Đã xử lý xong, có ${formatNumber(job?.failedRows)} ${config.itemLabel} chưa được nhập.`
        : `Đã nhập thành công ${formatNumber(job?.successRows)} ${config.itemLabel}.`;

  const handleViewHistory = () => {
    onClose();
    navigate(ROUTES.admin.importExportHistory);
  };

  return (
    <div className="space-y-4">
      <div className="flex items-center gap-3">
        {isActive ? (
          <Spinner size="md" decorative />
        ) : (
          <Icon
            className={cn(
              "size-8 shrink-0",
              isFailed ? "text-destructive" : hasFailures ? "text-warning" : "text-success",
            )}
          />
        )}
        <p className="text-sm font-medium">{message}</p>
      </div>

      {isActive && (
        <div className="space-y-1.5">
          <JobProgressBar
            value={job?.progress ?? 0}
            indeterminate={!job || job.progress === 0}
            label="Tiến độ nhập dữ liệu"
          />
          <p className="text-xs text-muted-foreground tabular-nums">
            {job ? `${job.progress}%` : "Đang chờ xử lý"}
          </p>
        </div>
      )}

      {!isActive && job && (
        <div className="grid grid-cols-2 gap-3">
          <div className="border border-border p-4">
            <p className="text-xs text-muted-foreground">Thành công</p>
            <p className="text-2xl font-semibold text-success tabular-nums">
              {formatNumber(job.successRows)}
            </p>
          </div>
          <div className="border border-border p-4">
            <p className="text-xs text-muted-foreground">Thất bại</p>
            <p
              className={cn(
                "text-2xl font-semibold tabular-nums",
                hasFailures && "text-destructive",
              )}
            >
              {formatNumber(job.failedRows)}
            </p>
          </div>
        </div>
      )}

      <ImportWizardFooter>
        {!isActive && (
          <Button type="button" variant="outline" onClick={onRestart}>
            Nhập file khác
          </Button>
        )}
        <Button type="button" variant="outline" onClick={handleViewHistory}>
          Xem lịch sử
        </Button>
        <Button type="button" onClick={onClose}>
          Đóng
        </Button>
      </ImportWizardFooter>
    </div>
  );
};
