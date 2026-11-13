import { BoxSelect, PowerOff } from "lucide-react";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";

interface SeatEditorSelectionBarProps {
  selectedCount: number;
  brushLabel: string;
  onApply: () => void;
  onClear: () => void;
  onSelectAll: () => void;
  onDisable: () => void;
  className?: string;
}

export const SeatEditorSelectionBar = ({
  selectedCount,
  brushLabel,
  onApply,
  onClear,
  onSelectAll,
  onDisable,
  className,
}: SeatEditorSelectionBarProps) => (
  <div
    className={cn(
      "flex flex-wrap items-center gap-2 border border-primary/40 bg-card px-3 py-2 text-xs",
      className,
    )}
  >
    <BoxSelect className="size-3.5 text-primary" />
    <span className="font-medium text-primary">
      {selectedCount > 0
        ? `Đã chọn ${selectedCount} ô ghế.`
        : "Kéo chuột trên lưới, hoặc bấm nhãn hàng/cột để chọn vùng."}
    </span>
    <div className="ml-auto flex flex-wrap gap-2">
      <Button type="button" size="xs" variant="outline" onClick={onSelectAll}>
        Chọn tất cả
      </Button>
      <Button type="button" size="xs" variant="outline" disabled={selectedCount === 0} onClick={onClear}>
        Bỏ chọn
      </Button>
      <Button type="button" size="xs" variant="outline" disabled={selectedCount === 0} onClick={onDisable}>
        <PowerOff className="size-3" />
        Tắt ghế
      </Button>
      <Button type="button" size="xs" disabled={selectedCount === 0} onClick={onApply}>
        Áp dụng: {brushLabel}
      </Button>
    </div>
  </div>
);
