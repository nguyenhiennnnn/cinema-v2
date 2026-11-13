import { seatTypeLabel } from "@/components/common/seat-map/seat-kind";
import { resolveSeatVisualKind } from "@/components/common/seat-map/seat-map-types";
import type { SeatTypeDto } from "@/features/admin/catalog/types";
import type { SeatEditorBrush } from "../types/seat-editor";

export const describeBrush = (
  brush: SeatEditorBrush,
  seatTypes: SeatTypeDto[] | undefined,
): { label: string; hint: string } => {
  switch (brush.type) {
    case "seatType": {
      const seatType = seatTypes?.find((t) => t.seatTypeId === brush.seatTypeId);
      const name = seatType?.name ?? "";
      const label = seatType ? seatTypeLabel(name) : "Loại ghế";
      const isCouple = resolveSeatVisualKind(name) === "couple";
      return {
        label: `Gán ${label}`,
        hint: isCouple
          ? "Bấm vào ô ghế để đổi loại. Ghế đôi cần được gộp 2 ghế liền kề bằng công cụ Gộp 2 ghế thành ghế đôi."
          : "Bấm vào ô ghế để đổi sang loại này.",
      };
    }
    case "pairCouple":
      return {
        label: "Gộp 2 ghế thành ghế đôi",
        hint: "Bấm ghế thứ nhất, rồi bấm ghế liền kề cùng hàng. Bấm vào ghế đôi có sẵn để tách lại.",
      };
    case "toggleAisle":
      return {
        label: "Đặt/bỏ lối đi",
        hint: "Bấm vào ô để biến thành lối đi, bấm lần nữa để trả lại thành ghế.",
      };
    case "toggleActive":
      return {
        label: "Bật/tắt ghế",
        hint: "Bấm vào ghế để tắt (không bán), bấm lần nữa để bật lại.",
      };
  }
};
