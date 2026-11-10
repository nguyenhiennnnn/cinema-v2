"use client";

import { useState } from "react";
import { Check, ChevronsUpDown, X } from "lucide-react";
import { Spinner } from "@/components/common/spinner";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { useFieldDisabled } from "@/components/ui/field";
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
import { filterBySearch } from "./select-name-utils";
import { useSelectCreate, type SelectCreateProps } from "./use-select-create";
import { SelectCreateItem, SelectCreateNote } from "./select-create-item";

export interface MultiSelectFieldProps<T> extends SelectCreateProps<T> {
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
  onCreate,
  canCreate,
  createMaxLength,
}: MultiSelectFieldProps<T>) => {
  const [open, setOpen] = useState(false);
  const [picked, setPicked] = useState<T[]>([]);
  const isDisabled = useFieldDisabled(disabled);

  const handleOpenChange = (next: boolean) => {
    setOpen(next);
    if (!next) onSearchChange("");
  };

  const lookupOptions = (known: T[]) =>
    [...selectedOptions, ...picked, ...known].reduce<Map<string | number, T>>(
      (map, option) => {
        map.set(getOptionValue(option), option);
        return map;
      },
      new Map(),
    );

  const addOption = (option: T, known: T[]) => {
    const optionValue = getOptionValue(option);
    if (value.includes(optionValue)) return;
    setPicked((prev) => [...prev, option]);
    const selected = value
      .map((v) => lookupOptions(known).get(v))
      .filter((o): o is T => o !== undefined);
    onChange([...value, optionValue], [...selected, option]);
  };

  const createState = useSelectCreate({
    options,
    getOptionLabel,
    searchValue,
    onCreate,
    canCreate,
    createMaxLength,
    onCreated: (option) => {
      addOption(option, options);
      onSearchChange("");
    },
  });

  const allKnownOptions = lookupOptions(createState.allOptions);
  const selectedItems = value
    .map((v) => allKnownOptions.get(v))
    .filter((o): o is T => o !== undefined);

  const toggle = (option: T) => {
    const optionValue = getOptionValue(option);
    if (value.includes(optionValue)) {
      remove(optionValue);
    } else {
      addOption(option, createState.allOptions);
    }
  };

  const remove = (optionValue: string | number) => {
    onChange(
      value.filter((v) => v !== optionValue),
      selectedItems.filter((o) => getOptionValue(o) !== optionValue),
    );
  };

  const visibleOptions = filterBySearch(
    createState.allOptions,
    searchValue,
    getOptionLabel,
  );

  return (
    <Popover open={open} onOpenChange={handleOpenChange}>
      <PopoverTrigger
        render={
          <Button
            type="button"
            variant="outline"
            role="combobox"
            aria-expanded={open}
            disabled={isDisabled}
            className={cn(
              "h-auto min-h-9 w-full justify-between font-normal group-data-[invalid=true]/field:border-destructive group-data-[invalid=true]/field:ring-3 group-data-[invalid=true]/field:ring-destructive/20 dark:group-data-[invalid=true]/field:border-destructive/50 dark:group-data-[invalid=true]/field:ring-destructive/40",
              className,
            )}
          />
        }
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
                <span className="max-w-30 truncate">
                  {getOptionLabel(option)}
                </span>
                <span
                  role="button"
                  tabIndex={-1}
                  aria-label={`Bỏ chọn ${getOptionLabel(option)}`}
                  className="cut-xs p-0.5 hover:bg-muted-foreground/20"
                  onClick={(e) => {
                    e.stopPropagation();
                    if (!isDisabled) remove(optionValue);
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
      </PopoverTrigger>

      <PopoverContent className="w-(--anchor-width) p-0" align="start">
        <Command shouldFilter={false}>
          <CommandInput
            placeholder={searchPlaceholder}
            value={searchValue}
            onValueChange={onSearchChange}
            disabled={createState.isCreating}
          />
          <CommandList>
            {isLoading ? (
              <div className="flex items-center justify-center py-6">
                <Spinner size="sm" decorative className="text-muted-foreground" />
              </div>
            ) : (
              <>
                <CommandEmpty>{emptyText}</CommandEmpty>
                <CommandGroup>
                  {visibleOptions.map((option) => {
                    const optionValue = getOptionValue(option);
                    const isSelected = value.includes(optionValue);
                    return (
                      <CommandItem
                        key={optionValue}
                        value={String(optionValue)}
                        disabled={createState.isCreating}
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
                  {createState.showCreate && (
                    <SelectCreateItem
                      term={createState.term}
                      isCreating={createState.isCreating}
                      onCreate={createState.create}
                    />
                  )}
                </CommandGroup>
              </>
            )}
          </CommandList>
          <SelectCreateNote
            message={createState.createError ?? createState.termError}
          />
        </Command>
      </PopoverContent>
    </Popover>
  );
};
