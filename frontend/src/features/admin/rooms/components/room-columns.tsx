import { createColumnHelper } from "@tanstack/react-table";
import { Eye, MoreHorizontal, Pencil, Power, PowerOff } from "lucide-react";

import { RoomStatusBadge } from "@/components/common/status-badge";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { DataTableColumnHeader } from "@/components/common/data-table/data-table-column-header";
import type { DataTableFeatures } from "@/components/common/data-table/data-table-features";

import type { AdminRoomDto } from "../types";

interface RoomColumnsOptions {
  onViewDetail: (room: AdminRoomDto) => void;
  onEdit: (room: AdminRoomDto) => void;
  onToggle: (room: AdminRoomDto) => void;

}

const columnHelper = createColumnHelper<DataTableFeatures, AdminRoomDto>();

export const createRoomColumns = ({ onViewDetail, onEdit, onToggle }: RoomColumnsOptions) =>
  columnHelper.columns([
    columnHelper.accessor("roomId", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="ID" />,
      cell: ({ row }) => (
        <span className="text-muted-foreground w-12 inline-block text-sm tabular-nums">
          #{row.original.roomId}
        </span>
      ),
    }),
    columnHelper.accessor("roomName", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Phòng chiếu" />,
      cell: ({ row }) => (
        <span className="font-medium text-foreground">{row.original.roomName}</span>
      ),
    }),
    columnHelper.accessor("screenTypeName", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Loại màn hình" />,
      cell: ({ row }) => (
        <span className="text-sm text-muted-foreground">
          {row.original.screenTypeName ?? "—"}
        </span>
      ),
      enableSorting: false,
    }),
    columnHelper.accessor("totalSeats", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Tổng số ghế" />,
      cell: ({ row }) => (
        <span className="text-sm tabular-nums">{row.original.totalSeats}</span>
      ),
    }),
    columnHelper.accessor("isActive", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Trạng thái" />,
      cell: ({ row }) => <RoomStatusBadge isActive={row.original.isActive} />,
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
              <DropdownMenuItem onClick={() => onViewDetail(row.original)}>
                <Eye /> Xem chi tiết
              </DropdownMenuItem>
              <DropdownMenuItem onClick={() => onEdit(row.original)}>
                <Pencil /> Chỉnh sửa
              </DropdownMenuItem>
              {row.original.isActive ? (
                <DropdownMenuItem variant="destructive" onClick={() => onToggle(row.original)}>
                  <PowerOff /> Tạm ngưng
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