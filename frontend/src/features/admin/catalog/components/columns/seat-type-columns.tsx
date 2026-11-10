import { createColumnHelper } from "@tanstack/react-table";
import { MoreHorizontal, Pencil, Trash2 } from "lucide-react";

import { SeatTypeBadge } from "@/components/common/status-badge";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { DataTableColumnHeader } from "@/components/common/data-table/data-table-column-header";
import type { DataTableFeatures } from "@/components/common/data-table/data-table-features";

import type { SeatTypeDto } from "@/features/admin/catalog/types";

interface SeatTypeColumnsOptions {
  onEdit: (seatType: SeatTypeDto) => void;
  onDelete: (seatType: SeatTypeDto) => void;
}

const columnHelper = createColumnHelper<DataTableFeatures, SeatTypeDto>();

export const createSeatTypeColumns = ({
  onEdit,
  onDelete,
}: SeatTypeColumnsOptions) =>
  columnHelper.columns([
    columnHelper.accessor("seatTypeId", {
      header: ({ column }) => (
        <DataTableColumnHeader column={column} title="ID" />
      ),
      cell: ({ row }) => (
        <span className="text-muted-foreground w-14 inline-block text-sm tabular-nums">
          #{row.original.seatTypeId}
        </span>
      ),
    }),
    columnHelper.accessor("name", {
      filterFn: "includesString",
      header: ({ column }) => (
        <DataTableColumnHeader column={column} title="Loại ghế" />
      ),
      cell: ({ row }) => <SeatTypeBadge name={row.original.name} />,
    }),
    columnHelper.accessor("priceMultiplier", {
      header: ({ column }) => (
        <DataTableColumnHeader column={column} title="Hệ số giá" />
      ),
      cell: ({ row }) => (
        <span className="font-semibold tabular-nums text-primary">
          x{row.original.priceMultiplier}
        </span>
      ),
    }),
    columnHelper.display({
      id: "actions",
      header: () => <div className="text-right">Hành động</div>,
      cell: ({ row }) => (
        <div className="flex justify-end">
          <DropdownMenu>
            <DropdownMenuTrigger>
              <Button variant="ghost" size="icon" className="size-8">
                <span className="sr-only">Mở menu</span>
                <MoreHorizontal className="size-4" />
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end">
              <DropdownMenuItem onClick={() => onEdit(row.original)}>
                <Pencil /> Chỉnh sửa
              </DropdownMenuItem>
              <DropdownMenuItem
                variant="destructive"
                onClick={() => onDelete(row.original)}
              >
                <Trash2 /> Xóa
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      ),
      enableHiding: false,
    }),
  ]);