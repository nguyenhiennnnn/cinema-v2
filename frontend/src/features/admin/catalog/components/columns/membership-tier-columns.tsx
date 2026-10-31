import { createColumnHelper } from "@tanstack/react-table";
import { MoreHorizontal, Pencil, Trash2 } from "lucide-react";
import { formatNumber } from "@/lib/formatters";
import { MembershipTierBadge } from "@/components/common/status-badge";
import { Button } from "@/components/ui/button";
import {
DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { DataTableColumnHeader } from "@/components/common/data-table/data-table-column-header";
import type { DataTableFeatures } from "@/components/common/data-table/data-table-features";
import type { MembershipTierDto } from "../../types";
interface MembershipTierColumnsOptions {
  onEdit: (tier: MembershipTierDto) => void;
  onDelete: (tier: MembershipTierDto) => void;
}

const columnHelper = createColumnHelper<DataTableFeatures, MembershipTierDto>();

export const createMembershipTierColumns = ({
  onEdit,
  onDelete,
}: MembershipTierColumnsOptions) =>
  columnHelper.columns([
    columnHelper.accessor("tierId", {
      header: ({ column }) => (
        <DataTableColumnHeader column={column} title="ID" />
      ),
      cell: ({ row }) => (
        <span className="text-muted-foreground w-14 inline-block text-sm tabular-nums">
          #{row.original.tierId}
        </span>
      ),
    }),
    columnHelper.accessor("tierName", {
      filterFn: "includesString",
      header: ({ column }) => (
        <DataTableColumnHeader column={column} title="Hạng thành viên" />
      ),
      cell: ({ row }) => <MembershipTierBadge tierName={row.original.tierName} />,
    }),
    columnHelper.accessor("minPoints", {
      header: ({ column }) => (
        <DataTableColumnHeader column={column} title="Điểm tối thiểu" />
      ),
      cell: ({ row }) => (
        <span className="font-semibold tabular-nums">
          {formatNumber(row.original.minPoints)}
        </span>
      ),
    }),
    columnHelper.accessor("discountPercent", {
      header: ({ column }) => (
        <DataTableColumnHeader column={column} title="Giảm giá" />
      ),
      cell: ({ row }) => (
        <span className="font-semibold tabular-nums text-primary">
          {row.original.discountPercent}%
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
