import type { ReactNode } from "react";
import {
  useTable,
  type ColumnDef,
  type ColumnFiltersState,
  type OnChangeFn,
  type PaginationState,
  type RowData,
  type RowSelectionState,
  type SortingState,
} from "@tanstack/react-table";

import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Skeleton } from "@/components/ui/skeleton";
import { cn } from "@/lib/utils";
import {
  features,
  type DataTableFeatures,
} from "@/components/common/data-table/data-table-features";
import { DataTablePagination } from "@/components/common/data-table/data-table-pagination";
import { DataTableToolbar } from "@/components/common/data-table/data-table-toolbar";

interface DataTableProps<TData extends RowData> {
  columns: ColumnDef<DataTableFeatures, TData>[];
  data: TData[];
  isLoading?: boolean;
  isRefreshing?: boolean;
  totalItems: number;
  pagination: PaginationState;
  onPaginationChange: OnChangeFn<PaginationState>;
  sorting?: SortingState;
  onSortingChange?: OnChangeFn<SortingState>;
  columnFilters?: ColumnFiltersState;
  onColumnFiltersChange?: OnChangeFn<ColumnFiltersState>;
  emptyMessage?: string;
  searchValue?: string;
  onSearchChange?: (value: string) => void;
  searchPlaceholder?: string;
  toolbarFilters?: ReactNode;
  rowSelection?: RowSelectionState;
  onRowSelectionChange?: OnChangeFn<RowSelectionState>;
  getRowId?: (row: TData) => string;
  bulkActions?: (selectedIds: string[]) => ReactNode;
}

export const DataTable = <TData extends RowData>({
  columns,
  data,
  isLoading,
  isRefreshing,
  totalItems,
  pagination,
  onPaginationChange,
  sorting,
  onSortingChange,
  columnFilters,
  onColumnFiltersChange,
  emptyMessage = "Không có dữ liệu.",
  searchValue,
  onSearchChange,
  searchPlaceholder,
  toolbarFilters,
  rowSelection,
  onRowSelectionChange,
  getRowId,
  bulkActions,
}: DataTableProps<TData>) => {
  const handlePaginationChange: OnChangeFn<PaginationState> = (updater) => {
    onRowSelectionChange?.({});
    onPaginationChange(updater);
  };

  const table = useTable({
    features,
    data,
    columns,
    manualPagination: true,
    manualSorting: Boolean(onSortingChange),
    manualFiltering: Boolean(onColumnFiltersChange),
    pageCount: Math.max(Math.ceil(totalItems / pagination.pageSize), 1),
    rowCount: totalItems,
    onPaginationChange: handlePaginationChange,
    onSortingChange,
    onColumnFiltersChange,
    ...(onRowSelectionChange ? { onRowSelectionChange } : {}),
    ...(getRowId ? { getRowId } : {}),
    state: {
      pagination,
      ...(sorting ? { sorting } : {}),
      ...(columnFilters ? { columnFilters } : {}),
      ...(rowSelection !== undefined ? { rowSelection } : {}),
    },
  });

  return (
    <div className="space-y-4">
      {(onSearchChange || bulkActions) && (
        <DataTableToolbar
          table={table}
          searchValue={searchValue ?? ""}
          onSearchChange={onSearchChange ?? (() => { })}
          searchPlaceholder={searchPlaceholder}
          filters={toolbarFilters}
          bulkActions={
            bulkActions && table.getSelectedRowModel().rows.length > 0
              ? bulkActions(table.getSelectedRowIds())
              : undefined
          }
        />
      )}

      <div className="overflow-hidden border">
        <Table>
          <TableHeader>
            {table.getHeaderGroups().map((headerGroup) => (
              <TableRow key={headerGroup.id}>
                {headerGroup.headers.map((header) => (
                  <TableHead key={header.id}>
                    {header.isPlaceholder ? null : (
                      <table.FlexRender header={header} />
                    )}
                  </TableHead>
                ))}
              </TableRow>
            ))}
          </TableHeader>
          <TableBody
            aria-busy={isRefreshing || undefined}
            className={cn(isRefreshing && "opacity-60 transition-opacity")}
          >
            {isLoading ? (
              Array.from({ length: pagination.pageSize }).map((_, i) => (
                <TableRow key={i}>
                  {columns.map((_, j) => (
                    <TableCell key={j}>
                      <Skeleton className="h-5 w-full" />
                    </TableCell>
                  ))}
                </TableRow>
              ))
            ) : table.getRowModel().rows?.length ? (
              table.getRowModel().rows.map((row) => (
                <TableRow
                  key={row.id}
                  data-state={row.getIsSelected() && "selected"}
                >
                  {row.getVisibleCells().map((cell) => (
                    <TableCell key={cell.id}>
                      <table.FlexRender cell={cell} />
                    </TableCell>
                  ))}
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell
                  colSpan={columns.length}
                  className="text-muted-foreground h-24 text-center"
                >
                  {emptyMessage}
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>

      <DataTablePagination table={table} totalItems={totalItems} />
    </div>
  );
};
