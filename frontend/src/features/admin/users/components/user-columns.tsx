import { createColumnHelper } from "@tanstack/react-table";
import { MoreHorizontal, Power, PowerOff, ShieldCheck } from "lucide-react";

import { RoleBadge, UserStatusBadge } from "@/components/common/status-badge";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { DataTableColumnHeader } from "@/components/common/data-table/data-table-column-header";
import type { DataTableFeatures } from "@/components/common/data-table/data-table-features";

import type { AdminUserListItemDto } from "../types";

interface UserColumnsOptions {
  onChangeRole: (user: AdminUserListItemDto) => void;
  onToggle: (user: AdminUserListItemDto) => void;
  /** Id của user đang đăng nhập — không cho tự đổi role / tự khóa chính mình (backend cũng chặn). */
  currentUserId?: number;
}

const columnHelper = createColumnHelper<DataTableFeatures, AdminUserListItemDto>();

export const createUserColumns = ({ onChangeRole, onToggle, currentUserId }: UserColumnsOptions) =>
  columnHelper.columns([
    columnHelper.accessor("userId", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="ID" />,
      cell: ({ row }) => (
        <span className="text-muted-foreground w-14 inline-block text-sm tabular-nums">
          #{row.original.userId}
        </span>
      ),
    }),
    columnHelper.accessor("fullName", {
      filterFn: "includesString",
      header: ({ column }) => <DataTableColumnHeader column={column} title="Người dùng" />,
      cell: ({ row }) => (
        <div className="flex flex-col">
          <span className="font-medium text-foreground">{row.original.fullName}</span>
          <span className="text-xs text-muted-foreground">{row.original.email}</span>
        </div>
      ),
    }),
    columnHelper.accessor("roleName", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Vai trò" />,
      cell: ({ row }) => <RoleBadge role={row.original.roleName} />,
      enableSorting: false,
    }),
    columnHelper.accessor("isActive", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Trạng thái" />,
      cell: ({ row }) => <UserStatusBadge isActive={row.original.isActive} />,
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
              <DropdownMenuItem
                disabled={row.original.userId === currentUserId}
                onClick={() => onChangeRole(row.original)}
              >
                <ShieldCheck /> Đổi vai trò
              </DropdownMenuItem>
              {row.original.isActive ? (
                <DropdownMenuItem
                  variant="destructive"
                  disabled={row.original.userId === currentUserId}
                  onClick={() => onToggle(row.original)}
                >
                  <PowerOff /> Khóa tài khoản
                </DropdownMenuItem>
              ) : (
                <DropdownMenuItem
                  disabled={row.original.userId === currentUserId}
                  onClick={() => onToggle(row.original)}
                >
                  <Power /> Mở khóa
                </DropdownMenuItem>
              )}
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      ),
      enableHiding: false,
    }),
  ]);