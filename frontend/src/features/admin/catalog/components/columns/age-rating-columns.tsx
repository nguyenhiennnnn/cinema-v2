import { createColumnHelper } from "@tanstack/react-table";
import { MoreHorizontal, Pencil, Trash2 } from "lucide-react";

import { AgeRatingBadge } from "@/components/common/status-badge";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { DataTableColumnHeader } from "@/components/common/data-table/data-table-column-header";
import type { DataTableFeatures } from "@/components/common/data-table/data-table-features";

import type { AgeRatingDto } from "../../types";

interface AgeRatingColumnsOptions {
  onEdit: (ageRating: AgeRatingDto) => void;
  onDelete: (ageRating: AgeRatingDto) => void;
}

const columnHelper = createColumnHelper<DataTableFeatures, AgeRatingDto>();

export const createAgeRatingColumns = ({
  onEdit,
  onDelete,
}: AgeRatingColumnsOptions) =>
  columnHelper.columns([
    columnHelper.accessor("ageRatingId", {
      header: ({ column }) => (
        <DataTableColumnHeader column={column} title="ID" />
      ),
      cell: ({ row }) => (
        <span className="text-muted-foreground w-14 inline-block text-sm tabular-nums">
          #{row.original.ageRatingId}
        </span>
      ),
    }),
    columnHelper.accessor("code", {
      filterFn: "includesString",
      header: ({ column }) => (
        <DataTableColumnHeader column={column} title="Mã phân loại" />
      ),
      cell: ({ row }) => <AgeRatingBadge code={row.original.code} />,
    }),
    columnHelper.accessor("description", {
      header: ({ column }) => (
        <DataTableColumnHeader column={column} title="Mô tả" />
      ),
      cell: ({ row }) => (
        <span className="text-muted-foreground text-sm line-clamp-1">
          {row.original.description || "—"}
        </span>
      ),
      enableSorting: false,
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