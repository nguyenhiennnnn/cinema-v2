import { TriangleAlert } from "lucide-react";

import { cn } from "@/lib/utils";
import { ShowtimeStatus } from "@/types/enum";
import {
  CONFLICT_BLOCK,
  getShowtimeStatusMeta,
  SeatFillBar,
} from "./showtime-calendar-shared";

const LEGEND_STATUSES = [
  ShowtimeStatus.Scheduled,
  ShowtimeStatus.Completed,
  ShowtimeStatus.Cancelled,
];

interface ShowtimeCalendarLegendProps {
  editable: boolean;
}

export const ShowtimeCalendarLegend = ({
  editable,
}: ShowtimeCalendarLegendProps) => (
  <div className="flex flex-col gap-2 text-xs text-muted-foreground sm:flex-row sm:items-center sm:justify-between">
    <ul className="flex flex-wrap items-center gap-x-4 gap-y-2">
      {LEGEND_STATUSES.map((status) => {
        const meta = getShowtimeStatusMeta(status);
        const Icon = meta.icon;
        return (
          <li key={status} className="flex items-center gap-1.5">
            <span
              className={cn(
                "flex size-5 items-center justify-center cut-xs",
                meta.block,
              )}
            >
              <Icon className="size-3" aria-hidden />
            </span>
            {meta.label}
          </li>
        );
      })}
      <li className="flex items-center gap-1.5">
        <span
          className={cn(
            "flex size-5 items-center justify-center cut-xs",
            CONFLICT_BLOCK,
          )}
        >
          <TriangleAlert className="size-3" aria-hidden />
        </span>
        Trùng giờ
      </li>
      <li className="flex items-center gap-1.5">
        <span className="h-5 w-8 border border-dashed border-border bg-foreground/5" />
        Khung giờ trống
      </li>
      <li className="flex items-center gap-1.5">
        <span className="w-8">
          <SeatFillBar percent={60} barClassName="bg-primary" />
        </span>
        Ghế đã bán
      </li>
    </ul>
    <p>{editable ? "Bấm vào suất chiếu để sửa." : "Chế độ chỉ xem."}</p>
  </div>
);
