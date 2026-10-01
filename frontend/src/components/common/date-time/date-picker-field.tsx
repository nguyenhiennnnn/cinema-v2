import { useState } from "react";
import type { Matcher } from "react-day-picker";
import { CalendarIcon, XIcon } from "lucide-react";

import { cn } from "@/lib/utils";
import { formatDate, parseDate, toDateInputValue } from "@/lib/formatters";
import { Button } from "@/components/ui/button";
import { Calendar } from "@/components/ui/calendar";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";

interface DatePickerFieldProps {
  value: string | null | undefined;
  onChange: (value: string | null) => void;
  placeholder?: string;
  disabled?: boolean;
  clearable?: boolean;
  captionLayout?: "label" | "dropdown";
  fromYear?: number;
  toYear?: number;
  calendarDisabled?: Matcher | Matcher[];
  className?: string;
  id?: string;
}

export const DatePickerField = ({
  value,
  onChange,
  placeholder = "Chọn ngày",
  disabled,
  clearable = true,
  captionLayout = "label",
  fromYear,
  toYear,
  calendarDisabled,
  className,
  id,
}: DatePickerFieldProps) => {
  const [open, setOpen] = useState(false);
  const selected = parseDate(value);

  const handleSelect = (date: Date | undefined) => {
    onChange(toDateInputValue(date));
    setOpen(false);
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
          <CalendarIcon className="size-4 shrink-0 opacity-60" />
          <span className="truncate">
            {selected ? formatDate(selected) : placeholder}
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
          onSelect={handleSelect}
          disabled={calendarDisabled}
          captionLayout={captionLayout}
          startMonth={fromYear ? new Date(fromYear, 0) : undefined}
          endMonth={toYear ? new Date(toYear, 11) : undefined}
          autoFocus
        />
      </PopoverContent>
    </Popover>
  );
};
