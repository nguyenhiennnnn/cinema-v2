import type { ColumnFiltersState } from "@tanstack/react-table";
import { Plus } from "lucide-react";

import { PageHeader } from "@/components/common/page-header";
import { Button } from "@/components/ui/button";
import { toast } from "@/components/ui/toast";
import { DataTable } from "@/components/common/data-table/data-table";
import { useDialogClose, useDialogOpen } from "@/stores/dialog-store";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";

import {
  useAdminDeleteScreenType,
  useAdminScreenTypes,
} from "../hooks/use-admin-screen-type";
import type { ScreenTypeDto } from "../types";

import { createScreenTypeColumns } from "../components/columns/sreen-type-columns";

import { ScreenTypeCreateForm } from "../components/forms/screen-types/screen-type-create-form";
import { ScreenTypeUpdateForm } from "../components/forms/screen-types/screen-type-update-form";
import { useTablePagination } from "@/hooks/use-table-pagination";
import { useAdminListParams } from "@/hooks/use-query-params";
import { DEFAULT_PAGE } from "@/config/default-constants";

const ScreenTypesPage = () => {
  const [params, setParams] = useAdminListParams();
  const { pagination, onPaginationChange } = useTablePagination(params, setParams);

  const screenTypes = useAdminScreenTypes({
    page: params.page,
    pageSize: params.pageSize,
  });
  const deleteScreenType = useAdminDeleteScreenType();

  const openDialog = useDialogOpen();
  const closeDialog = useDialogClose();
  const openAlertDialog = useAlertDialogOpen();

  const handleCreate = () => {
    openDialog({
      title: "Thêm loại màn hình mới",
      description: "Nhập tên định dạng chiếu mới (2D, 3D, IMAX...).",
      content: <ScreenTypeCreateForm onSuccess={closeDialog} />,
    });
  };

  const handleEdit = (screenType: ScreenTypeDto) => {
    openDialog({
      title: "Cập nhật loại màn hình",
      description: "Chỉnh sửa thông tin loại màn hình.",
      content: (
        <ScreenTypeUpdateForm screenType={screenType} onSuccess={closeDialog} />
      ),
    });
  };

  const handleDelete = (screenType: ScreenTypeDto) => {
    openAlertDialog({
      title: `Xóa loại màn hình "${screenType.name}"?`,
      description: "Hành động này không thể hoàn tác.",
      confirmText: "Xóa",
      variant: "destructive",
      onConfirm: async () => {
        await deleteScreenType.mutateAsync(screenType.screenTypeId);
        toast.add({ title: "Đã xóa loại màn hình.", type: "success" });
      },
    });
  };

  const columns = createScreenTypeColumns({
    onEdit: handleEdit,
    onDelete: handleDelete,
  });

  const columnFilters: ColumnFiltersState = params.search
    ? [{ id: "name", value: params.search }]
    : [];

  return (
    <div>
      <PageHeader
        title="Loại màn hình"
        description="Quản lý các định dạng chiếu của hệ thống rạp."
        action={
          <Button onClick={handleCreate}>
            <Plus className="size-4" />
            <span>Thêm loại màn hình</span>
          </Button>
        }
      />

      <DataTable
        columns={columns}
        data={screenTypes.data?.items ?? []}
        isLoading={screenTypes.isLoading}
        totalItems={screenTypes.data?.totalItems ?? 0}
        pagination={pagination}
        onPaginationChange={onPaginationChange}
        columnFilters={columnFilters}
        searchValue={params.search}
        onSearchChange={(value) => setParams({ search: value, page: DEFAULT_PAGE })}
        searchPlaceholder="Tìm theo tên loại màn hình..."
        emptyMessage="Chưa có loại màn hình nào."
      />
    </div>
  );
};

export default ScreenTypesPage;