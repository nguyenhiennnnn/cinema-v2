import { cn } from "@/lib/utils";

export interface SeatLegendItem {
  label: string;
  state?: "available" | "selected" | "held" | "booked" | "inactive" | "aisle";
  type?: "standard" | "vip" | "couple";
  className?: string;
}

// couple icon = 2 × standard icon (20px) + gap (5px) = 45px
const LEGEND_STD = 20;
const LEGEND_GAP = 5;
const LEGEND_COUPLE_W = LEGEND_STD * 2 + LEGEND_GAP;

export const SeatLegend = ({ items }: { items: SeatLegendItem[] }) => {
  return (
    <div className="flex flex-wrap items-center justify-center gap-x-4 gap-y-2 text-xs text-muted-foreground">
      {items.map((item) => (
        <div key={item.label} className="flex items-center gap-1.5">
          <span
            className={cn(
              "seat pointer-events-none shrink-0 text-[9px]",
              item.state === "inactive" && "opacity-30 bg-muted!",
              item.state === "aisle" && "bg-transparent! border border-dashed border-border/50",
              item.className,
            )}
            data-state={
              item.state === "inactive" || item.state === "aisle"
                ? "available"
                : (item.state ?? "available")
            }
            data-type={item.type ?? "standard"}
            style={{
              width: item.type === "couple" ? LEGEND_COUPLE_W : LEGEND_STD,
              height: LEGEND_STD,
            }}
          />
          <span className="whitespace-nowrap font-medium text-foreground/85">{item.label}</span>
        </div>
      ))}
    </div>
  );
};

export const CinemaScreen = () => {
  return (
    <div className="mb-8 flex w-full flex-col items-center">
      <div className="h-1.5 w-full max-w-xl cut-md bg-foreground" />
      <p className="mt-2 text-xs font-medium text-muted-foreground">Màn hình</p>
    </div>
  );
};
