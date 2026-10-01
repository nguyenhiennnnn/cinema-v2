import { AlertTriangle, CheckCircle2, OctagonX, X } from "lucide-react";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import type { ValidationIssue } from "../hooks/use-seat-editor-state";

interface SeatEditorValidationPanelProps {
  issues: ValidationIssue[];
  onClose: () => void;
}

export const SeatEditorValidationPanel = ({ issues, onClose }: SeatEditorValidationPanelProps) => {
  const errors = issues.filter((i) => i.level === "error");
  const warnings = issues.filter((i) => i.level === "warning");

  return (
    <div className="flex flex-col gap-3 rounded-xl border border-border bg-card p-4">
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
                Phát hiện {errors.length} lỗi và {warnings.length} cảnh báo.
              </span>
            </>
          )}
        </div>
        <Button variant="ghost" size="icon-sm" onClick={onClose}>
          <X className="size-4" />
        </Button>
      </div>

      {issues.length > 0 && (
        <ul className="flex flex-col gap-2">
          {issues.map((issue, idx) => (
            <li
              key={idx}
              className={cn(
                "flex items-start gap-2 rounded-lg border px-3 py-2 text-xs",
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
            </li>
          ))}
        </ul>
      )}
    </div>
  );
};
