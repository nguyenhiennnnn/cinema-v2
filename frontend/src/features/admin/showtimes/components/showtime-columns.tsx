import { createColumnHelper } from "@tanstack/react-table";
import { MoreHorizontal, Pencil, XCircle } from "lucide-react";

import { ShowtimeStatusBadge } from "@/components/common/status-badge";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { DataTableColumnHeader } from "@/components/common/data-table/data-table-column-header";
import type { DataTableFeatures } from "@/components/common/data-table/data-table-features";
import { formatCurrency, formatDate, formatTime } from "@/lib/formatters";

import type { AdminShowtimeDto } from "../types";

interface ShowtimeColumnsOptions {
  onEdit?: (showtime: AdminShowtimeDto) => void;
  onDelete?: (showtime: AdminShowtimeDto) => void;
  canManage?: boolean;
}

const columnHelper = createColumnHelper<DataTableFeatures, AdminShowtimeDto>();

export const createShowtimeColumns = ({
  onEdit,
  onDelete,
  canManage = true,
}: ShowtimeColumnsOptions) =>
  columnHelper.columns([
    columnHelper.accessor("showtimeId", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="ID" />,
      cell: ({ row }) => (
        <span className="text-muted-foreground w-12 inline-block text-sm tabular-nums">
          #{row.original.showtimeId}
        </span>
      ),
    }),
    columnHelper.accessor("movieTitle", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Phim" />,
      cell: ({ row }) => (
        <span className="font-medium text-foreground line-clamp-1">
          {row.original.movieTitle}
        </span>
      ),
      enableSorting: false,
    }),
    columnHelper.accessor("roomName", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Phòng chiếu" />,
      cell: ({ row }) => (
        <span className="text-sm text-muted-foreground">{row.original.roomName}</span>
      ),
      enableSorting: false,
    }),
    columnHelper.accessor("startTime", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Suất chiếu" />,
      cell: ({ row }) => (
        <div className="flex flex-col text-sm">
          <span className="font-medium tabular-nums">
            {formatTime(row.original.startTime)} - {formatTime(row.original.endTime)}
          </span>
          <span className="text-xs text-muted-foreground tabular-nums">
            {formatDate(row.original.startTime)}
          </span>
        </div>
      ),
    }),
    columnHelper.accessor("basePrice", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Giá vé" />,
      cell: ({ row }) => (
        <span className="text-sm font-medium tabular-nums">
          {formatCurrency(row.original.basePrice)}
        </span>
      ),
    }),
    columnHelper.accessor("status", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Trạng thái" />,
      cell: ({ row }) => <ShowtimeStatusBadge status={row.original.status} />,
      enableSorting: false,
    }),
    ...(canManage
      ? [
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
                  <DropdownMenuItem onClick={() => onEdit?.(row.original)}>
                    <Pencil /> Chỉnh sửa
                  </DropdownMenuItem>
                  <DropdownMenuItem
                    variant="destructive"
                    onClick={() => onDelete?.(row.original)}
                  >
                    <XCircle /> Hủy suất chiếu
                  </DropdownMenuItem>
                </DropdownMenuContent>
              </DropdownMenu>
            </div>
          ),
          enableHiding: false,
        }),
      ]
      : []),
  ]);