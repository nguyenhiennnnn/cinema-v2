import { ClipboardPaste, X } from "lucide-react";
import { Button } from "@/components/ui/button";

interface SeatEditorCopyRowBarProps {
  isPickingSource: boolean;
  sourceRow: string | null;
  onCancel: () => void;
}

export const SeatEditorCopyRowBar = ({
  isPickingSource,
  sourceRow,
  onCancel,
}: SeatEditorCopyRowBarProps) => (
  <div className="flex flex-wrap items-center gap-2 border border-accent/40 bg-accent/10 px-3 py-2 text-xs">
    <ClipboardPaste className="size-3.5" />
    <span className="font-medium">
      {isPickingSource
        ? "Bấm vào nhãn của hàng ghế muốn sao chép (bên trái lưới)."
        : `Đã sao chép hàng ${sourceRow}. Bấm vào nhãn hàng đích trong lưới để dán cấu hình.`}
    </span>
    <Button type="button" size="xs" variant="outline" className="ml-auto" onClick={onCancel}>
      <X className="size-3.5" />
      Hủy
    </Button>
  </div>
);
