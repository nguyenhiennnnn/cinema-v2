import { createColumnHelper } from "@tanstack/react-table";
import { MoreHorizontal, Pencil, Trash2 } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { DataTableColumnHeader } from "@/components/common/data-table/data-table-column-header";
import type { DataTableFeatures } from "@/components/common/data-table/data-table-features";

import type { PositionDto } from "../../types";

interface PositionColumnsOptions {
  onEdit: (position: PositionDto) => void;
  onDelete: (position: PositionDto) => void;
}

const columnHelper = createColumnHelper<DataTableFeatures, PositionDto>();

export const createPositionColumns = ({
  onEdit,
  onDelete,
}: PositionColumnsOptions) =>
  columnHelper.columns([
    columnHelper.accessor("positionId", {
      header: ({ column }) => (
        <DataTableColumnHeader column={column} title="ID" />
      ),
      cell: ({ row }) => (
        <span className="text-muted-foreground w-14 inline-block text-sm tabular-nums">
          #{row.original.positionId}
        </span>
      ),
    }),
    columnHelper.accessor("name", {
      filterFn: "includesString",
      header: ({ column }) => (
        <DataTableColumnHeader column={column} title="Chức vụ" />
      ),
      cell: ({ row }) => (
        <span className="font-medium text-foreground">
          {row.original.name}
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