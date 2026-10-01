import type { ReactTable, RowData } from "@tanstack/react-table";
import { Settings2 } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuCheckboxItem,
  DropdownMenuContent,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import type { DataTableFeatures } from "@/components/common/data-table/data-table-features";

export const DataTableViewOptions = <TData extends RowData>({
  table,
}: {
  table: ReactTable<DataTableFeatures, TData>;
}) => {
  return (
    <DropdownMenu>
      <DropdownMenuTrigger>
        <Button
          variant="outline"
          size="sm"
          className="ml-auto hidden h-8 lg:flex"
        >
          <Settings2 /> Hiển thị
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" className="w-37.5">
        <DropdownMenuLabel>Cột hiển thị</DropdownMenuLabel>
        <DropdownMenuSeparator />
        {table
          .getAllColumns()
          .filter(
            (column) =>
              typeof column.accessorFn !== "undefined" && column.getCanHide(),
          )
          .map((column) => (
            <DropdownMenuCheckboxItem
              key={column.id}
              className="capitalize"
              checked={column.getIsVisible()}
              onCheckedChange={(value) => column.toggleVisibility(!!value)}
            >
              {column.id}
            </DropdownMenuCheckboxItem>
          ))}
      </DropdownMenuContent>
    </DropdownMenu>
  );
};
