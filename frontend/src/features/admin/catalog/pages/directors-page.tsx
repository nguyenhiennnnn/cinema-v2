import { Plus } from "lucide-react";

import { PageHeader } from "@/components/common/page-header";
import { Button } from "@/components/ui/button";
import { toast } from "@/components/ui/toast";
import { DataTable } from "@/components/common/data-table/data-table";
import { useDialogClose, useDialogOpen } from "@/stores/dialog-store";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";

import {
  useAdminDirectors,
  useAdminDeleteDirector,
} from "../hooks/use-admin-director";
import type { DirectorDto } from "../types";
import { createDirectorColumns } from "../components/columns/director-columns";
import { DirectorCreateForm } from "../components/forms/directors/director-create-form";
import { DirectorUpdateForm } from "../components/forms/directors/director-update-form";
import { useAdminPersonParams } from "@/hooks/use-query-params";
import { useTablePagination } from "@/hooks/use-table-pagination";
import { DEFAULT_PAGE } from "@/config/default-constants";

const DirectorsPage = () => {
  const [params, setParams] = useAdminPersonParams();
  const { pagination, onPaginationChange } = useTablePagination(params, setParams);

  const directors = useAdminDirectors({
    page: params.page,
    pageSize: params.pageSize,
    search: params.search,
  });

  const deleteDirector = useAdminDeleteDirector();

  const openDialog = useDialogOpen();
  const closeDialog = useDialogClose();
  const openAlertDialog = useAlertDialogOpen();

  const handleCreate = () => {
    openDialog({
      title: "Thêm đạo diễn mới",
      description: "Nhập họ tên và ảnh đại diện (nếu có).",
      content: <DirectorCreateForm onSuccess={closeDialog} />,
    });
  };

  const handleEdit = (director: DirectorDto) => {
    openDialog({
      title: "Cập nhật đạo diễn",
      description: "Chỉnh sửa họ tên và ảnh đại diện.",
      content: (
        <DirectorUpdateForm director={director} onSuccess={closeDialog} />
      ),
    });
  };

  const handleDelete = (director: DirectorDto) => {
    openAlertDialog({
      title: `Xóa đạo diễn "${director.fullName}"?`,
      description: "Hành động này không thể hoàn tác.",
      confirmText: "Xóa",
      variant: "destructive",
      onConfirm: async () => {
        await deleteDirector.mutateAsync(director.directorId);
        toast.add({ title: "Đã xóa đạo diễn.", type: "success" });
      },
    });
  };

  const columns = createDirectorColumns({
    onEdit: handleEdit,
    onDelete: handleDelete,
  });

  return (
    <div>
      <PageHeader
        title="Đạo diễn"
        description="Quản lý danh sách đạo diễn."
        action={
          <Button onClick={handleCreate}>
            <Plus className="size-4" />
            <span>Thêm đạo diễn</span>
          </Button>
        }
      />

      <DataTable
        columns={columns}
        data={directors.data?.items ?? []}
        isLoading={directors.isLoading}
        totalItems={directors.data?.totalItems ?? 0}
        pagination={pagination}
        onPaginationChange={onPaginationChange}
        searchValue={params.search}
        onSearchChange={(value) => setParams({ search: value, page: DEFAULT_PAGE })}
        searchPlaceholder="Tìm theo tên đạo diễn..."
        emptyMessage="Chưa có đạo diễn nào."
      />
    </div>
  );
};

export default DirectorsPage;