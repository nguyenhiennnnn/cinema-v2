import {
  Ban,
  BoxSelect,
  ClipboardPaste,
  Copy,
  Eye,
  EyeOff,
  Heart,
  ListChecks,
  PowerOff,
  Redo2,
  Save,
  Undo2,
  Wand2,
  X,
} from "lucide-react";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import type { SeatTypeDto } from "@/features/admin/catalog/types";
import type { AutoGenerateOptions, SeatEditorBrush } from "../hooks/use-seat-editor-state";
import { SeatEditorAutoGenerateForm } from "./forms/seat-editor-auto-generate-form";
import { useDialogClose, useDialogOpen } from "@/stores/dialog-store";

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

  // undo/redo
  canUndo: boolean;
  canRedo: boolean;
  onUndo: () => void;
  onRedo: () => void;

  // preview mode
  previewMode: boolean;
  onTogglePreview: () => void;

  // multi-select + bulk edit
  selectionMode: boolean;
  onToggleSelectionMode: () => void;
  selectedCount: number;
  onApplyToSelection: () => void;
  onClearSelection: () => void;

  // copy/paste row
  isPickingCopySource: boolean;
  copySourceRow: string | null;
  onStartCopyRow: () => void;
  onCancelCopyRow: () => void;

  // validation
  onValidate: () => void;

  // auto-generate
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
  const isSeatTypeBrush = (id: number) => brush.type === "seatType" && brush.seatTypeId === id;
  const editingDisabled = previewMode;

  const openDialog = useDialogOpen();
  const closeDialog = useDialogClose();

  const handleOpen = () => {
    openDialog({
      title: "Tạo layout ghế tự động",
      description:
        "Nhập kích thước phòng, layout sẽ được tạo sẵn để bạn chỉnh tay tiếp. Thao tác này thay thế toàn bộ sơ đồ hiện tại (có thể bấm Hoàn tác nếu cần).",
      className: "sm:max-w-md",
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
    <div className="flex flex-col gap-4 rounded-xl border border-border bg-card p-4">
      <div className="flex flex-wrap items-end gap-4">
        <div className="flex flex-col gap-1.5">
          <Label htmlFor="rows-input">Số hàng</Label>
          <Input
            id="rows-input"
            type="number"
            min={1}
            max={52}
            value={rows}
            disabled={editingDisabled}
            onChange={(e) => onResize(Math.max(1, Number(e.target.value) || 1), cols)}
            className="w-24"
          />
        </div>
        <div className="flex flex-col gap-1.5">
          <Label htmlFor="cols-input">Số cột</Label>
          <Input
            id="cols-input"
            type="number"
            min={1}
            max={60}
            value={cols}
            disabled={editingDisabled}
            onChange={(e) => onResize(rows, Math.max(1, Number(e.target.value) || 1))}
            className="w-24"
          />
        </div>

        <div className="flex items-center gap-1">
          <Button
            type="button"
            variant="outline"
            size="icon"
            title="Hoàn tác"
            disabled={editingDisabled || !canUndo}
            onClick={onUndo}
          >
            <Undo2 className="size-4" />
          </Button>
          <Button
            type="button"
            variant="outline"
            size="icon"
            title="Làm lại"
            disabled={editingDisabled || !canRedo}
            onClick={onRedo}
          >
            <Redo2 className="size-4" />
          </Button>
        </div>

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
          <span>{isSaving ? "Đang lưu..." : "Lưu sơ đồ ghế"}</span>
        </Button>
        {isDirty && (
          <span className="text-xs font-medium text-warning">Có thay đổi chưa lưu</span>
        )}
      </div>

      {!previewMode && (
        <>
          <div className="flex flex-col gap-2">
            <Label>Công cụ (chọn rồi bấm vào ô ghế để áp dụng)</Label>
            <div className="flex flex-wrap gap-2">
              {seatTypes?.map((st) => (
                <button
                  key={st.seatTypeId}
                  type="button"
                  onClick={() => onBrushChange({ type: "seatType", seatTypeId: st.seatTypeId })}
                  className={cn(
                    "rounded-lg border px-3 py-1.5 text-xs font-medium transition-all",
                    isSeatTypeBrush(st.seatTypeId)
                      ? "border-primary bg-primary text-primary-foreground"
                      : "border-border bg-card text-foreground hover:bg-muted",
                  )}
                >
                  {st.name}
                </button>
              ))}

              <button
                type="button"
                onClick={() => onBrushChange({ type: "pairCouple" })}
                className={cn(
                  "flex items-center gap-1.5 rounded-lg border px-3 py-1.5 text-xs font-medium transition-all",
                  brush.type === "pairCouple"
                    ? "border-couple bg-couple/15 text-couple"
                    : "border-border bg-card text-foreground hover:bg-muted",
                )}
              >
                <Heart className="size-3.5" />
                Ghép ghế đôi
              </button>

              <button
                type="button"
                onClick={() => onBrushChange({ type: "toggleAisle" })}
                className={cn(
                  "flex items-center gap-1.5 rounded-lg border px-3 py-1.5 text-xs font-medium transition-all",
                  brush.type === "toggleAisle"
                    ? "border-primary bg-primary text-primary-foreground"
                    : "border-border bg-card text-foreground hover:bg-muted",
                )}
              >
                <Ban className="size-3.5" />
                Đặt/bỏ lối đi
              </button>

              <button
                type="button"
                onClick={() => onBrushChange({ type: "toggleActive" })}
                className={cn(
                  "flex items-center gap-1.5 rounded-lg border px-3 py-1.5 text-xs font-medium transition-all",
                  brush.type === "toggleActive"
                    ? "border-destructive bg-destructive/10 text-destructive"
                    : "border-border bg-card text-foreground hover:bg-muted",
                )}
              >
                <PowerOff className="size-3.5" />
                Bật/tắt ghế
              </button>

              <span className="mx-1 h-6 w-px bg-border" aria-hidden />

              <button
                type="button"
                onClick={onToggleSelectionMode}
                className={cn(
                  "flex items-center gap-1.5 rounded-lg border px-3 py-1.5 text-xs font-medium transition-all",
                  selectionMode
                    ? "border-primary bg-primary text-primary-foreground"
                    : "border-border bg-card text-foreground hover:bg-muted",
                )}
              >
                <BoxSelect className="size-3.5" />
                Chọn nhiều ô (kéo chuột)
              </button>

              <button
                type="button"
                onClick={isPickingCopySource || copySourceRow ? onCancelCopyRow : onStartCopyRow}
                className={cn(
                  "flex items-center gap-1.5 rounded-lg border px-3 py-1.5 text-xs font-medium transition-all",
                  isPickingCopySource || copySourceRow
                    ? "border-primary bg-primary text-primary-foreground"
                    : "border-border bg-card text-foreground hover:bg-muted",
                )}
              >
                <Copy className="size-3.5" />
                Sao chép hàng ghế
              </button>

              <Button type="button" variant="outline" onClick={handleOpen}>
                <Wand2 className="size-4" />
                <span>Tạo layout tự động</span>
              </Button>
            </div>
          </div>

          {selectionMode && (
            <div className="flex flex-wrap items-center gap-2 rounded-lg border border-primary/30 bg-primary/5 px-3 py-2 text-xs">
              <BoxSelect className="size-3.5 text-primary" />
              <span className="font-medium text-primary">
                {selectedCount > 0
                  ? `Đã chọn ${selectedCount} ô ghế.`
                  : "Kéo chuột trên lưới ghế để chọn một vùng."}
              </span>
              <div className="ml-auto flex gap-2">
                <Button
                  type="button"
                  size="xs"
                  variant="outline"
                  disabled={selectedCount === 0}
                  onClick={onClearSelection}
                >
                  Bỏ chọn
                </Button>
                <Button
                  type="button"
                  size="xs"
                  disabled={selectedCount === 0}
                  onClick={onApplyToSelection}
                >
                  Áp dụng công cụ hiện tại cho vùng đã chọn
                </Button>
              </div>
            </div>
          )}

          {(isPickingCopySource || copySourceRow) && (
            <div className="flex flex-wrap items-center gap-2 rounded-lg border border-accent/40 bg-accent/10 px-3 py-2 text-xs">
              <ClipboardPaste className="size-3.5" />
              <span className="font-medium">
                {isPickingCopySource
                  ? "Bấm vào nhãn của hàng ghế muốn sao chép (bên trái lưới)."
                  : `Đã sao chép hàng ${copySourceRow}. Bấm vào nhãn hàng đích trong lưới để dán cấu hình.`}
              </span>
              <Button
                type="button"
                size="xs"
                variant="outline"
                className="ml-auto"
                onClick={onCancelCopyRow}
              >
                <X className="size-3.5" />
                Hủy
              </Button>
            </div>
          )}
        </>
      )}
    </div>
  );
}
