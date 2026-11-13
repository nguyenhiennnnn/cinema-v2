import type { SeatLegendItem } from "./seat-legend";
import { SEAT_KIND_LABEL } from "./seat-kind";

export const TYPE_LEGEND_ITEMS: SeatLegendItem[] = [
  { label: SEAT_KIND_LABEL.standard, state: "available", type: "standard", group: "type" },
  { label: SEAT_KIND_LABEL.vip, state: "available", type: "vip", group: "type" },
  { label: SEAT_KIND_LABEL.couple, state: "available", type: "couple", group: "type" },
];

export const STATUS_LEGEND_ITEMS: SeatLegendItem[] = [
  { label: "Đang chọn", state: "selected", type: "standard", group: "status" },
  { label: "Đang giữ", state: "held", type: "standard", group: "status" },
  { label: "Đã bán", state: "booked", type: "standard", group: "status" },
];

export const EDITOR_LEGEND_ITEMS: SeatLegendItem[] = [
  ...TYPE_LEGEND_ITEMS,
  { label: "Đã tắt (không bán)", state: "inactive", type: "standard" },
  { label: "Lối đi", state: "aisle", type: "standard" },
];

export const PREVIEW_LEGEND_ITEMS: SeatLegendItem[] = [...TYPE_LEGEND_ITEMS, ...STATUS_LEGEND_ITEMS];
