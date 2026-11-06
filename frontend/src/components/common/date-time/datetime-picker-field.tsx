import { useState } from "react";
import type { Matcher } from "react-day-picker";
import { CalendarClockIcon, ClockIcon, XIcon } from "lucide-react";

import { cn } from "@/lib/utils";
import { getBusinessNow, toWallDate } from "@/lib/business-time";
import { combineDateAndTime, toTimeValue } from "@/lib/wall-time";
import { formatDateTime, toDateTimeInputValue } from "@/lib/formatters";
import { Button } from "@/components/ui/button";
import { Calendar } from "@/components/ui/calendar";
import { InputGroup, InputGroupAddon, InputGroupInput } from "@/components/ui/input-group";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";

interface DateTimePickerFieldProps {
  value: string | null | undefined;
  onChange: (value: string | null) => void;
  placeholder?: string;
  disabled?: boolean;
  clearable?: boolean;
  calendarDisabled?: Matcher | Matcher[];
  minuteStep?: number;
  className?: string;
  id?: string;
}

export const DateTimePickerField = ({
  value,
  onChange,
  placeholder = "Chọn ngày và giờ",
  disabled,
  clearable = true,
  calendarDisabled,
  minuteStep = 1,
  className,
  id,
}: DateTimePickerFieldProps) => {
  const [open, setOpen] = useState(false);
  const selected = toWallDate(value);
  const timeValue = toTimeValue(selected);

  const handleDateSelect = (date: Date | undefined) => {
    if (!date) {
      onChange(null);
      return;
    }
    const base = selected ?? getBusinessNow();
    let timeStr = toTimeValue(base);
    if (!selected) {
      const nowH = base.getHours();
      if (nowH < 8 || nowH >= 23) {
        timeStr = "10:00";
      }
    }
    onChange(toDateTimeInputValue(combineDateAndTime(date, timeStr)));
  };

  const handleTimeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const time = e.target.value;
    if (!time) return;
    const base = selected ?? getBusinessNow();
    onChange(toDateTimeInputValue(combineDateAndTime(base, time)));
  };

  const handleClear = (e: React.MouseEvent) => {
    e.stopPropagation();
    onChange(null);
  };

  return (
    <Popover open={open} onOpenChange={setOpen}>
      <PopoverTrigger
        render={
          <Button
            type="button"
            id={id}
            variant="outline"
            disabled={disabled}
            data-empty={!selected}
            className={cn(
              "h-10 w-full justify-between font-normal group-data-[invalid=true]/field:border-destructive group-data-[invalid=true]/field:ring-3 group-data-[invalid=true]/field:ring-destructive/20 data-[empty=true]:text-muted-foreground dark:group-data-[invalid=true]/field:border-destructive/50 dark:group-data-[invalid=true]/field:ring-destructive/40",
              className,
            )}
          />
        }
      >
        <span className="flex items-center gap-2 truncate">
          <CalendarClockIcon className="size-4 shrink-0 opacity-60" />
          <span className="truncate">
            {selected ? formatDateTime(selected) : placeholder}
          </span>
        </span>
        {clearable && selected && !disabled && (
          <XIcon
            className="size-3.5 shrink-0 opacity-60 hover:opacity-100"
            onClick={handleClear}
          />
        )}
      </PopoverTrigger>
      <PopoverContent className="w-auto p-0" align="start">
        <Calendar
          mode="single"
          selected={selected}
          onSelect={handleDateSelect}
          disabled={calendarDisabled}
          autoFocus
        />
        <div className="flex items-center gap-2 border-t p-3">
          <InputGroup>
            <InputGroupAddon>
              <ClockIcon />
            </InputGroupAddon>
            <InputGroupInput
              type="time"
              step={minuteStep * 60}
              value={timeValue}
              onChange={handleTimeChange}
              disabled={!selected}
            />
          </InputGroup>
          <Button type="button" size="sm" onClick={() => setOpen(false)} disabled={!selected}>
            Xong
          </Button>
        </div>
      </PopoverContent>
    </Popover>
  );
};
