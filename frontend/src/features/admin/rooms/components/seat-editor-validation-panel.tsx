import { AlertTriangle, CheckCircle2, OctagonX, X } from "lucide-react";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import type { ValidationIssue } from "../types/seat-editor";

interface SeatEditorValidationPanelProps {
  issues: ValidationIssue[];
  focusedIndex: number | null;
  onFocusIssue: (index: number) => void;
  onClose: () => void;
}

export const SeatEditorValidationPanel = ({
  issues,
  focusedIndex,
  onFocusIssue,
  onClose,
}: SeatEditorValidationPanelProps) => {
  const errors = issues.filter((i) => i.level === "error");
  const warnings = issues.filter((i) => i.level === "warning");

  return (
    <div className="flex flex-col gap-3 bg-card p-4">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-2 text-sm font-semibold">
          {issues.length === 0 ? (
            <>
              <CheckCircle2 className="size-4 text-success" />
              <span>Không phát hiện vấn đề nào trong sơ đồ ghế.</span>
            </>
          ) : (
            <>
              <AlertTriangle className="size-4 text-warning" />
              <span>
                Phát hiện {errors.length} lỗi và {warnings.length} cảnh báo. Bấm vào từng mục để xem ghế liên quan.
              </span>
            </>
          )}
        </div>
        <Button variant="ghost" size="icon-sm" aria-label="Đóng kết quả kiểm tra" onClick={onClose}>
          <X className="size-4" />
        </Button>
      </div>

      {issues.length > 0 && (
        <ul className="flex max-h-60 flex-col gap-2 overflow-y-auto">
          {issues.map((issue, idx) => {
            const clickable = !!issue.cellKeys && issue.cellKeys.length > 0;
            return (
              <li key={idx}>
                <button
                  type="button"
                  disabled={!clickable}
                  onClick={() => onFocusIssue(idx)}
                  aria-pressed={focusedIndex === idx}
                  className={cn(
                    "flex w-full items-start gap-2 border px-3 py-2 text-left text-xs transition-colors",
                    clickable && "cursor-pointer hover:bg-muted/60",
                    focusedIndex === idx && "ring-2 ring-current",
                    issue.level === "error"
                      ? "border-destructive/40 bg-destructive/5 text-destructive"
                      : "border-warning/40 bg-warning/5 text-warning",
                  )}
                >
                  {issue.level === "error" ? (
                    <OctagonX className="mt-0.5 size-3.5 shrink-0" />
                  ) : (
                    <AlertTriangle className="mt-0.5 size-3.5 shrink-0" />
                  )}
                  <span>{issue.message}</span>
                </button>
              </li>
            );
          })}
        </ul>
      )}
    </div>
  );
};
