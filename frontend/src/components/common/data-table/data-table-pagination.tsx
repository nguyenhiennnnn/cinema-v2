import type { ReactTable, RowData } from "@tanstack/react-table";
import {
  ChevronLeft,
  ChevronRight,
  ChevronsLeft,
  ChevronsRight,
} from "lucide-react";

import { Button } from "@/components/ui/button";
import { PAGE_SIZE_OPTIONS } from "@/config/default-constants";
import type { DataTableFeatures } from "@/components/common/data-table/data-table-features";
import { SelectField } from "../select-field";

interface DataTablePaginationProps<TData extends RowData> {
  table: ReactTable<DataTableFeatures, TData>;
  totalItems: number;
}

export const DataTablePagination = <TData extends RowData>({
  table,
  totalItems,
}: DataTablePaginationProps<TData>) => {
  const { pageIndex, pageSize } = table.state.pagination;

  return (
    <div className="flex items-center justify-between px-2">
      <div className="text-muted-foreground flex-1 text-sm">
        {table.getFilteredSelectedRowModel().rows.length > 0
          ? `${table.getFilteredSelectedRowModel().rows.length} dòng đã chọn.`
          : `${totalItems} kết quả.`}
      </div>
      <div className="flex items-center space-x-6 lg:space-x-8">
        <div className="flex items-center space-x-2">
          <p className="text-sm font-medium">Số dòng / trang</p>
          <SelectField
            value={pageSize}
            onChange={(value) =>
              table.setPagination({ pageIndex: 0, pageSize: Number(value) })
            }
            options={PAGE_SIZE_OPTIONS}
            getOptionValue={(size) => size}
            getOptionLabel={(size) => String(size)}
            className="h-8 w-17.5"
          />
        </div>
        <div className="flex w-25 items-center justify-center text-sm font-medium">
          Trang {pageIndex + 1} / {Math.max(table.getPageCount(), 1)}
        </div>
        <div className="flex items-center space-x-2">
          <Button
            variant="outline"
            size="icon"
            className="hidden size-8 lg:flex"
            onClick={() => table.setPageIndex(0)}
            disabled={!table.getCanPreviousPage()}
          >
            <span className="sr-only">Trang đầu</span>
            <ChevronsLeft />
          </Button>
          <Button
            variant="outline"
            size="icon"
            className="size-8"
            onClick={() => table.previousPage()}
            disabled={!table.getCanPreviousPage()}
          >
            <span className="sr-only">Trang trước</span>
            <ChevronLeft />
          </Button>
          <Button
            variant="outline"
            size="icon"
            className="size-8"
            onClick={() => table.nextPage()}
            disabled={!table.getCanNextPage()}
          >
            <span className="sr-only">Trang sau</span>
            <ChevronRight />
          </Button>
          <Button
            variant="outline"
            size="icon"
            className="hidden size-8 lg:flex"
            onClick={() => table.setPageIndex(table.getPageCount() - 1)}
            disabled={!table.getCanNextPage()}
          >
            <span className="sr-only">Trang cuối</span>
            <ChevronsRight />
          </Button>
        </div>
      </div>
    </div>
  );
};
