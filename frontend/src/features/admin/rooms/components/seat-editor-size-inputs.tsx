import { useEffect, useState, type KeyboardEvent } from "react";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { SEAT_GRID_MAX_COLS, SEAT_GRID_MAX_ROWS } from "@/config/default-constants";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";

interface SeatEditorSizeInputsProps {
  rows: number;
  cols: number;
  disabled: boolean;
  onResize: (rows: number, cols: number) => void;
}

const clampInt = (value: string, max: number) =>
  Math.min(max, Math.max(1, Math.round(Number(value)) || 1));

export const SeatEditorSizeInputs = ({ rows, cols, disabled, onResize }: SeatEditorSizeInputsProps) => {
  const openAlert = useAlertDialogOpen();
  const [draftRows, setDraftRows] = useState(String(rows));
  const [draftCols, setDraftCols] = useState(String(cols));

  useEffect(() => {
    setDraftRows(String(rows));
  }, [rows]);

  useEffect(() => {
    setDraftCols(String(cols));
  }, [cols]);

  const resetDrafts = () => {
    setDraftRows(String(rows));
    setDraftCols(String(cols));
  };

  const commit = () => {
    const nextRows = clampInt(draftRows, SEAT_GRID_MAX_ROWS);
    const nextCols = clampInt(draftCols, SEAT_GRID_MAX_COLS);
    setDraftRows(String(nextRows));
    setDraftCols(String(nextCols));
    if (nextRows === rows && nextCols === cols) return;

    if (nextRows < rows || nextCols < cols) {
      openAlert({
        title: "Thu nhỏ kích thước phòng?",
        description: `Sơ đồ sẽ giảm từ ${rows} × ${cols} xuống ${nextRows} × ${nextCols}. Các ghế nằm ngoài vùng mới sẽ bị xóa khỏi sơ đồ (có thể Hoàn tác).`,
        variant: "destructive",
        confirmText: "Thu nhỏ",
        onConfirm: () => onResize(nextRows, nextCols),
        onCancel: resetDrafts,
      });
      return;
    }
    onResize(nextRows, nextCols);
  };

  const handleKeyDown = (e: KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter") {
      e.preventDefault();
      commit();
    }
  };

  return (
    <>
      <div className="flex flex-col gap-1.5">
        <Label htmlFor="rows-input">Số hàng</Label>
        <Input
          id="rows-input"
          type="number"
          min={1}
          max={SEAT_GRID_MAX_ROWS}
          value={draftRows}
          disabled={disabled}
          onChange={(e) => setDraftRows(e.target.value)}
          onBlur={commit}
          onKeyDown={handleKeyDown}
          className="w-24"
        />
      </div>
      <div className="flex flex-col gap-1.5">
        <Label htmlFor="cols-input">Số cột</Label>
        <Input
          id="cols-input"
          type="number"
          min={1}
          max={SEAT_GRID_MAX_COLS}
          value={draftCols}
          disabled={disabled}
          onChange={(e) => setDraftCols(e.target.value)}
          onBlur={commit}
          onKeyDown={handleKeyDown}
          className="w-24"
        />
      </div>
    </>
  );
};
