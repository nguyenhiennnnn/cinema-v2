import { useRef, useState } from "react";
import { Download, FileSpreadsheet, Upload, X } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Spinner } from "@/components/common/spinner";
import { IMPORT_ACCEPT, IMPORT_MAX_FILE_BYTES } from "@/config/default-constants";
import { useBlockDialogClose } from "@/hooks/use-block-dialog-close";
import { cn } from "@/lib/utils";
import type { ImportPreviewResponse } from "@/types/import";

import { ImportWizardFooter } from "./import-wizard-footer";
import type { ImportWizardConfig } from "./import-wizard-types";
import {
  describeImportError,
  formatFileSize,
  validateImportFile,
} from "./import-wizard-utils";
import { useImportPreview, useImportTemplate } from "./use-import-wizard";

interface ImportFileStepProps {
  config: ImportWizardConfig;
  notice: string | null;
  onPreviewed: (preview: ImportPreviewResponse) => void;
  onCancel: () => void;
}

const UPLOAD_COMPLETE_PERCENT = 100;

export const ImportFileStep = ({
  config,
  notice,
  onPreviewed,
  onCancel,
}: ImportFileStepProps) => {
  const inputRef = useRef<HTMLInputElement>(null);
  const [file, setFile] = useState<File | null>(null);
  const [isDragging, setIsDragging] = useState(false);
  const [clientError, setClientError] = useState<string | null>(null);
  const [progress, setProgress] = useState(0);

  const template = useImportTemplate(config);
  const preview = useImportPreview(config);
  const isUploading = preview.isPending;
  useBlockDialogClose(isUploading);

  const selectFile = (candidate: File | undefined) => {
    if (!candidate || isUploading) return;
    preview.reset();
    const error = validateImportFile(candidate);
    if (error) {
      setFile(null);
      setClientError(error);
      return;
    }
    setClientError(null);
    setFile(candidate);
  };

  const removeFile = () => {
    if (isUploading) return;
    preview.reset();
    setFile(null);
    setClientError(null);
    if (inputRef.current) inputRef.current.value = "";
  };

  const handleSubmit = () => {
    if (!file) return;
    setProgress(0);
    preview.mutate({ file, onProgress: setProgress }, { onSuccess: onPreviewed });
  };

  const errorMessage =
    clientError ?? (preview.error ? describeImportError(preview.error) : null);
  const isAnalyzing = isUploading && progress >= UPLOAD_COMPLETE_PERCENT;

  return (
    <div className="space-y-4">
      <div className="flex flex-col gap-3 border border-border bg-muted/40 p-4 sm:flex-row sm:items-center sm:justify-between">
        <p className="text-sm text-muted-foreground">
          Tải file mẫu, điền dữ liệu rồi chọn lại file để kiểm tra trước khi nhập.
        </p>
        <Button
          type="button"
          variant="outline"
          size="sm"
          className="shrink-0 gap-2"
          onClick={() => template.mutate()}
          disabled={template.isPending}
        >
          {template.isPending ? <Spinner size="sm" decorative /> : <Download />}
          <span>Tải file mẫu</span>
        </Button>
      </div>

      {notice && (
        <p className="cut-sm bg-warning/10 px-3 py-2 text-sm text-warning">
          {notice}
        </p>
      )}

      <input
        ref={inputRef}
        type="file"
        accept={IMPORT_ACCEPT}
        className="hidden"
        onChange={(event) => selectFile(event.target.files?.[0])}
      />

      {file ? (
        <div className="space-y-3 border border-border p-4">
          <div className="flex items-center gap-3">
            <FileSpreadsheet className="size-8 shrink-0 text-muted-foreground" />
            <div className="min-w-0 flex-1">
              <p className="truncate text-sm font-medium">{file.name}</p>
              <p className="text-xs text-muted-foreground">
                {formatFileSize(file.size)}
              </p>
            </div>
            <Button
              type="button"
              variant="ghost"
              size="icon-sm"
              onClick={removeFile}
              disabled={isUploading}
            >
              <span className="sr-only">Bỏ file</span>
              <X />
            </Button>
          </div>

          {isUploading && (
            <div className="space-y-1.5">
              <div
                role="progressbar"
                aria-valuemin={0}
                aria-valuemax={100}
                aria-valuenow={isAnalyzing ? undefined : progress}
                className="h-1.5 w-full overflow-hidden bg-muted"
              >
                <div
                  className={cn(
                    "h-full bg-primary transition-[width]",
                    isAnalyzing && "animate-pulse",
                  )}
                  style={{ width: `${progress}%` }}
                />
              </div>
              <p className="text-xs text-muted-foreground">
                {isAnalyzing
                  ? "Đang phân tích và kiểm tra dữ liệu..."
                  : `Đang tải lên... ${progress}%`}
              </p>
            </div>
          )}
        </div>
      ) : (
        <div
          role="button"
          tabIndex={0}
          onClick={() => inputRef.current?.click()}
          onKeyDown={(event) => {
            if (event.key === "Enter" || event.key === " ") {
              event.preventDefault();
              inputRef.current?.click();
            }
          }}
          onDragOver={(event) => {
            event.preventDefault();
            setIsDragging(true);
          }}
          onDragLeave={() => setIsDragging(false)}
          onDrop={(event) => {
            event.preventDefault();
            setIsDragging(false);
            selectFile(event.dataTransfer.files[0]);
          }}
          className={cn(
            "flex cursor-pointer flex-col items-center justify-center gap-2 border-2 border-dashed border-border p-10 text-center transition-colors",
            isDragging && "border-primary bg-accent/10",
          )}
        >
          <Upload className="size-8 text-muted-foreground" />
          <p className="text-sm text-muted-foreground">
            Kéo thả file vào đây hoặc{" "}
            <span className="font-medium text-primary">chọn file</span>
          </p>
          <p className="text-xs text-muted-foreground">
            Hỗ trợ .xlsx, .csv (UTF-8), tối đa{" "}
            {IMPORT_MAX_FILE_BYTES / (1024 * 1024)}MB
          </p>
        </div>
      )}

      {errorMessage && (
        <p
          role="alert"
          className="cut-sm bg-destructive/10 px-3 py-2 text-sm text-destructive"
        >
          {errorMessage}
        </p>
      )}

      <ImportWizardFooter>
        <Button
          type="button"
          variant="outline"
          onClick={onCancel}
          disabled={isUploading}
        >
          Hủy
        </Button>
        <Button type="button" onClick={handleSubmit} disabled={!file || isUploading}>
          {isUploading && <Spinner size="sm" decorative className="text-current" />}
          {isUploading ? "Đang xử lý..." : "Xem trước"}
        </Button>
      </ImportWizardFooter>
    </div>
  );
};
