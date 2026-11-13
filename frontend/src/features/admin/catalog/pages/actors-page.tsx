import { useState } from "react";
import { Plus } from "lucide-react";
import type { RowSelectionState } from "@tanstack/react-table";

import { PageHeader } from "@/components/common/page-header";
import { Button } from "@/components/ui/button";
import { DataTable } from "@/components/common/data-table/data-table";
import { BulkDeleteButton } from "@/components/common/data-table/bulk-delete-button";
import { createSelectionColumn } from "@/components/common/data-table/data-table-selection-column";
import { toastBulkResult } from "@/lib/bulk";
import { useDialogClose, useDialogOpen } from "@/stores/dialog-store";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";

import { useTablePagination } from "@/hooks/use-table-pagination";

import { useAdminPersonParams } from "@/hooks/use-query-params";
import {
  useAdminActors,
  useAdminDeleteActor,
  useAdminBulkDeleteActors,
} from "../hooks/use-admin-actor";
import type { ActorDto } from "../types";
import { createActorColumns } from "../components/columns/actor-columns";
import { ActorCreateForm } from "../components/forms/actors/actor-create-form";
import { ActorUpdateForm } from "../components/forms/actors/actor-update-form";
import { DEFAULT_PAGE, IRREVERSIBLE_ACTION_MESSAGE } from "@/config/default-constants";
import { useDebouncedValue } from "@/hooks/use-debounced-value";

const ActorsPage = () => {
  const [params, setParams] = useAdminPersonParams();
  const { pagination, onPaginationChange } = useTablePagination(params, setParams);

  const actors = useAdminActors({
    page: params.page,
    pageSize: params.pageSize,
    search: useDebouncedValue(params.search),
  });
  const deleteActor = useAdminDeleteActor();
  const bulkDeleteActors = useAdminBulkDeleteActors();
  const [rowSelection, setRowSelection] = useState<RowSelectionState>({});

  const openDialog = useDialogOpen();
  const closeDialog = useDialogClose();
  const openAlertDialog = useAlertDialogOpen();

  const handleCreate = () => {
    openDialog({
      title: "Thêm diễn viên mới",
      description: "Nhập họ tên và ảnh đại diện (nếu có).",
      content: <ActorCreateForm onSuccess={closeDialog} />,
    });
  };

  const handleEdit = (actor: ActorDto) => {
    openDialog({
      title: "Cập nhật diễn viên",
      description: "Chỉnh sửa họ tên và ảnh đại diện.",
      content: <ActorUpdateForm actor={actor} onSuccess={closeDialog} />,
    });
  };

  const handleDelete = (actor: ActorDto) => {
    openAlertDialog({
      title: `Xóa diễn viên "${actor.fullName}"?`,
      description: IRREVERSIBLE_ACTION_MESSAGE,
      confirmText: "Xóa",
      variant: "destructive",
      onConfirm: async () => {
        await deleteActor.mutateAsync(actor.actorId);
      },
    });
  };

  const handleBulkDelete = async (selectedIds: string[]) => {
    const res = await bulkDeleteActors.mutateAsync({ ids: selectedIds.map(Number) });
    setRowSelection({});
    toastBulkResult(res.data, "xóa", "diễn viên");
  };

  const columns = [
    createSelectionColumn<ActorDto>(),
    ...createActorColumns({
      onEdit: handleEdit,
      onDelete: handleDelete,
    }),
  ];

  return (
    <div>
      <PageHeader
        title="Diễn viên"
        description="Quản lý danh sách diễn viên."
        action={
          <Button onClick={handleCreate}>
            <Plus className="size-4" />
            <span>Thêm diễn viên</span>
          </Button>
        }
      />

      <DataTable
        columns={columns}
        data={actors.data?.items ?? []}
        isLoading={actors.isLoading}
        isRefreshing={actors.isPlaceholderData}
        totalItems={actors.data?.totalItems ?? 0}
        pagination={pagination}
        onPaginationChange={onPaginationChange}
        searchValue={params.search}
        onSearchChange={(value) => setParams({ search: value, page: DEFAULT_PAGE })}
        searchPlaceholder="Tìm theo tên diễn viên..."
        emptyMessage="Chưa có diễn viên nào."
        rowSelection={rowSelection}
        onRowSelectionChange={setRowSelection}
        getRowId={(row) => String(row.actorId)}
        bulkActions={(selectedIds) => (
          <BulkDeleteButton
            count={selectedIds.length}
            itemLabel="diễn viên"
            onConfirm={() => handleBulkDelete(selectedIds)}
            isPending={bulkDeleteActors.isPending}
          />
        )}
      />
    </div>
  );
};

export default ActorsPage;