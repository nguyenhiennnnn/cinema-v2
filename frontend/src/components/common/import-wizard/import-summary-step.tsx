import { CircleCheck } from "lucide-react";

import { Button } from "@/components/ui/button";
import { formatNumber } from "@/lib/formatters";
import type { ImportSummary } from "@/types/import";

import { ImportWizardFooter } from "./import-wizard-footer";

interface ImportSummaryStepProps {
  summary: ImportSummary;
  onRestart: () => void;
  onClose: () => void;
}

export const ImportSummaryStep = ({
  summary,
  onRestart,
  onClose,
}: ImportSummaryStepProps) => (
  <div className="space-y-4">
    <div className="flex items-center gap-3">
      <CircleCheck className="size-8 shrink-0 text-success" />
      <p className="text-sm font-medium">{summary.message}</p>
    </div>

    <div className="grid grid-cols-2 gap-3 sm:grid-cols-3">
      {summary.metrics.map((metric) => (
        <div key={metric.label} className="border border-border p-4">
          <p className="text-xs text-muted-foreground">{metric.label}</p>
          <p className="text-2xl font-semibold tabular-nums">
            {formatNumber(metric.value)}
          </p>
        </div>
      ))}
    </div>

    <ImportWizardFooter>
      <Button type="button" variant="outline" onClick={onRestart}>
        Nhập file khác
      </Button>
      <Button type="button" onClick={onClose}>
        Đóng
      </Button>
    </ImportWizardFooter>
  </div>
);
