import { Eye, EyeOff, ListChecks, Save, Upload } from "lucide-react";
import { ENDPOINTS } from "@/api/endpoints";
import { ExportButton } from "@/components/common/export-button";
import { ImportWizard } from "@/components/common/import-wizard/import-wizard";
import { Button } from "@/components/ui/button";
import type { SeatTypeDto } from "@/features/admin/catalog/types";
import type { AutoGenerateOptions, SeatEditorBrush } from "../types/seat-editor";
import { SeatEditorAutoGenerateForm } from "./forms/seat-editor-auto-generate-form";
import { SeatEditorBrushTools } from "./seat-editor-brush-tools";
import { SeatEditorCopyRowBar } from "./seat-editor-copy-row-bar";
import { SeatEditorHistoryButtons } from "./seat-editor-history-buttons";
import { SeatEditorSizeInputs } from "./seat-editor-size-inputs";
import { createSeatLayoutImportConfig } from "../config/seat-layout-import-config";
import { useDialogClose, useDialogOpen } from "@/stores/dialog-store";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";

import { FORM_PENDING_LABEL } from "@/config/default-constants";

interface SeatEditorToolbarProps {
  roomId: number;
  onImported: () => void;
  seatTypes: SeatTypeDto[] | undefined;
  brush: SeatEditorBrush;
  onBrushChange: (brush: SeatEditorBrush) => void;
  rows: number;
  cols: number;
  onResize: (rows: number, cols: number) => void;
  onSave: () => void;
  isSaving?: boolean;
  isDirty: boolean;

  canUndo: boolean;
  canRedo: boolean;
  onUndo: () => void;
  onRedo: () => void;

  previewMode: boolean;
  onTogglePreview: () => void;

  selectionMode: boolean;
  onToggleSelectionMode: () => void;

  isPickingCopySource: boolean;
  copySourceRow: string | null;
  onStartCopyRow: () => void;
  onCancelCopyRow: () => void;

  onValidate: () => void;

  defaultSeatTypeId: number | null;
  onAutoGenerate: (options: AutoGenerateOptions) => void;
}

