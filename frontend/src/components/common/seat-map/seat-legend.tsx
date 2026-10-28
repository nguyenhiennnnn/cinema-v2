import type { SeatCellState, SeatVisualKind } from "./seat-map-types";

export interface SeatLegendItem {
  label: string;
  state?: SeatCellState;
  type?: SeatVisualKind;
}

// ô mẫu 24px; ghế đôi = 2 ô + khoảng hở 4px
const LEGEND_STD = 24;
const LEGEND_GAP = 4;
const LEGEND_COUPLE_W = LEGEND_STD * 2 + LEGEND_GAP;

/** Chú giải dùng chính ô ghế làm mẫu, đặt phía trên sơ đồ. Hẹp thì chia lưới đều, đủ rộng (600px) thì một hàng; cần cha có `@container`. */
export const SeatLegend = ({ items }: { items: SeatLegendItem[] }) => {
  return (
    <ul className="grid grid-cols-2 gap-x-4 gap-y-3 @min-[420px]:grid-cols-3 @min-[600px]:flex @min-[600px]:flex-wrap @min-[600px]:items-center">
      {items.map((item) => (
        <li key={item.label} className="flex items-center gap-2">
          <span
            className="seat pointer-events-none shrink-0"
            data-state={item.state ?? "available"}
            data-type={item.type ?? "standard"}
            style={{
              width: item.type === "couple" ? LEGEND_COUPLE_W : LEGEND_STD,
              height: LEGEND_STD,
            }}
            aria-hidden
          >
            {/* ghế đã tắt nhận biết bằng chữ gạch ngang, nên ô mẫu cần có chữ */}
            {item.state === "inactive" ? "1" : null}
          </span>
          <span className="whitespace-nowrap text-[13px] text-muted-foreground">{item.label}</span>
        </li>
      ))}
    </ul>
  );
};

/** Màn hình: một thanh đặc cao 6px, nhãn câu thường bên dưới. Không đường cong, không ánh sáng. */
export const CinemaScreen = ({ className }: { className?: string }) => {
  return (
    <div className={className}>
      <div className="h-1.5 w-full bg-foreground cut-xs" />
      <p className="mt-2 text-center text-[13px] text-muted-foreground">Màn hình</p>
    </div>
  );
};
