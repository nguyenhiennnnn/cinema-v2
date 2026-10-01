import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import type { EnumOption } from "@/lib/enum-options";
import { cn } from "@/lib/utils";

interface EnumSelectFilterProps {
  value: string | null;
  onChange: (value: string | null) => void;
  options: EnumOption[];
  placeholder: string;
  allLabel?: string;
  className?: string;
}

export const EnumSelectFilter = ({
  value,
  onChange,
  options,
  placeholder,
  allLabel = "Tất cả",
  className,
}: EnumSelectFilterProps) => {
  const currentValue = value ?? "all";
  const currentLabel =
    currentValue === "all"
      ? allLabel
      : options.find((o) => o.value === currentValue)?.label ?? placeholder;

  return (
    <Select
      value={currentValue}
      onValueChange={(v) => onChange(v === "all" ? null : v)}
    >
      <SelectTrigger className={cn("w-full", className)}>
        <SelectValue placeholder={placeholder}>{currentLabel}</SelectValue>
      </SelectTrigger>
      <SelectContent>
        <SelectItem value="all">{allLabel}</SelectItem>
        {options.map((option) => (
          <SelectItem key={option.value} value={option.value}>
            {option.label}
          </SelectItem>
        ))}
      </SelectContent>
    </Select>
  );
};