import type { ColumnDef } from "@tanstack/react-table";
import { Checkbox } from "@/components/ui/checkbox";
import type { DataTableFeatures } from "./data-table-features";

const SELECTION_COL_ID = "select";

export function createSelectionColumn<TData>(): ColumnDef<DataTableFeatures, TData> {
  return {
    id: SELECTION_COL_ID,
    header: ({ table }) => {
      const isAllSelected = table.getIsAllPageRowsSelected();
      const isSomeSelected = table.getIsSomePageRowsSelected();
      return (
        <Checkbox
          checked={isAllSelected || (isSomeSelected ? ("indeterminate" as unknown as boolean) : false)}
          indeterminate={isSomeSelected && !isAllSelected}
          onCheckedChange={(checked) =>
            table.toggleAllPageRowsSelected(!!checked)
          }
          aria-label="Chọn tất cả trang này"
        />
      );
    },
    cell: ({ row }) => (
      <Checkbox
        checked={row.getIsSelected()}
        onCheckedChange={(checked) => row.toggleSelected(!!checked)}
        aria-label="Chọn dòng này"
      />
    ),
    enableSorting: false,
    enableHiding: false,
  };
}
