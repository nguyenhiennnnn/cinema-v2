import { Plus } from "lucide-react";

import { PageHeader } from "@/components/common/page-header";
import { Button } from "@/components/ui/button";
import { toast } from "@/components/ui/toast";
import { DataTable } from "@/components/common/data-table/data-table";
import { useDialogClose, useDialogOpen } from "@/stores/dialog-store";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";

import { useTablePagination } from "@/hooks/use-table-pagination";

import { useAdminPersonParams } from "@/hooks/use-query-params";
import { useAdminActors, useAdminDeleteActor } from "../hooks/use-admin-actor";
import type { ActorDto } from "../types";
import { createActorColumns } from "../components/columns/actor-columns";
import { ActorCreateForm } from "../components/forms/actors/actor-create-form";
import { ActorUpdateForm } from "../components/forms/actors/actor-update-form";
import { DEFAULT_PAGE } from "@/config/default-constants";

const ActorsPage = () => {
  const [params, setParams] = useAdminPersonParams();
  const { pagination, onPaginationChange } = useTablePagination(params, setParams);

  const actors = useAdminActors({
    page: params.page,
    pageSize: params.pageSize,
    search: params.search,
  });
  const deleteActor = useAdminDeleteActor();

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
      description: "Hành động này không thể hoàn tác.",
      confirmText: "Xóa",
      variant: "destructive",
      onConfirm: async () => {
        await deleteActor.mutateAsync(actor.actorId);
        toast.add({ title: "Đã xóa diễn viên.", type: "success" });
      },
    });
  };

  const columns = createActorColumns({
    onEdit: handleEdit,
    onDelete: handleDelete,
  });

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
        totalItems={actors.data?.totalItems ?? 0}
        pagination={pagination}
        onPaginationChange={onPaginationChange}
        searchValue={params.search}
        onSearchChange={(value) => setParams({ search: value, page: DEFAULT_PAGE })}
        searchPlaceholder="Tìm theo tên diễn viên..."
        emptyMessage="Chưa có diễn viên nào."
      />
    </div>
  );
};

export default ActorsPage;