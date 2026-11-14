import { CircleCheck, TriangleAlert } from "lucide-react";

import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import type { ImportApplyResponse } from "@/types/import";

import { ImportWizardFooter } from "./import-wizard-footer";
import type { ImportWizardConfig } from "./import-wizard-types";
import { formatRowList, groupImportFailures } from "./import-wizard-utils";

interface ImportResultStepProps {
  config: ImportWizardConfig;
  result: ImportApplyResponse;
  onRestart: () => void;
  onClose: () => void;
}

export const ImportResultStep = ({
  config,
  result,
  onRestart,
  onClose,
}: ImportResultStepProps) => {
  const failures = groupImportFailures(result.results);
  const hasFailures = result.totalFailed > 0;
  const Icon = hasFailures ? TriangleAlert : CircleCheck;

  return (
    <div className="space-y-4">
      <div className="flex items-center gap-3">
        <Icon
          className={cn("size-8 shrink-0", hasFailures ? "text-warning" : "text-success")}
        />
        <p className="text-sm font-medium">
          {hasFailures
            ? `Đã xử lý xong, có ${result.totalFailed} ${config.itemLabel} chưa được nhập.`
            : `Đã nhập thành công ${result.totalSuccess} ${config.itemLabel}.`}
        </p>
      </div>

      <div className="grid grid-cols-2 gap-3">
        <div className="border border-border p-4">
          <p className="text-xs text-muted-foreground">Thành công</p>
          <p className="text-2xl font-semibold text-success tabular-nums">
            {result.totalSuccess}
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
            {result.totalFailed}
          </p>
        </div>
      </div>

      {failures.length > 0 && (
        <div className="space-y-2">
          <p className="text-sm font-medium">Lý do lỗi</p>
          <ul className="max-h-64 divide-y divide-border overflow-y-auto border border-border">
            {failures.map((group) => (
              <li key={group.reason} className="space-y-0.5 px-3 py-2">
                <p className="text-sm text-destructive">{group.reason}</p>
                {group.rows.length > 0 && (
                  <p className="text-xs text-muted-foreground">
                    {formatRowList(group.rows)}
                  </p>
                )}
              </li>
            ))}
          </ul>
        </div>
      )}

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
};
