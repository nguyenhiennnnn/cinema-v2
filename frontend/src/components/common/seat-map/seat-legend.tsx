import { cn } from "@/lib/utils";

export interface SeatLegendItem {
  label: string;
  className: string;
}

export const SeatLegend = ({ items }: { items: SeatLegendItem[] }) => {
  return (
    <div className="flex flex-wrap items-center justify-center gap-x-4 gap-y-2 text-xs text-muted-foreground">
      {items.map((item) => (
        <div key={item.label} className="flex items-center gap-1.5">
          <span className={cn("inline-block size-3.5 border", item.className)} />
          <span>{item.label}</span>
        </div>
      ))}
    </div>
  );
};

export const CinemaScreen = () => {
  return (
    <div className="mb-10 flex w-full flex-col items-center">
      <div className="h-2 w-3/4 max-w-xl border-t-2 border-primary/80 [clip-path:polygon(2%_0,98%_0,100%_100%,0_100%)]" />
      <p className="mt-2.5 text-eyebrow">Màn hình</p>
    </div>
  );
};
