import { useState } from "react";
import { set } from "date-fns";
import type { Matcher } from "react-day-picker";
import { CalendarClockIcon, ClockIcon, XIcon } from "lucide-react";

import { cn } from "@/lib/utils";
import { formatDateTime, parseDate, toDateTimeInputValue } from "@/lib/formatters";
import { Button } from "@/components/ui/button";
import { Calendar } from "@/components/ui/calendar";
import { InputGroup, InputGroupAddon, InputGroupInput } from "@/components/ui/input-group";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";

function combineDateAndTime(date: Date, time: string): Date {
  const [hours, minutes] = time.split(":").map((n) => Number(n) || 0);
  return set(date, { hours, minutes, seconds: 0, milliseconds: 0 });
}

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
  const selected = parseDate(value);
  const timeValue = selected
    ? `${String(selected.getHours()).padStart(2, "0")}:${String(selected.getMinutes()).padStart(2, "0")}`
    : "";

  const handleDateSelect = (date: Date | undefined) => {
    if (!date) {
      onChange(null);
      return;
    }
    const base = selected ?? new Date();
    onChange(toDateTimeInputValue(combineDateAndTime(date, `${base.getHours()}:${base.getMinutes()}`)));
  };

  const handleTimeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const time = e.target.value;
    if (!time) return;
    const base = selected ?? new Date();
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
              "w-full justify-between font-normal group-data-[invalid=true]/field:border-destructive group-data-[invalid=true]/field:ring-3 group-data-[invalid=true]/field:ring-destructive/20 data-[empty=true]:text-muted-foreground dark:group-data-[invalid=true]/field:border-destructive/50 dark:group-data-[invalid=true]/field:ring-destructive/40",
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
