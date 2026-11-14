import { createColumnHelper } from "@tanstack/react-table";
import { MoreHorizontal, Pencil, Trash2 } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { DataTableColumnHeader } from "@/components/common/data-table/data-table-column-header";
import type { DataTableFeatures } from "@/components/common/data-table/data-table-features";
import { MovieStatusBadge } from "@/components/common/status-badge";
import { formatDate } from "@/lib/formatters";

import type { AdminMovieListItemDto } from "@/features/admin/movies/types";

interface MovieColumnsOptions {
  onEdit?: (movie: AdminMovieListItemDto) => void;
  onDelete?: (movie: AdminMovieListItemDto) => void;
  canManage?: boolean;
}

const columnHelper = createColumnHelper<DataTableFeatures, AdminMovieListItemDto>();

export const createMovieColumns = ({ onEdit, onDelete, canManage = true }: MovieColumnsOptions) =>
  columnHelper.columns([
    columnHelper.accessor("movieId", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="ID" />,
      cell: ({ row }) => (
        <span className="text-muted-foreground w-12 inline-block text-sm tabular-nums">
          #{row.original.movieId}
        </span>
      ),
    }),
    columnHelper.accessor("title", {
      filterFn: "includesString",
      header: ({ column }) => <DataTableColumnHeader column={column} title="Tên phim" />,
      cell: ({ row }) => (
        <div className="flex items-center gap-3">
          <img
            src={row.original.posterUrl || "/placeholders/movie-poster.png"}
            alt={row.original.title}
            className="h-14 w-10 shrink-0 border border-border object-cover"
          />
          <span className="font-medium text-foreground line-clamp-2">
            {row.original.title}
          </span>
        </div>
      ),
    }),
    columnHelper.accessor("directorName", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Đạo diễn" />,
      cell: ({ row }) => (
        <span className="text-sm text-muted-foreground">
          {row.original.directorName ?? "—"}
        </span>
      ),
      enableSorting: false,
    }),
    columnHelper.accessor("languageName", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Ngôn ngữ" />,
      cell: ({ row }) => (
        <span className="text-sm text-muted-foreground">
          {row.original.languageName ?? "—"}
        </span>
      ),
      enableSorting: false,
    }),
    columnHelper.accessor("releaseDate", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Ngày khởi chiếu" />,
      cell: ({ row }) => (
        <span className="text-sm tabular-nums">
          {row.original.releaseDate ? formatDate(row.original.releaseDate) : "—"}
        </span>
      ),
    }),
    columnHelper.accessor("status", {
      header: ({ column }) => <DataTableColumnHeader column={column} title="Trạng thái" />,
      cell: ({ row }) => <MovieStatusBadge status={row.original.status} />,
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
                    <Trash2 /> Xóa phim
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