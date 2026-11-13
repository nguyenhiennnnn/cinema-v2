import type { ReactNode } from "react";
import { Ban, BoxSelect, Copy, Heart, Info, PowerOff, Wand2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import type { SeatTypeDto } from "@/features/admin/catalog/types";
import type { SeatEditorBrush } from "../types/seat-editor";
import { seatTypeLabel } from "@/components/common/seat-map/seat-kind";
import { describeBrush } from "../utils/seat-editor-labels";
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

const ToolGroup = ({ label, children }: { label: string; children: ReactNode }) => (
  <div className="flex flex-col gap-1.5">
    <span className="text-xs font-medium text-muted-foreground">{label}</span>
    <div className="flex flex-wrap gap-2">{children}</div>
  </div>
);

export const SeatEditorBrushTools = ({
  seatTypes,
  brush,
  onBrushChange,
  selectionMode,
  onToggleSelectionMode,
  isCopyActive,
  onToggleCopyRow,
  onOpenAutoGenerate,
}: SeatEditorBrushToolsProps) => {
  const current = describeBrush(brush, seatTypes);

  const statusText = isCopyActive
    ? "Đang dùng: Sao chép hàng ghế — bấm nhãn hàng nguồn, rồi bấm nhãn hàng đích để dán."
    : selectionMode
      ? `Đang dùng: Chọn nhiều ô — kéo chuột trên lưới hoặc bấm nhãn hàng/cột để chọn vùng, rồi bấm Áp dụng (${current.label}).`
      : `Đang dùng: ${current.label} — ${current.hint}`;

  return (
    <div className="flex flex-col gap-3">
      <div className="flex flex-wrap gap-x-8 gap-y-3">
        <ToolGroup label="Gán loại ghế">
          {seatTypes?.map((st, index) => (
            <SeatEditorToolButton
              key={st.seatTypeId}
              active={brush.type === "seatType" && brush.seatTypeId === st.seatTypeId}
              onClick={() => onBrushChange({ type: "seatType", seatTypeId: st.seatTypeId })}
              tooltip={describeBrush({ type: "seatType", seatTypeId: st.seatTypeId }, seatTypes).hint}
              shortcut={index < 9 ? String(index + 1) : undefined}
            >
              {seatTypeLabel(st.name)}
            </SeatEditorToolButton>
          ))}
        </ToolGroup>

        <ToolGroup label="Cấu trúc">
          <SeatEditorToolButton
            icon={Heart}
            active={brush.type === "pairCouple"}
            onClick={() => onBrushChange({ type: "pairCouple" })}
            tooltip={describeBrush({ type: "pairCouple" }, seatTypes).hint}
          >
            Gộp 2 ghế thành ghế đôi
          </SeatEditorToolButton>
          <SeatEditorToolButton
            icon={Ban}
            active={brush.type === "toggleAisle"}
            onClick={() => onBrushChange({ type: "toggleAisle" })}
            tooltip={describeBrush({ type: "toggleAisle" }, seatTypes).hint}
          >
            Đặt/bỏ lối đi
          </SeatEditorToolButton>
          <SeatEditorToolButton
            icon={PowerOff}
            active={brush.type === "toggleActive"}
            onClick={() => onBrushChange({ type: "toggleActive" })}
            tooltip={describeBrush({ type: "toggleActive" }, seatTypes).hint}
          >
            Bật/tắt ghế
          </SeatEditorToolButton>
        </ToolGroup>

        <ToolGroup label="Chọn và sao chép">
          <SeatEditorToolButton
            icon={BoxSelect}
            active={selectionMode}
            onClick={onToggleSelectionMode}
            tooltip="Kéo chuột trên lưới hoặc bấm nhãn hàng/cột để chọn nhiều ô, rồi áp dụng công cụ cho cả vùng."
          >
            Chọn nhiều ô
          </SeatEditorToolButton>
          <SeatEditorToolButton
            icon={Copy}
            active={isCopyActive}
            onClick={onToggleCopyRow}
            tooltip="Sao chép cấu hình một hàng ghế sang hàng khác."
          >
            Sao chép hàng ghế
          </SeatEditorToolButton>
        </ToolGroup>

        <ToolGroup label="Tự động">
          <Button type="button" variant="outline" size="sm" onClick={onOpenAutoGenerate}>
            <Wand2 className="size-3.5" />
            <span>Tạo layout tự động</span>
          </Button>
        </ToolGroup>
      </div>

      <p
        role="status"
        aria-live="polite"
        className="flex items-start gap-2 border-l-2 border-foreground bg-muted/50 px-3 py-2 text-xs text-foreground"
      >
        <Info className="mt-0.5 size-3.5 shrink-0 text-muted-foreground" />
        <span>{statusText}</span>
      </p>
    </div>
  );
};
