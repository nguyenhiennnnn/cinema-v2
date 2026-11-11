import type { CSSProperties, ReactNode } from "react";
import { cn } from "@/lib/utils";
import { SeatLegend, type SeatLegendItem } from "./seat-legend";

interface SeatMapFrameProps {
  legend: SeatLegendItem[];
  children: ReactNode;
  className?: string;
}

const SPOTLIGHT: CSSProperties = {
  backgroundImage:
    "radial-gradient(ellipse 70% 45% at 50% 28%, color-mix(in oklch, var(--foreground) 5%, transparent), transparent 70%)",
};

export const SeatMapFrame = ({ legend, children, className }: SeatMapFrameProps) => {
  const statusItems = legend.filter((item) => item.group === "status");
  const topItems = legend.filter((item) => item.group !== "status");

  return (
    <div className={cn("@container bg-card p-4 sm:p-6 cut-lg", className)} style={SPOTLIGHT}>
      <div className="mb-6 border-b border-border/60 pb-4">
        <SeatLegend items={topItems} />
      </div>
      {children}
      {statusItems.length > 0 && (
        <div className="mt-4 flex justify-center border-t border-border/60 pt-4">
          <SeatLegend items={statusItems} className="justify-center gap-x-4" />
        </div>
      )}
    </div>
  );
};
