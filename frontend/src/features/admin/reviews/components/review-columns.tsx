import { createColumnHelper } from "@tanstack/react-table";
import { MoreHorizontal, Star, Trash2 } from "lucide-react";

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

import type { AdminReviewDto } from "../types";

interface ReviewColumnsOptions {
  onDelete: (review: AdminReviewDto) => void;
}

const columnHelper = createColumnHelper<DataTableFeatures, AdminReviewDto>();

export const createReviewColumns = ({ onDelete }: ReviewColumnsOptions) =>
  columnHelper.columns([
    columnHelper.accessor("reviewId", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="ID" />,
      cell: ({ row }) => (
        <span className="text-muted-foreground w-14 inline-block text-sm tabular-nums">
          #{row.original.reviewId}
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
    }),
    columnHelper.accessor("userName", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Người dùng" />,
      cell: ({ row }) => (
        <span className="text-sm text-foreground">{row.original.userName}</span>
      ),
    }),
    columnHelper.accessor("rating", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Đánh giá" />,
      cell: ({ row }) => (
        <div className="flex items-center gap-0.5">
          {Array.from({ length: 5 }).map((_, i) => (
            <Star
              key={i}
              className={
                i < row.original.rating
                  ? "size-3.5 fill-accent text-accent"
                  : "size-3.5 text-muted-foreground/30"
              }
            />
          ))}
        </div>
      ),
      enableSorting: false,
    }),
    columnHelper.accessor("comment", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Nội dung" />,
      cell: ({ row }) =>
        row.original.comment ? (
          <span className="text-sm text-muted-foreground line-clamp-2 max-w-xs">
            {row.original.comment}
          </span>
        ) : (
          <span className="text-muted-foreground text-sm">—</span>
        ),
      enableSorting: false,
    }),
    columnHelper.accessor("createdAt", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Ngày" />,
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
              <DropdownMenuItem variant="destructive" onClick={() => onDelete(row.original)}>
                <Trash2 /> Xóa đánh giá
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      ),
      enableHiding: false,
    }),
  ]);