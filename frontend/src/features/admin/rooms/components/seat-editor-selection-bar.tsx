import { BoxSelect } from "lucide-react";
import { Button } from "@/components/ui/button";

interface SeatEditorSelectionBarProps {
  selectedCount: number;
  onApply: () => void;
  onClear: () => void;
}

export const SeatEditorSelectionBar = ({
  selectedCount,
  onApply,
  onClear,
}: SeatEditorSelectionBarProps) => (
  <div className="flex flex-wrap items-center gap-2 border border-primary/30 bg-primary/5 px-3 py-2 text-xs">
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
        onClick={onClear}
      >
        Bỏ chọn
      </Button>
      <Button type="button" size="xs" disabled={selectedCount === 0} onClick={onApply}>
        Áp dụng công cụ hiện tại cho vùng đã chọn
      </Button>
    </div>
  </div>
);
