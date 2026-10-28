import type { ReactNode } from "react";
import { cn } from "@/lib/utils";
import { SeatLegend, type SeatLegendItem } from "./seat-legend";

interface SeatMapFrameProps {
  legend: SeatLegendItem[];
  children: ReactNode;
  className?: string;
}

/**
 * Khung chung của mọi sơ đồ ghế (đặt vé, quầy bán vé, trình soạn thảo phòng):
 * khối `card` vát `cut-lg`, không viền, chú giải ở trên, sơ đồ ở dưới.
 */
export const SeatMapFrame = ({ legend, children, className }: SeatMapFrameProps) => {
  return (
    <div className={cn("@container bg-card p-4 sm:p-6 cut-lg", className)}>
      <div className="mb-8">
        <SeatLegend items={legend} />
      </div>
      {children}
    </div>
  );
};
