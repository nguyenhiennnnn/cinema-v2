import { useMemo } from "react";
import { cn } from "@/lib/utils";
import { toDateKey, formatDayMonth, formatWeekdayShort } from "@/lib/cinema-format";

interface DateStripProps {
  value: string;
  onChange: (date: string) => void;
  daysCount?: number;
  className?: string;
}

export const DateStrip = ({ value, onChange, daysCount = 14, className }: DateStripProps) => {
  const days = useMemo(() => {
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    return Array.from({ length: daysCount }, (_, i) => {
      const date = new Date(today);
      date.setDate(date.getDate() + i);
      return date;
    });
  }, [daysCount]);

  return (
    <div className={cn("flex gap-2 overflow-x-auto pb-2", className)}>
      {days.map((date) => {
        const key = toDateKey(date);
        const active = key === value;
        return (
          <button
            key={key}
            type="button"
            onClick={() => onChange(key)}
            className={cn(
              "flex min-w-16 shrink-0 flex-col items-center gap-0.5 border px-3 py-2 transition-colors",
              active
                ? "border-primary bg-primary text-primary-foreground"
                : "border-border bg-card text-foreground hover:border-primary/50 hover:bg-muted",
            )}
          >
            <span className="text-[10px] font-semibold tracking-wide uppercase opacity-70">{formatWeekdayShort(date)}</span>
            <span className="font-heading text-base font-semibold tabular">{formatDayMonth(date)}</span>
          </button>
        );
      })}
    </div>
  );
}