import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { useFieldDisabled } from "@/components/ui/field";
import { cn } from "@/lib/utils";

interface SelectFieldProps<T> {
  value: string | number | null | undefined;
  onChange: (value: string, option: T | null) => void;
  options: T[];
  getOptionValue: (option: T) => string | number;
  getOptionLabel: (option: T) => string;
  placeholder?: string;
  disabled?: boolean;
  className?: string;
}

export const SelectField = <T,>({
  value,
  onChange,
  options,
  getOptionValue,
  getOptionLabel,
  placeholder = "Chọn...",
  disabled,
  className,
}: SelectFieldProps<T>) => {
  const isDisabled = useFieldDisabled(disabled);

  const handleChange = (val: string) => {
    const option =
      options.find((o) => String(getOptionValue(o)) === val) ?? null;
    onChange(val, option);
  };

  const selectedOption =
    value != null
      ? options.find((o) => String(getOptionValue(o)) === String(value))
      : null;

  return (
    <Select
      value={value != null ? String(value) : undefined}
      onValueChange={(v) => handleChange(v ?? "")}
      disabled={isDisabled}
    >
      <SelectTrigger
        className={cn(
          "w-full group-data-[invalid=true]/field:border-destructive group-data-[invalid=true]/field:ring-3 group-data-[invalid=true]/field:ring-destructive/20 dark:group-data-[invalid=true]/field:border-destructive/50 dark:group-data-[invalid=true]/field:ring-destructive/40",
          className,
        )}
      >
        <SelectValue placeholder={placeholder}>
          {selectedOption ? getOptionLabel(selectedOption) : placeholder}
        </SelectValue>
      </SelectTrigger>
      <SelectContent>
        {options.map((option) => {
          const optionValue = getOptionValue(option);
          return (
            <SelectItem key={optionValue} value={String(optionValue)}>
              {getOptionLabel(option)}
            </SelectItem>
          );
        })}
      </SelectContent>
    </Select>
  );
};