export const SeatEditorToolbar = ({
  roomId,
  onImported,
  seatTypes,
  brush,
  onBrushChange,
  rows,
  cols,
  onResize,
  onSave,
  isSaving,
  isDirty,
  canUndo,
  canRedo,
  onUndo,
  onRedo,
  previewMode,
  onTogglePreview,
  selectionMode,
  onToggleSelectionMode,
  isPickingCopySource,
  copySourceRow,
  onStartCopyRow,
  onCancelCopyRow,
  onValidate,
  defaultSeatTypeId,
  onAutoGenerate,
}: SeatEditorToolbarProps) => {
  const editingDisabled = previewMode;
  const isCopyActive = isPickingCopySource || !!copySourceRow;

  const openDialog = useDialogOpen();
  const closeDialog = useDialogClose();
  const openAlert = useAlertDialogOpen();

  const handleGenerate = (options: AutoGenerateOptions) => {
    openAlert({
      title: "Thay thế toàn bộ sơ đồ ghế?",
      description: `Layout mới ${options.rows} hàng × ${options.cols} cột sẽ ghi đè sơ đồ hiện tại. Bạn vẫn có thể Hoàn tác (Ctrl+Z) sau khi tạo.`,
      variant: "destructive",
      confirmText: "Tạo layout",
      onConfirm: () => onAutoGenerate(options),
    });
  };

  const handleOpen = () => {
    openDialog({
      title: "Tạo layout ghế tự động",
      description:
        "Nhập kích thước phòng, layout sẽ được tạo sẵn để bạn chỉnh tay tiếp. Thao tác này thay thế toàn bộ sơ đồ hiện tại (có thể bấm Hoàn tác nếu cần).",
      size: "md",
      content: (
        <SeatEditorAutoGenerateForm
          rows={rows}
          cols={cols}
          seatTypes={seatTypes}
          defaultSeatTypeId={defaultSeatTypeId}
          onGenerate={handleGenerate}
          onDone={closeDialog}
        />
      ),
    });
  };

  const openImport = () => {
    const config = createSeatLayoutImportConfig(roomId);
    openDialog({
      title: config.title,
      description: config.description,
      size: "xl",
      className: "sm:max-w-5xl",
      content: (
        <ImportWizard config={config} onClose={closeDialog} onCompleted={onImported} />
      ),
    });
  };

  const handleImport = () => {
    if (!isDirty) {
      openImport();
      return;
    }
    openAlert({
      title: "Nhập sơ đồ ghế từ file?",
      description:
        "Sơ đồ trong file sẽ thay thế sơ đồ hiện tại của phòng. Các thay đổi chưa lưu trong trình chỉnh sửa sẽ bị mất.",
      variant: "destructive",
      confirmText: "Tiếp tục",
      onConfirm: openImport,
    });
  };

  return (
    <>
      <div className="sticky top-16 z-20 flex flex-wrap items-end gap-4 bg-card p-4 shadow-sm">
        <SeatEditorSizeInputs
          rows={rows}
          cols={cols}
          disabled={editingDisabled}
          onResize={onResize}
        />
        <SeatEditorHistoryButtons
          disabled={editingDisabled}
          canUndo={canUndo}
          canRedo={canRedo}
          onUndo={onUndo}
          onRedo={onRedo}
        />

        <Button
          type="button"
          variant={previewMode ? "default" : "outline"}
          onClick={onTogglePreview}
        >
          {previewMode ? <EyeOff className="size-4" /> : <Eye className="size-4" />}
          <span>{previewMode ? "Thoát xem trước" : "Xem trước"}</span>
        </Button>

        <Button
          type="button"
          variant="outline"
          disabled={editingDisabled}
          onClick={onValidate}
        >
          <ListChecks className="size-4" />
          <span>Kiểm tra layout</span>
        </Button>

        <Button
          type="button"
          variant="outline"
          disabled={editingDisabled}
          onClick={handleImport}
        >
          <Upload className="size-4" />
          <span>Import</span>
        </Button>

        <ExportButton
          endpoint={ENDPOINTS.admin.rooms.seatLayoutExport(roomId)}
          fallbackFilename="seat-layout"
          label="Export"
          size="default"
        />

        <div className="ml-auto flex items-center gap-3">
          {isDirty && (
            <span className="flex items-center gap-1.5 text-xs font-medium text-warning">
              <span className="size-2 rounded-full bg-warning" aria-hidden />
              Có thay đổi chưa lưu
            </span>
          )}
          <Button onClick={onSave} disabled={isSaving || previewMode} title="Lưu (Ctrl+S)">
            <Save className="size-4" />
            <span>{isSaving ? FORM_PENDING_LABEL : "Lưu sơ đồ ghế"}</span>
            {isDirty && !isSaving && (
              <span className="size-1.5 rounded-full bg-primary-foreground" aria-hidden />
            )}
          </Button>
        </div>
      </div>

      {!previewMode && (
        <div className="flex flex-col gap-3 bg-card p-4">
          <SeatEditorBrushTools
            seatTypes={seatTypes}
            brush={brush}
            onBrushChange={onBrushChange}
            selectionMode={selectionMode}
            onToggleSelectionMode={onToggleSelectionMode}
            isCopyActive={isCopyActive}
            onToggleCopyRow={isCopyActive ? onCancelCopyRow : onStartCopyRow}
            onOpenAutoGenerate={handleOpen}
          />

          {isCopyActive && (
            <SeatEditorCopyRowBar
              isPickingSource={isPickingCopySource}
              sourceRow={copySourceRow}
              onCancel={onCancelCopyRow}
            />
          )}
        </div>
      )}
    </>
  );
};
