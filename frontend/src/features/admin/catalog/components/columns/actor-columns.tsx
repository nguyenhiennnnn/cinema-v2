import { createColumnHelper } from "@tanstack/react-table";
import { MoreHorizontal, Pencil, Trash2, User } from "lucide-react";

import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { DataTableColumnHeader } from "@/components/common/data-table/data-table-column-header";
import type { DataTableFeatures } from "@/components/common/data-table/data-table-features";

import type { ActorDto } from "../../types";

interface ActorColumnsOptions {
  onEdit: (actor: ActorDto) => void;
  onDelete: (actor: ActorDto) => void;
}

const columnHelper = createColumnHelper<DataTableFeatures, ActorDto>();

export const createActorColumns = ({ onEdit, onDelete }: ActorColumnsOptions) =>
  columnHelper.columns([
    columnHelper.accessor("actorId", {
      header: ({ column }) => (
        <DataTableColumnHeader column={column} title="ID" />
      ),
      cell: ({ row }) => (
        <span className="text-muted-foreground w-14 inline-block text-sm tabular-nums">
          #{row.original.actorId}
        </span>
      ),
    }),
    columnHelper.accessor("fullName", {
      filterFn: "includesString",
      header: ({ column }) => (
        <DataTableColumnHeader column={column} title="Diễn viên" />
      ),
      cell: ({ row }) => (
        <div className="flex items-center gap-2.5">
          <Avatar className="size-8">
            <AvatarImage
              src={row.original.avatarUrl ?? undefined}
              alt={row.original.fullName}
            />
            <AvatarFallback>
              <User className="size-4" />
            </AvatarFallback>
          </Avatar>
          <span className="font-medium text-foreground">
            {row.original.fullName}
          </span>
        </div>
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