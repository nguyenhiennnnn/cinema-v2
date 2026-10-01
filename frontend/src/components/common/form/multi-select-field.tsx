"use client";

import { useState } from "react";
import { Check, ChevronsUpDown, Loader2, X } from "lucide-react";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
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

interface MultiSelectFieldProps<T> {
  value: (string | number)[];
  onChange: (values: (string | number)[], options: T[]) => void;
  options: T[];
  getOptionValue: (option: T) => string | number;
  getOptionLabel: (option: T) => string;
  selectedOptions?: T[];
  searchValue: string;
  onSearchChange: (value: string) => void;
  isLoading?: boolean;
  placeholder?: string;
  searchPlaceholder?: string;
  emptyText?: string;
  disabled?: boolean;
  maxBadges?: number;
  className?: string;
}

export const MultiSelectField = <T,>({
  value,
  onChange,
  options,
  getOptionValue,
  getOptionLabel,
  selectedOptions = [],
  searchValue,
  onSearchChange,
  isLoading = false,
  placeholder = "Chọn...",
  searchPlaceholder = "Tìm kiếm...",
  emptyText = "Không tìm thấy kết quả.",
  disabled,
  maxBadges = 3,
  className,
}: MultiSelectFieldProps<T>) => {
  const [open, setOpen] = useState(false);

  const allKnownOptions = [...selectedOptions, ...options].reduce<
    Map<string | number, T>
  >((map, option) => {
    map.set(getOptionValue(option), option);
    return map;
  }, new Map());

  const selectedItems = value
    .map((v) => allKnownOptions.get(v))
    .filter((o): o is T => o !== undefined);

  const toggle = (option: T) => {
    const optionValue = getOptionValue(option);
    const exists = value.includes(optionValue);

    if (exists) {
      const nextValues = value.filter((v) => v !== optionValue);
      const nextOptions = selectedItems.filter(
        (o) => getOptionValue(o) !== optionValue,
      );
      onChange(nextValues, nextOptions);
    } else {
      onChange([...value, optionValue], [...selectedItems, option]);
    }
  };

  const remove = (optionValue: string | number) => {
    onChange(
      value.filter((v) => v !== optionValue),
      selectedItems.filter((o) => getOptionValue(o) !== optionValue),
    );
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
            "h-auto min-h-9 w-full justify-between font-normal group-data-[invalid=true]/field:border-destructive group-data-[invalid=true]/field:ring-3 group-data-[invalid=true]/field:ring-destructive/20 dark:group-data-[invalid=true]/field:border-destructive/50 dark:group-data-[invalid=true]/field:ring-destructive/40",
            className,
          )}
        >
          <div className="flex flex-wrap items-center gap-1">
            {selectedItems.length === 0 && (
              <span className="text-muted-foreground">{placeholder}</span>
            )}
            {selectedItems.slice(0, maxBadges).map((option) => {
              const optionValue = getOptionValue(option);
              return (
                <Badge
                  key={optionValue}
                  variant="secondary"
                  className="gap-1 pr-1"
                >
                  <span className="truncate max-w-30">
                    {getOptionLabel(option)}
                  </span>
                  <span
                    role="button"
                    tabIndex={-1}
                    className="rounded-full p-0.5 hover:bg-muted-foreground/20"
                    onClick={(e) => {
                      e.stopPropagation();
                      remove(optionValue);
                    }}
                  >
                    <X className="size-3" />
                  </span>
                </Badge>
              );
            })}
            {selectedItems.length > maxBadges && (
              <Badge variant="secondary">
                +{selectedItems.length - maxBadges}
              </Badge>
            )}
          </div>
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
                    const isSelected = value.includes(optionValue);
                    return (
                      <CommandItem
                        key={optionValue}
                        value={String(optionValue)}
                        onSelect={() => toggle(option)}
                      >
                        <Check
                          className={cn(
                            "mr-2 size-4",
                            isSelected ? "opacity-100" : "opacity-0",
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
}
