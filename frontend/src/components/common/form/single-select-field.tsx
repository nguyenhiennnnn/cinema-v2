"use client";

import { useState } from "react";
import { Check, ChevronsUpDown, Loader2 } from "lucide-react";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from "@/components/ui/command";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";

interface SingleSelectFieldProps<T> {
  value: string | number | null | undefined;
  onChange: (value: string | number | null, option: T | null) => void;
  options: T[];
  getOptionValue: (option: T) => string | number;
  getOptionLabel: (option: T) => string;
  selectedLabel?: string;
  searchValue: string;
  onSearchChange: (value: string) => void;
  isLoading?: boolean;
  placeholder?: string;
  searchPlaceholder?: string;
  emptyText?: string;
  disabled?: boolean;
  clearable?: boolean;
  className?: string;
}

export const SingleSelectField = <T,>({
  value,
  onChange,
  options,
  getOptionValue,
  getOptionLabel,
  selectedLabel,
  searchValue,
  onSearchChange,
  isLoading = false,
  placeholder = "Chọn...",
  searchPlaceholder = "Tìm kiếm...",
  emptyText = "Không tìm thấy kết quả.",
  disabled,
  clearable = true,
  className,
}: SingleSelectFieldProps<T>) => {
  const [open, setOpen] = useState(false);

  const selectedOption = options.find((o) => getOptionValue(o) === value);
  const displayLabel = selectedOption
    ? getOptionLabel(selectedOption)
    : (selectedLabel ?? "");

  const handleSelect = (option: T) => {
    const optionValue = getOptionValue(option);
    if (optionValue === value && clearable) {
      onChange(null, null);
    } else {
      onChange(optionValue, option);
    }
    setOpen(false);
  };

  return (
    <Popover open={open} onOpenChange={setOpen}>
      <PopoverTrigger>
        <Button
          type="button"
          variant="outline"
          role="combobox"
          aria-expanded={open}
          disabled={disabled}
          className={cn(
            "w-full justify-between font-normal group-data-[invalid=true]/field:border-destructive group-data-[invalid=true]/field:ring-3 group-data-[invalid=true]/field:ring-destructive/20 dark:group-data-[invalid=true]/field:border-destructive/50 dark:group-data-[invalid=true]/field:ring-destructive/40",
            !displayLabel && "text-muted-foreground",
            className,
          )}
        >
          <span className="truncate">{displayLabel || placeholder}</span>
          <ChevronsUpDown className="ml-2 size-4 shrink-0 opacity-50" />
        </Button>
      </PopoverTrigger>

      <PopoverContent
        className="w-[--radix-popover-trigger-width] p-0"
        align="start"
      >
        <Command shouldFilter={false}>
          <CommandInput
            placeholder={searchPlaceholder}
            value={searchValue}
            onValueChange={onSearchChange}
          />
          <CommandList>
            {isLoading ? (
              <div className="flex items-center justify-center py-6">
                <Loader2 className="size-4 animate-spin text-muted-foreground" />
              </div>
            ) : (
              <>
                <CommandEmpty>{emptyText}</CommandEmpty>
                <CommandGroup>
                  {options.map((option) => {
                    const optionValue = getOptionValue(option);
                    return (
                      <CommandItem
                        key={optionValue}
                        value={String(optionValue)}
                        onSelect={() => handleSelect(option)}
                      >
                        <Check
                          className={cn(
                            "mr-2 size-4",
                            value === optionValue ? "opacity-100" : "opacity-0",
                          )}
                        />
                        <span className="truncate">
                          {getOptionLabel(option)}
                        </span>
                      </CommandItem>
                    );
                  })}
                </CommandGroup>
              </>
            )}
          </CommandList>
        </Command>
      </PopoverContent>
    </Popover>
  );
};
