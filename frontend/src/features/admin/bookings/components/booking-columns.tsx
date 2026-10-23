import { createColumnHelper } from "@tanstack/react-table";
import { Eye, MoreHorizontal } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { DataTableColumnHeader } from "@/components/common/data-table/data-table-column-header";
import type { DataTableFeatures } from "@/components/common/data-table/data-table-features";
import { BookingChannelBadge, BookingStatusBadge } from "@/components/common/status-badge";
import { formatCurrency, formatDate } from "@/lib/formatters";

import type { AdminBookingListItemDto } from "../types";

interface BookingColumnsOptions {
  onViewDetail: (booking: AdminBookingListItemDto) => void;
}

const columnHelper = createColumnHelper<DataTableFeatures, AdminBookingListItemDto>();

export const createBookingColumns = ({ onViewDetail }: BookingColumnsOptions) =>
  columnHelper.columns([
    columnHelper.accessor("bookingCode", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Mã đơn" />,
      cell: ({ row }) => (
        <span className="tabular-nums font-medium tracking-wider text-foreground">
          {row.original.bookingCode}
        </span>
      ),
    }),
    columnHelper.accessor("customerName", {
      filterFn: "includesString",
      header: ({ column }) => <DataTableColumnHeader column={column} title="Khách hàng" />,
      cell: ({ row }) => (
        <div className="flex flex-col">
          <span className="font-medium text-foreground">{row.original.customerName}</span>
          {row.original.guestPhone && (
            <span className="text-xs text-muted-foreground">{row.original.guestPhone}</span>
          )}
        </div>
      ),
    }),
    columnHelper.accessor("channel", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Kênh" />,
      cell: ({ row }) => <BookingChannelBadge channel={row.original.channel} />,
      enableSorting: false,
    }),
    columnHelper.accessor("finalAmount", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Số tiền" />,
      cell: ({ row }) => (
        <span className="font-semibold tabular-nums text-primary">
          {formatCurrency(row.original.finalAmount)}
        </span>
      ),
    }),
    columnHelper.accessor("status", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Trạng thái" />,
      cell: ({ row }) => <BookingStatusBadge status={row.original.status} />,
      enableSorting: false,
    }),
    columnHelper.accessor("createdAt", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Ngày đặt" />,
      cell: ({ row }) => (
        <span className="text-sm tabular-nums text-muted-foreground">
          {formatDate(row.original.createdAt)}
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
              <DropdownMenuItem onClick={() => onViewDetail(row.original)}>
                <Eye /> Xem chi tiết
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      ),
      enableHiding: false,
    }),
  ]);