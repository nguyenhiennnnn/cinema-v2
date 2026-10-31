import type { ReactNode } from "react";
import type { ReactTable, RowData } from "@tanstack/react-table";
import { Search } from "lucide-react";

import { Input } from "@/components/ui/input";
import { DataTableViewOptions } from "@/components/common/data-table/data-table-view-options";
import type { DataTableFeatures } from "@/components/common/data-table/data-table-features";

interface DataTableToolbarProps<TData extends RowData> {
  table: ReactTable<DataTableFeatures, TData>;
  searchValue: string;
  onSearchChange: (value: string) => void;
  searchPlaceholder?: string;
  filters?: ReactNode;
  bulkActions?: ReactNode;
}

export const DataTableToolbar = <TData extends RowData>({
  table,
  searchValue,
  onSearchChange,
  searchPlaceholder = "Tìm kiếm...",
  filters,
  bulkActions,
}: DataTableToolbarProps<TData>) => {
  return (
    <div className="flex items-center justify-between gap-2">
      <div className="flex flex-1 items-center gap-2">
        <div className="relative max-w-sm flex-1">
          <Search className="text-muted-foreground pointer-events-none absolute top-1/2 left-2.5 size-4 -translate-y-1/2" />
          <Input
            placeholder={searchPlaceholder}
            value={searchValue}
            onChange={(e) => onSearchChange(e.target.value)}
            className="pl-8"
          />
        </div>
        {filters}
      </div>
      <div className="flex items-center gap-2">
        {bulkActions}
        <DataTableViewOptions table={table} />
      </div>
    </div>
  );
};
