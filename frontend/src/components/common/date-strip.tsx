import { useMemo } from "react";
import { addDays } from "date-fns";
import { cn } from "@/lib/utils";
import { getBusinessToday, getBusinessTodayDate } from "@/lib/business-time";
import { toDateKey, formatWeekdayShort } from "@/lib/cinema-format";

interface DateStripProps {
  value: string;
  onChange: (date: string) => void;
  daysCount?: number;
  className?: string;
}

export const DateStrip = ({ value, onChange, daysCount = 14, className }: DateStripProps) => {
  const days = useMemo(() => {
    const today = getBusinessTodayDate();
    return Array.from({ length: daysCount }, (_, i) => addDays(today, i));
  }, [daysCount]);

  const todayKey = getBusinessToday();

  return (
    <div className={cn("flex gap-2 overflow-x-auto pb-2 scrollbar-none", className)}>
      {days.map((date) => {
        const key = toDateKey(date);
        const active = key === value;
        const isToday = key === todayKey;
        const dayNum = date.getDate();
        const weekday = formatWeekdayShort(date).toLowerCase();

        return (
          <button
            key={key}
            type="button"
            onClick={() => onChange(key)}
            className={cn(
              "flex w-14 shrink-0 flex-col items-center justify-center py-2.5 cut-sm transition-colors cursor-pointer outline-none select-none",
              active
                ? "bg-primary text-primary-foreground"
                : "bg-card text-foreground hover:bg-muted",
              !active && isToday && "border-b-2 border-primary",
            )}
          >
            <span className="t-numeral text-[28px] font-extrabold leading-none">{dayNum}</span>
            <span
              className={cn(
                "mt-1 text-[13px] font-medium leading-tight",
                active ? "text-primary-foreground/90" : "text-muted-foreground",
              )}
            >
              {weekday}
            </span>
          </button>
        );
      })}
    </div>
  );
};
