import { cn } from "@/lib/utils";

export interface SeatLegendItem {
  label: string;
  state?: "available" | "selected" | "held" | "booked" | "inactive" | "aisle";
  type?: "standard" | "vip" | "couple";
  className?: string;
}

// couple icon = 2 × standard icon (20px) + gap (4px) = 44px
const LEGEND_STD = 20;
const LEGEND_GAP = 4;
const LEGEND_COUPLE_W = LEGEND_STD * 2 + LEGEND_GAP;

export const SeatLegend = ({ items }: { items: SeatLegendItem[] }) => {
  return (
    <div className="flex flex-wrap items-center justify-center gap-x-5 gap-y-2.5">
      {items.map((item) => (
        <div key={item.label} className="flex items-center gap-2">
          <span
            className={cn(
              "seat pointer-events-none shrink-0",
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
          <span className="whitespace-nowrap text-[13px] font-medium text-muted-foreground">
            {item.label}
          </span>
        </div>
      ))}
    </div>
  );
};

export const CinemaScreen = () => {
  return (
    <div className="mb-10 flex w-full flex-col items-center gap-2">
      <div className="h-1.5 w-4/5 max-w-lg bg-foreground cut-md" />
      <p className="text-[11px] font-medium tracking-widest text-muted-foreground/60 uppercase">
        Màn hình
      </p>
    </div>
  );
};
