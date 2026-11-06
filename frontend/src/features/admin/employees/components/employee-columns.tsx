import { createColumnHelper } from "@tanstack/react-table";
import { MoreHorizontal, Pencil, Power, PowerOff } from "lucide-react";

import { EmployeeStatusBadge, PositionBadge } from "@/components/common/status-badge";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { DataTableColumnHeader } from "@/components/common/data-table/data-table-column-header";
import type { DataTableFeatures } from "@/components/common/data-table/data-table-features";
import { formatDate } from "@/lib/formatters";

import type { AdminEmployeeDto } from "../types";

interface EmployeeColumnsOptions {
  onEdit: (employee: AdminEmployeeDto) => void;
  onToggle: (employee: AdminEmployeeDto) => void;
  /** Id user đang đăng nhập — không cho tự vô hiệu hóa hồ sơ của chính mình. */
  currentUserId?: number;
}

const columnHelper = createColumnHelper<DataTableFeatures, AdminEmployeeDto>();

export const createEmployeeColumns = ({ onEdit, onToggle, currentUserId }: EmployeeColumnsOptions) =>
  columnHelper.columns([
    columnHelper.accessor("employeeId", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="ID" />,
      cell: ({ row }) => (
        <span className="text-muted-foreground w-14 inline-block text-sm tabular-nums">
          #{row.original.employeeId}
        </span>
      ),
    }),
    columnHelper.accessor("fullName", {
      filterFn: "includesString",
      header: ({ column }) => <DataTableColumnHeader column={column} title="Nhân viên" />,
      cell: ({ row }) => (
        <div className="flex flex-col">
          <span className="font-medium text-foreground">{row.original.fullName}</span>
          <span className="text-xs text-muted-foreground">{row.original.email}</span>
        </div>
      ),
    }),
    columnHelper.accessor("positionName", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Vị trí" />,
      cell: ({ row }) => <PositionBadge name={row.original.positionName} />,
      enableSorting: false,
    }),
    columnHelper.accessor("hireDate", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Ngày vào làm" />,
      cell: ({ row }) => (
        <span className="text-sm tabular-nums">
          {formatDate(row.original.hireDate)}
        </span>
      ),
    }),
    columnHelper.accessor("isActive", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Trạng thái" />,
      cell: ({ row }) => <EmployeeStatusBadge isActive={row.original.isActive} />,
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
                  disabled={row.original.userId === currentUserId}
                  onClick={() => onToggle(row.original)}
                >
                  <PowerOff /> Cho nghỉ việc
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