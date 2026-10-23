import { CalendarIcon } from "lucide-react";

import { cn } from "@/lib/utils";
import { formatDate, parseDate, toDateInputValue } from "@/lib/formatters";
import { Button } from "@/components/ui/button";
import { Calendar } from "@/components/ui/calendar";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";

interface DateFilterPickerProps {
  value: string | null;
  onChange: (value: string | null) => void;
  placeholder?: string;
  className?: string;
}

export const DateFilterPicker = ({
  value,
  onChange,
  placeholder = "Chọn ngày",
  className,
}: DateFilterPickerProps) => {
  const selected = parseDate(value);

  return (
    <Popover>
      <PopoverTrigger
        render={
          <Button
            variant="outline"
            data-empty={!selected}
            className={cn(
              "h-10 justify-start text-left font-normal data-[empty=true]:text-muted-foreground",
              className,
            )}
          />
        }
      >
        <CalendarIcon />
        {selected ? formatDate(selected) : <span>{placeholder}</span>}
      </PopoverTrigger>
      <PopoverContent className="w-auto p-0">
        <Calendar
          mode="single"
          selected={selected}
          onSelect={(date) => onChange(toDateInputValue(date))}
        />
      </PopoverContent>
    </Popover>
  );
};