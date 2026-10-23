import { cn } from "@/lib/utils";

export interface SeatLegendItem {
  label: string;
  state?: "available" | "selected" | "held" | "booked";
  type?: "standard" | "vip" | "couple";
  className?: string;
}

export const SeatLegend = ({ items }: { items: SeatLegendItem[] }) => {
  return (
    <div className="flex flex-wrap items-center justify-center gap-x-5 gap-y-2 text-xs text-muted-foreground">
      {items.map((item) => (
        <div key={item.label} className="flex items-center gap-2">
          <span
            className={cn("seat size-5 pointer-events-none text-[9px]", item.className)}
            data-state={item.state ?? "available"}
            data-type={item.type ?? "standard"}
          />
          <span className="font-medium text-foreground/85">{item.label}</span>
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
