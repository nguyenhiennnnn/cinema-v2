import type { ColumnFiltersState } from "@tanstack/react-table";
import { Plus } from "lucide-react";

import { PageHeader } from "@/components/common/page-header";
import { Button } from "@/components/ui/button";
import { DataTable } from "@/components/common/data-table/data-table";
import { useDialogClose, useDialogOpen } from "@/stores/dialog-store";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";

import {
  useAdminSeatTypes,
  useAdminDeleteSeatType,
} from "../hooks/use-admin-seat-type";
import type { SeatTypeDto } from "../types";
import { createSeatTypeColumns } from "../components/columns/seat-type-columns";
import { SeatTypeCreateForm } from "../components/forms/seat-types/seat-type-create-form";
import { SeatTypeUpdateForm } from "../components/forms/seat-types/seat-type-update-form";
import { useTablePagination } from "@/hooks/use-table-pagination";
import { useAdminListParams } from "@/hooks/use-query-params";
import { DEFAULT_PAGE } from "@/config/default-constants";

const SeatTypesPage = () => {
  const [params, setParams] = useAdminListParams();
  const { pagination, onPaginationChange } = useTablePagination(params, setParams);

  const seatTypes = useAdminSeatTypes({
    page: params.page,
    pageSize: params.pageSize,
  });
  const deleteSeatType = useAdminDeleteSeatType();

  const openDialog = useDialogOpen();
  const closeDialog = useDialogClose();
  const openAlertDialog = useAlertDialogOpen();

  const handleCreate = () => {
    openDialog({
      title: "Thêm loại ghế mới",
      description: "Nhập tên loại ghế và hệ số giá.",
      content: <SeatTypeCreateForm onSuccess={closeDialog} />,
    });
  };

  const handleEdit = (seatType: SeatTypeDto) => {
    openDialog({
      title: "Cập nhật loại ghế",
      description: "Chỉnh sửa tên loại ghế và hệ số giá.",
      content: <SeatTypeUpdateForm seatType={seatType} onSuccess={closeDialog} />,
    });
  };

  const handleDelete = (seatType: SeatTypeDto) => {
    openAlertDialog({
      title: `Xóa loại ghế "${seatType.name}"?`,
      description: "Hành động này không thể hoàn tác.",
      confirmText: "Xóa",
      variant: "destructive",
      onConfirm: async () => {
        await deleteSeatType.mutateAsync(seatType.seatTypeId);
      },
    });
  };

  const columns = createSeatTypeColumns({
    onEdit: handleEdit,
    onDelete: handleDelete,
  });

  const columnFilters: ColumnFiltersState = params.search
    ? [{ id: "name", value: params.search }]
    : [];

  return (
    <div>
      <PageHeader
        title="Loại ghế"
        description="Quản lý các loại ghế và hệ số giá tương ứng."
        action={
          <Button onClick={handleCreate}>
            <Plus className="size-4" />
            <span>Thêm loại ghế</span>
          </Button>
        }
      />

      <DataTable
        columns={columns}
        data={seatTypes.data?.items ?? []}
        isLoading={seatTypes.isLoading}
        totalItems={seatTypes.data?.totalItems ?? 0}
        pagination={pagination}
        onPaginationChange={onPaginationChange}
        columnFilters={columnFilters}
        searchValue={params.search}
        onSearchChange={(value) => setParams({ search: value, page: DEFAULT_PAGE })}
        searchPlaceholder="Tìm theo tên loại ghế..."
        emptyMessage="Chưa có loại ghế nào."
      />
    </div>
  );
};

export default SeatTypesPage;