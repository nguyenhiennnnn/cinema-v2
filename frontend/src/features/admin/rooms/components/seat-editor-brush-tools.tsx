import { Ban, BoxSelect, Copy, Heart, PowerOff, Wand2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import type { SeatTypeDto } from "@/features/admin/catalog/types";
import type { SeatEditorBrush } from "../types/seat-editor";
import { SeatEditorToolButton } from "./seat-editor-tool-button";

interface SeatEditorBrushToolsProps {
  seatTypes: SeatTypeDto[] | undefined;
  brush: SeatEditorBrush;
  onBrushChange: (brush: SeatEditorBrush) => void;
  selectionMode: boolean;
  onToggleSelectionMode: () => void;
  isCopyActive: boolean;
  onToggleCopyRow: () => void;
  onOpenAutoGenerate: () => void;
}

export const SeatEditorBrushTools = ({
  seatTypes,
  brush,
  onBrushChange,
  selectionMode,
  onToggleSelectionMode,
  isCopyActive,
  onToggleCopyRow,
  onOpenAutoGenerate,
}: SeatEditorBrushToolsProps) => (
  <div className="flex flex-col gap-2">
    <Label>Công cụ (chọn rồi bấm vào ô ghế để áp dụng)</Label>
    <div className="flex flex-wrap gap-2">
      {seatTypes?.map((st) => (
        <SeatEditorToolButton
          key={st.seatTypeId}
          active={brush.type === "seatType" && brush.seatTypeId === st.seatTypeId}
          onClick={() => onBrushChange({ type: "seatType", seatTypeId: st.seatTypeId })}
        >
          {st.name}
        </SeatEditorToolButton>
      ))}

      <SeatEditorToolButton
        icon={Heart}
        active={brush.type === "pairCouple"}
        activeClassName="border-couple bg-couple/15 text-couple"
        onClick={() => onBrushChange({ type: "pairCouple" })}
      >
        Ghép ghế đôi
      </SeatEditorToolButton>

      <SeatEditorToolButton
        icon={Ban}
        active={brush.type === "toggleAisle"}
        onClick={() => onBrushChange({ type: "toggleAisle" })}
      >
        Đặt/bỏ lối đi
      </SeatEditorToolButton>

      <SeatEditorToolButton
        icon={PowerOff}
        active={brush.type === "toggleActive"}
        activeClassName="border-destructive bg-destructive/10 text-destructive"
        onClick={() => onBrushChange({ type: "toggleActive" })}
      >
        Bật/tắt ghế
      </SeatEditorToolButton>

      <span className="mx-1 h-6 w-px bg-border" aria-hidden />

      <SeatEditorToolButton icon={BoxSelect} active={selectionMode} onClick={onToggleSelectionMode}>
        Chọn nhiều ô (kéo chuột)
      </SeatEditorToolButton>

      <SeatEditorToolButton icon={Copy} active={isCopyActive} onClick={onToggleCopyRow}>
        Sao chép hàng ghế
      </SeatEditorToolButton>

      <Button type="button" variant="outline" onClick={onOpenAutoGenerate}>
        <Wand2 className="size-4" />
        <span>Tạo layout tự động</span>
      </Button>
    </div>
  </div>
);
