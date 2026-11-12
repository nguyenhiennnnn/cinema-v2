import type { ColumnFiltersState } from "@tanstack/react-table";
import { Plus } from "lucide-react";

import { PageHeader } from "@/components/common/page-header";
import { Button } from "@/components/ui/button";
import { DataTable } from "@/components/common/data-table/data-table";
import { useDialogClose, useDialogOpen } from "@/stores/dialog-store";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";

import {
  useAdminPositions,
  useAdminDeletePosition,
} from "../hooks/use-admin-position";
import type { PositionDto } from "../types";
import { createPositionColumns } from "../components/columns/position-columns";
import { PositionCreateForm } from "../components/forms/positions/position-create-form";
import { PositionUpdateForm } from "../components/forms/positions/position-update-form";
import { useTablePagination } from "@/hooks/use-table-pagination";
import { useAdminListParams } from "@/hooks/use-query-params";
import { DEFAULT_PAGE } from "@/config/default-constants";

const PositionsPage = () => {
  const [params, setParams] = useAdminListParams();
  const { pagination, onPaginationChange } = useTablePagination(params, setParams);

  const positions = useAdminPositions({
    page: params.page,
    pageSize: params.pageSize,
  });
  const deletePosition = useAdminDeletePosition();

  const openDialog = useDialogOpen();
  const closeDialog = useDialogClose();
  const openAlertDialog = useAlertDialogOpen();

  const handleCreate = () => {
    openDialog({
      title: "Thêm chức vụ mới",
      description: "Nhập tên chức vụ nhân sự.",
      content: <PositionCreateForm onSuccess={closeDialog} />,
    });
  };

  const handleEdit = (position: PositionDto) => {
    openDialog({
      title: "Cập nhật chức vụ",
      description: "Chỉnh sửa tên chức vụ.",
      content: <PositionUpdateForm position={position} onSuccess={closeDialog} />,
    });
  };

  const handleDelete = (position: PositionDto) => {
    openAlertDialog({
      title: `Xóa chức vụ "${position.name}"?`,
      description: "Hành động này không thể hoàn tác.",
      confirmText: "Xóa",
      variant: "destructive",
      onConfirm: async () => {
        await deletePosition.mutateAsync(position.positionId);
      },
    });
  };

  const columns = createPositionColumns({
    onEdit: handleEdit,
    onDelete: handleDelete,
  });

  const columnFilters: ColumnFiltersState = params.search
    ? [{ id: "name", value: params.search }]
    : [];

  return (
    <div>
      <PageHeader
        title="Chức vụ"
        description="Quản lý danh sách chức vụ nhân sự trong rạp."
        action={
          <Button onClick={handleCreate}>
            <Plus className="size-4" />
            <span>Thêm chức vụ</span>
          </Button>
        }
      />

      <DataTable
        columns={columns}
        data={positions.data?.items ?? []}
        isLoading={positions.isLoading}
        isRefreshing={positions.isPlaceholderData}
        totalItems={positions.data?.totalItems ?? 0}
        pagination={pagination}
        onPaginationChange={onPaginationChange}
        columnFilters={columnFilters}
        searchValue={params.search}
        onSearchChange={(value) => setParams({ search: value, page: DEFAULT_PAGE })}
        searchPlaceholder="Tìm theo tên chức vụ..."
        emptyMessage="Chưa có chức vụ nào."
      />
    </div>
  );
};

export default PositionsPage;