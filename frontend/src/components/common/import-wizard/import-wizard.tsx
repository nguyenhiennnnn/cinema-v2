import { useState } from "react";
import { Check } from "lucide-react";

import {
  useDiscardConfirm,
  useUnsavedChangesGuard,
} from "@/components/common/modal-guard";
import { trackImportExportJob } from "@/features/admin/import-export/lib/job-sync";
import { cn } from "@/lib/utils";
import type { ImportApplyResponse, ImportPreviewResponse } from "@/types/import";

import { ImportBackgroundStep } from "./import-background-step";
import { ImportFileStep } from "./import-file-step";
import { ImportPreviewStep } from "./import-preview-step";
import { ImportResultStep } from "./import-result-step";
import type { ImportWizardConfig } from "./import-wizard-types";

type WizardStep = "file" | "preview" | "result" | "background";

const STEPS: { id: WizardStep; label: string }[] = [
  { id: "file", label: "Chọn file" },
  { id: "preview", label: "Xem trước" },
  { id: "result", label: "Kết quả" },
];

const EXPIRED_NOTICE = "Phiên xem trước đã hết hạn. Vui lòng chọn lại file.";

interface ImportWizardProps {
  config: ImportWizardConfig;
  onClose: () => void;
}

export const ImportWizard = ({ config, onClose }: ImportWizardProps) => {
  const [step, setStep] = useState<WizardStep>("file");
  const [preview, setPreview] = useState<ImportPreviewResponse | null>(null);
  const [result, setResult] = useState<ImportApplyResponse | null>(null);
  const [backgroundJobId, setBackgroundJobId] = useState<string | null>(null);
  const [notice, setNotice] = useState<string | null>(null);

  const isPreviewing = step === "preview";
  useUnsavedChangesGuard(isPreviewing);
  const confirmDiscard = useDiscardConfirm(isPreviewing);

  const restart = (nextNotice: string | null = null) => {
    setPreview(null);
    setResult(null);
    setBackgroundJobId(null);
    setNotice(nextNotice);
    setStep("file");
  };

  const currentIndex = STEPS.findIndex(
    (item) => item.id === (step === "background" ? "result" : step),
  );

  return (
    <div className="space-y-5">
      <ol className="flex items-center gap-2">
        {STEPS.map((item, index) => {
          const isDone = index < currentIndex;
          const isCurrent = index === currentIndex;
          return (
            <li
              key={item.id}
              aria-current={isCurrent ? "step" : undefined}
              className="flex flex-1 items-center gap-2 last:flex-none"
            >
              <span
                className={cn(
                  "flex size-6 shrink-0 items-center justify-center text-xs font-medium",
                  isCurrent || isDone
                    ? "bg-primary text-primary-foreground"
                    : "bg-muted text-muted-foreground",
                )}
              >
                {isDone ? <Check className="size-3.5" /> : index + 1}
              </span>
              <span
                className={cn(
                  "text-sm",
                  isCurrent ? "font-medium text-foreground" : "text-muted-foreground",
                )}
              >
                {item.label}
              </span>
              {index < STEPS.length - 1 && (
                <span className="h-px flex-1 bg-border" aria-hidden />
              )}
            </li>
          );
        })}
      </ol>

      {step === "file" && (
        <ImportFileStep
          config={config}
          notice={notice}
          onPreviewed={(next) => {
            setPreview(next);
            setNotice(null);
            setStep("preview");
          }}
          onCancel={onClose}
        />
      )}

      {step === "preview" && preview && (
        <ImportPreviewStep
          key={preview.previewId}
          config={config}
          preview={preview}
          onApplied={(outcome) => {
            if (outcome.mode === "background") {
              setBackgroundJobId(outcome.jobId);
              setStep("background");
              void trackImportExportJob(outcome.jobId);
              return;
            }
            setResult(outcome.result);
            setStep("result");
          }}
          onExpired={() => restart(EXPIRED_NOTICE)}
          onBack={() => confirmDiscard(() => restart())}
        />
      )}

      {step === "result" && result && (
        <ImportResultStep
          config={config}
          result={result}
          onRestart={() => restart()}
          onClose={onClose}
        />
      )}

      {step === "background" && backgroundJobId && (
        <ImportBackgroundStep
          config={config}
          jobId={backgroundJobId}
          onRestart={() => restart()}
          onClose={onClose}
        />
      )}
    </div>
  );
};
