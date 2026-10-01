import { createColumnHelper } from "@tanstack/react-table";
import { ImageOff, MoreHorizontal, Pencil, Power, PowerOff } from "lucide-react";

import { ComboStatusBadge } from "@/components/common/status-badge";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { DataTableColumnHeader } from "@/components/common/data-table/data-table-column-header";
import type { DataTableFeatures } from "@/components/common/data-table/data-table-features";
import { formatCurrency } from "@/lib/formatters";

import type { AdminComboDto } from "../types";

interface ComboColumnsOptions {
  onEdit: (combo: AdminComboDto) => void;
  onToggle: (combo: AdminComboDto) => void;
}

const columnHelper = createColumnHelper<DataTableFeatures, AdminComboDto>();

export const createComboColumns = ({ onEdit, onToggle }: ComboColumnsOptions) =>
  columnHelper.columns([
    columnHelper.accessor("comboId", {
      header: ({ column }) => (
        <DataTableColumnHeader column={column} title="ID" />
      ),
      cell: ({ row }) => (
        <span className="text-muted-foreground w-14 inline-block text-sm tabular-nums">
          #{row.original.comboId}
        </span>
      ),
    }),
    columnHelper.accessor("name", {
      filterFn: "includesString",
      header: ({ column }) => (
        <DataTableColumnHeader column={column} title="Combo" />
      ),
      cell: ({ row }) => (
        <div className="flex items-center gap-2.5">
          <div className="flex size-10 shrink-0 items-center justify-center overflow-hidden rounded-md border border-border bg-muted">
            {row.original.imageUrl ? (
              <img
                src={row.original.imageUrl}
                alt={row.original.name}
                className="h-full w-full object-cover"
              />
            ) : (
              <ImageOff className="size-4 text-muted-foreground" />
            )}
          </div>
          <div className="flex flex-col">
            <span className="font-medium text-foreground">
              {row.original.name}
            </span>
            {row.original.description && (
              <span className="text-xs text-muted-foreground line-clamp-1">
                {row.original.description}
              </span>
            )}
          </div>
        </div>
      ),
    }),
    columnHelper.accessor("price", {
      header: ({ column }) => (
        <DataTableColumnHeader column={column} title="Giá" />
      ),
      cell: ({ row }) => (
        <span className="font-semibold tabular-nums text-primary">
          {formatCurrency(row.original.price)}
        </span>
      ),
    }),
    columnHelper.accessor("isActive", {
      header: ({ column }) => (
        <DataTableColumnHeader column={column} title="Trạng thái" />
      ),
      cell: ({ row }) => <ComboStatusBadge isActive={row.original.isActive} />,
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
              {row.original.isActive ? (
                <DropdownMenuItem
                  variant="destructive"
                  onClick={() => onToggle(row.original)}
                >
                  <PowerOff /> Ngừng bán
                </DropdownMenuItem>
              ) : (
                <DropdownMenuItem onClick={() => onToggle(row.original)}>
                  <Power /> Kích hoạt lại
                </DropdownMenuItem>
              )}
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      ),
      enableHiding: false,
    }),
  ]);