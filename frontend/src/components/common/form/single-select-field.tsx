"use client";

import { useState } from "react";
import { Check, ChevronsUpDown, Loader2 } from "lucide-react";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
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
import { filterBySearch, isSameValue } from "./select-name-utils";
import { useSelectCreate, type SelectCreateProps } from "./use-select-create";
import { SelectCreateItem, SelectCreateNote } from "./select-create-item";

export interface SingleSelectFieldProps<T> extends SelectCreateProps<T> {
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
  onCreate,
  canCreate,
  createMaxLength,
}: SingleSelectFieldProps<T>) => {
  const [open, setOpen] = useState(false);
  const [picked, setPicked] = useState<T | null>(null);
  const isDisabled = useFieldDisabled(disabled);

  const handleOpenChange = (next: boolean) => {
    setOpen(next);
    if (!next) onSearchChange("");
  };

  const select = (option: T) => {
    setPicked(option);
    onChange(getOptionValue(option), option);
    handleOpenChange(false);
  };

  const createState = useSelectCreate({
    options,
    getOptionLabel,
    searchValue,
    onCreate,
    canCreate,
    createMaxLength,
    onCreated: select,
  });

  const selectedOption =
    createState.allOptions.find((o) => isSameValue(getOptionValue(o), value)) ??
    (picked && isSameValue(getOptionValue(picked), value) ? picked : undefined);
  const displayLabel = selectedOption
    ? getOptionLabel(selectedOption)
    : (selectedLabel ?? "");

  const handleSelect = (option: T) => {
    if (clearable && isSameValue(getOptionValue(option), value)) {
      setPicked(null);
      onChange(null, null);
      handleOpenChange(false);
    } else {
      select(option);
    }
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
              "w-full justify-between font-normal group-data-[invalid=true]/field:border-destructive group-data-[invalid=true]/field:ring-3 group-data-[invalid=true]/field:ring-destructive/20 dark:group-data-[invalid=true]/field:border-destructive/50 dark:group-data-[invalid=true]/field:ring-destructive/40",
              !displayLabel && "text-muted-foreground",
              className,
            )}
          />
        }
      >
        <span className="truncate">{displayLabel || placeholder}</span>
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
                <Loader2 className="size-4 animate-spin text-muted-foreground" />
              </div>
            ) : (
              <>
                <CommandEmpty>{emptyText}</CommandEmpty>
                <CommandGroup>
                  {visibleOptions.map((option) => {
                    const optionValue = getOptionValue(option);
                    return (
                      <CommandItem
                        key={optionValue}
                        value={String(optionValue)}
                        disabled={createState.isCreating}
                        onSelect={() => handleSelect(option)}
                      >
                        <Check
                          className={cn(
                            "mr-2 size-4",
                            isSameValue(value, optionValue)
                              ? "opacity-100"
                              : "opacity-0",
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
