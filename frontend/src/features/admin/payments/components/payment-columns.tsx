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
import { formatCurrency, formatDate } from "@/lib/formatters";

import type { AdminPaymentListItemDto } from "../types";
import { PaymentStatusBadge } from "@/components/common/status-badge";
import type { PaymentStatus } from "@/types/enum";

interface PaymentColumnsOptions {
  onViewDetail: (payment: AdminPaymentListItemDto) => void;
}

const columnHelper = createColumnHelper<DataTableFeatures, AdminPaymentListItemDto>();

export const createPaymentColumns = ({ onViewDetail }: PaymentColumnsOptions) =>
  columnHelper.columns([
    columnHelper.accessor("paymentId", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="ID" />,
      cell: ({ row }) => (
        <span className="text-muted-foreground w-14 inline-block text-sm tabular-nums">
          #{row.original.paymentId}
        </span>
      ),
    }),
    columnHelper.accessor("bookingCode", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Đơn đặt vé" />,
      cell: ({ row }) => (
        <span className="tabular-nums font-medium tracking-wider text-foreground">
          {row.original.bookingCode}
        </span>
      ),
    }),
    columnHelper.accessor("paymentMethod", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Phương thức" />,
      cell: ({ row }) => (
        <span className="text-sm text-foreground">{row.original.paymentMethod}</span>
      ),
    }),
    columnHelper.accessor("amount", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Số tiền" />,
      cell: ({ row }) => (
        <span className="font-semibold tabular-nums text-primary">
          {formatCurrency(row.original.amount)}
        </span>
      ),
    }),
    columnHelper.accessor("status", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Trạng thái" />,
      cell: ({ row }) => <PaymentStatusBadge status={row.original.status as PaymentStatus} />,
      enableSorting: false,
    }),
    columnHelper.accessor("paidAt", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Ngày thanh toán" />,
      cell: ({ row }) => (
        <span className="text-sm tabular-nums text-muted-foreground">
          {row.original.paidAt ? formatDate(row.original.paidAt) : "—"}
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