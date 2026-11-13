import { Eye, EyeOff, ListChecks, Save } from "lucide-react";
import { Button } from "@/components/ui/button";
import type { SeatTypeDto } from "@/features/admin/catalog/types";
import type { AutoGenerateOptions, SeatEditorBrush } from "../types/seat-editor";
import { SeatEditorAutoGenerateForm } from "./forms/seat-editor-auto-generate-form";
import { SeatEditorBrushTools } from "./seat-editor-brush-tools";
import { SeatEditorCopyRowBar } from "./seat-editor-copy-row-bar";
import { SeatEditorHistoryButtons } from "./seat-editor-history-buttons";
import { SeatEditorSelectionBar } from "./seat-editor-selection-bar";
import { SeatEditorSizeInputs } from "./seat-editor-size-inputs";
import { useDialogClose, useDialogOpen } from "@/stores/dialog-store";

import { FORM_PENDING_LABEL } from "@/config/default-constants";

interface SeatEditorToolbarProps {
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
  selectedCount: number;
  onApplyToSelection: () => void;
  onClearSelection: () => void;

  isPickingCopySource: boolean;
  copySourceRow: string | null;
  onStartCopyRow: () => void;
  onCancelCopyRow: () => void;

  onValidate: () => void;

  defaultSeatTypeId: number | null;
  onAutoGenerate: (options: AutoGenerateOptions) => void;
}

export const SeatEditorToolbar = ({
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
  selectedCount,
  onApplyToSelection,
  onClearSelection,
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
          onGenerate={onAutoGenerate}
          onDone={closeDialog}
        />
      ),
    });
  };

  return (
    <div className="flex flex-col gap-4 bg-card p-4">
      <div className="flex flex-wrap items-end gap-4">
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

        <Button onClick={onSave} disabled={isSaving || previewMode} className="ml-auto">
          <Save className="size-4" />
          <span>{isSaving ? FORM_PENDING_LABEL : "Lưu sơ đồ ghế"}</span>
        </Button>
        {isDirty && (
          <span className="text-xs font-medium text-warning">Có thay đổi chưa lưu</span>
        )}
      </div>

      {!previewMode && (
        <>
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

          {selectionMode && (
            <SeatEditorSelectionBar
              selectedCount={selectedCount}
              onApply={onApplyToSelection}
              onClear={onClearSelection}
            />
          )}

          {isCopyActive && (
            <SeatEditorCopyRowBar
              isPickingSource={isPickingCopySource}
              sourceRow={copySourceRow}
              onCancel={onCancelCopyRow}
            />
          )}
        </>
      )}
    </div>
  );
}
