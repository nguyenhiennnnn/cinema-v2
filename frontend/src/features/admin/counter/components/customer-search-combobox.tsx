import { useState, useCallback } from "react";
import { SearchIcon, XIcon, UserIcon, TagIcon } from "lucide-react";

import { cn } from "@/lib/utils";
import { COMBOBOX_BLUR_CLOSE_DELAY_MS } from "@/config/default-constants";
import { useDebouncedValue } from "@/hooks/use-debounced-value";
import { useCustomerLookup } from "@/features/admin/users/hooks/use-admin-users";
import type { CustomerLookupItemDto } from "@/features/admin/users/types";
import { formatNumber } from "@/lib/formatters";

interface CustomerSearchComboboxProps {
  selected: CustomerLookupItemDto | null;
  onSelect: (customer: CustomerLookupItemDto | null) => void;
}

export const CustomerSearchCombobox = ({
  selected,
  onSelect,
}: CustomerSearchComboboxProps) => {
  const [query, setQuery] = useState("");
  const [open, setOpen] = useState(false);
  const debouncedQuery = useDebouncedValue(query);

  const { data: results = [], isFetching } = useCustomerLookup({
    q: debouncedQuery,
  });

  const handleSelect = useCallback(
    (customer: CustomerLookupItemDto) => {
      onSelect(customer);
      setQuery("");
      setOpen(false);
    },
    [onSelect],
  );

  const handleClear = useCallback(() => {
    onSelect(null);
    setQuery("");
  }, [onSelect]);

  if (selected) {
    return (
      <div className="flex flex-col gap-1">
        <span className="text-xs font-medium text-muted-foreground">
          Khách hàng
        </span>
        <div className="flex items-start justify-between gap-2 cut-sm border border-border bg-muted px-3 py-2">
          <div className="flex min-w-0 flex-1 flex-col gap-0.5">
            <span className="truncate text-sm font-medium text-foreground">
              {selected.fullName}
            </span>
            <span className="text-xs text-muted-foreground">
              {selected.phoneLast4 ? `****${selected.phoneLast4}` : "Chưa có SĐT"}
              {" · "}
              {selected.maskedEmail}
            </span>
            {selected.tierName && (
              <span className="flex items-center gap-1 text-xs text-accent-foreground">
                <TagIcon className="size-3" />
                {selected.tierName}
                {selected.discountPercent > 0 && (
                  <span className="text-success">
                    {" "}
                    (−{formatNumber(selected.discountPercent)}%)
                  </span>
                )}
              </span>
            )}
          </div>
          <button
            type="button"
            onClick={handleClear}
            className="mt-0.5 shrink-0 text-muted-foreground transition-colors hover:text-foreground"
            aria-label="Bỏ chọn khách hàng"
          >
            <XIcon className="size-4" />
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="relative flex flex-col gap-1">
      <span className="text-xs font-medium text-muted-foreground">
        Tìm khách hàng{" "}
        <span className="font-normal text-muted-foreground/70">(SĐT, email hoặc tên)</span>
      </span>

      <div className="relative">
        <SearchIcon className="absolute left-2.5 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
        <input
          type="text"
          value={query}
          onChange={(e) => {
            setQuery(e.target.value);
            setOpen(true);
          }}
          onFocus={() => setOpen(true)}
          onBlur={() => setTimeout(() => setOpen(false), COMBOBOX_BLUR_CLOSE_DELAY_MS)}
          placeholder="Nhập SĐT, email hoặc tên..."
          className={cn(
            "w-full cut-sm border border-border bg-input py-2 pl-8 pr-3 text-sm outline-none",
            "placeholder:text-muted-foreground/60",
            "focus:border-primary",
          )}
        />
        {isFetching && (
          <span className="absolute right-2.5 top-1/2 size-3.5 -translate-y-1/2 animate-spin rounded-full border-2 border-muted-foreground border-t-transparent" />
        )}
      </div>

      {open && debouncedQuery.trim().length > 0 && (
        <div className="absolute left-0 right-0 top-full z-50 mt-1 cut-sm border border-border bg-popover shadow-sm">
          {results.length === 0 && !isFetching && (
            <div className="flex items-center gap-2 px-3 py-4 text-sm text-muted-foreground">
              <UserIcon className="size-4" />
              Không tìm thấy khách hàng.
            </div>
          )}
          {results.map((customer) => (
            <button
              key={customer.userId}
              type="button"
              onMouseDown={(e) => {
                e.preventDefault();
                handleSelect(customer);
              }}
              className={cn(
                "flex w-full flex-col gap-0.5 px-3 py-2 text-left text-sm",
                "transition-colors hover:bg-muted focus:bg-muted",
              )}
            >
              <span className="font-medium text-foreground">
                {customer.fullName}
              </span>
              <span className="text-xs text-muted-foreground">
                {customer.phoneLast4
                  ? `****${customer.phoneLast4}`
                  : "Chưa có SĐT"}
                {" · "}
                {customer.maskedEmail}
                {customer.tierName && (
                  <span className="ml-1 text-accent-foreground">
                    · {customer.tierName}
                    {customer.discountPercent > 0 &&
                      ` (−${formatNumber(customer.discountPercent)}%)`}
                  </span>
                )}
              </span>
            </button>
          ))}
        </div>
      )}
    </div>
  );
}
