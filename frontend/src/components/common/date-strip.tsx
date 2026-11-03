import { useMemo, useRef } from "react";
import { addDays } from "date-fns";
import { ChevronLeft, ChevronRight } from "lucide-react";
import { cn } from "@/lib/utils";
import { getBusinessToday, getBusinessTodayDate } from "@/lib/business-time";
import { toDateKey, formatWeekdayShort } from "@/lib/cinema-format";
import { Button } from "@/components/ui/button";

interface DateStripProps {
  value: string;
  onChange: (date: string) => void;
  daysCount?: number;
  availableDates?: string[];
  className?: string;
}

const SCROLL_STEP = 4 * 64;

export const DateStrip = ({ value, onChange, daysCount = 14, availableDates, className }: DateStripProps) => {
  const scrollRef = useRef<HTMLDivElement>(null);

  const days = useMemo(() => {
    const today = getBusinessTodayDate();
    return Array.from({ length: daysCount }, (_, i) => addDays(today, i));
  }, [daysCount]);

  const availableSet = useMemo(() => (availableDates ? new Set(availableDates) : null), [availableDates]);
  const todayKey = getBusinessToday();

  const scrollBy = (direction: -1 | 1) =>
    scrollRef.current?.scrollBy({ left: direction * SCROLL_STEP, behavior: "smooth" });

  return (
    <div className={cn("flex items-center gap-2", className)}>
      <Button
        variant="ghost"
        size="icon-sm"
        aria-label="Các ngày trước"
        className="hidden md:inline-flex"
        onClick={() => scrollBy(-1)}
      >
        <ChevronLeft className="size-4" />
      </Button>
      <div ref={scrollRef} className="flex min-w-0 flex-1 gap-2 overflow-x-auto pb-2 scrollbar-none">
        {days.map((date, index) => {
          const key = toDateKey(date);
          const active = key === value;
          const isToday = key === todayKey;
          const unavailable = availableSet ? !availableSet.has(key) : false;
          const dayNum = date.getDate();
          const weekday = formatWeekdayShort(date).toLowerCase();
          const showMonth = index === 0 || dayNum === 1;

          return (
            <button
              key={key}
              type="button"
              disabled={unavailable}
              onClick={() => onChange(key)}
              className={cn(
                "flex w-14 shrink-0 flex-col items-center justify-center py-2 cut-sm transition-colors cursor-pointer outline-none select-none",
                active
                  ? "bg-primary text-primary-foreground"
                  : "bg-card text-foreground hover:bg-muted",
                !active && isToday && "border-b-2 border-primary",
                unavailable && "cursor-not-allowed opacity-40 hover:bg-card",
              )}
            >
              <span
                className={cn(
                  "text-[11px] font-semibold uppercase leading-none",
                  !showMonth && "invisible",
                  active ? "text-primary-foreground/90" : "text-primary",
                )}
              >
                Th{date.getMonth() + 1}
              </span>
              <span className="t-numeral mt-1 text-[28px] font-extrabold leading-none">{dayNum}</span>
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
      <Button
        variant="ghost"
        size="icon-sm"
        aria-label="Các ngày sau"
        className="hidden md:inline-flex"
        onClick={() => scrollBy(1)}
      >
        <ChevronRight className="size-4" />
      </Button>
    </div>
  );
};
