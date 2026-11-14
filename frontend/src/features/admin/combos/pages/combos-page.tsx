import { useState } from "react";
import type { ColumnFiltersState, RowSelectionState } from "@tanstack/react-table";
import { Plus, Power, PowerOff } from "lucide-react";

import { PageHeader } from "@/components/common/page-header";
import { Button } from "@/components/ui/button";
import { DataTable } from "@/components/common/data-table/data-table";
import { BulkActionButton } from "@/components/common/data-table/bulk-action-button";
import { createSelectionColumn } from "@/components/common/data-table/data-table-selection-column";
import { toastBulkResult } from "@/lib/bulk";
import { useDialogClose, useDialogOpen } from "@/stores/dialog-store";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";

import {
  useAdminBulkSetComboActive,
  useAdminCombos,
  useAdminToggleCombo,
} from "../hooks/use-admin-combo";
import type { AdminComboDto } from "@/features/admin/combos/types";
import { createComboColumns } from "../components/combo-columns";
import { ComboCreateForm } from "../components/forms/combo-create-form";
import { ComboUpdateForm } from "../components/forms/combo-update-form";
import { useAdminListParams } from "@/hooks/use-query-params";
import { useTablePagination } from "@/hooks/use-table-pagination";
import { DEFAULT_PAGE } from "@/config/default-constants";

const AdminCombosPage = () => {
  const [params, setParams] = useAdminListParams();
  const { pagination, onPaginationChange } = useTablePagination(params, setParams);

  const combos = useAdminCombos({
    page: pagination.pageIndex + 1,
    pageSize: pagination.pageSize,
  });
  const toggleCombo = useAdminToggleCombo();
  const bulkSetActive = useAdminBulkSetComboActive();
  const [rowSelection, setRowSelection] = useState<RowSelectionState>({});

  const openDialog = useDialogOpen();
  const closeDialog = useDialogClose();
  const openAlertDialog = useAlertDialogOpen();

  const handleCreate = () => {
    openDialog({
      title: "Thêm combo mới",
      description: "Nhập tên, mô tả, giá và hình ảnh cho combo.",
      content: <ComboCreateForm onSuccess={closeDialog} />,
    });
  };

  const handleEdit = (combo: AdminComboDto) => {
    openDialog({
      title: "Cập nhật combo",
      description: "Chỉnh sửa tên, mô tả, giá và hình ảnh.",
      content: <ComboUpdateForm combo={combo} onSuccess={closeDialog} />,
    });
  };

  const handleToggle = (combo: AdminComboDto) => {
    const willActivate = !combo.isActive;
    openAlertDialog({
      title: willActivate
        ? `Kích hoạt lại combo "${combo.name}"?`
        : `Ngừng bán combo "${combo.name}"?`,
      description: willActivate
        ? "Combo sẽ hiển thị lại và có thể được đặt mua."
        : "Combo sẽ bị ẩn khỏi danh sách mua hàng cho đến khi kích hoạt lại.",
      confirmText: willActivate ? "Kích hoạt" : "Ngừng bán",
      variant: willActivate ? "default" : "destructive",
      onConfirm: async () => {
        await toggleCombo.mutateAsync(combo.comboId);
      },
    });
  };

  const handleBulkSetActive = async (selectedIds: string[], isActive: boolean) => {
    const res = await bulkSetActive.mutateAsync({ ids: selectedIds.map(Number), isActive });
    setRowSelection({});
    toastBulkResult(res.data, isActive ? "kích hoạt" : "ngừng bán", "combo");
  };

  const columns = [
    createSelectionColumn<AdminComboDto>(),
    ...createComboColumns({
      onEdit: handleEdit,
      onToggle: handleToggle,
    }),
  ];

  const columnFilters: ColumnFiltersState = params.search
    ? [{ id: "name", value: params.search }]
    : [];

  return (
    <div>
      <PageHeader
        title="Combo bắp nước"
        description="Quản lý danh sách combo, giá bán và trạng thái kinh doanh."
        action={
          <Button onClick={handleCreate}>
            <Plus className="size-4" />
            <span>Thêm combo</span>
          </Button>
        }
      />

      <DataTable
        columns={columns}
        data={combos.data?.items ?? []}
        isLoading={combos.isLoading}
        isRefreshing={combos.isPlaceholderData}
        totalItems={combos.data?.totalItems ?? 0}
        pagination={pagination}
        onPaginationChange={onPaginationChange}
        columnFilters={columnFilters}
        searchValue={params.search}
        onSearchChange={(value) => setParams({ search: value, page: DEFAULT_PAGE })}
        searchPlaceholder="Tìm theo tên combo..."
        emptyMessage="Chưa có combo nào."
        rowSelection={rowSelection}
        onRowSelectionChange={setRowSelection}
        getRowId={(row) => String(row.comboId)}
        bulkActions={(selectedIds) => (
          <>
            <BulkActionButton
              count={selectedIds.length}
              itemLabel="combo"
              actionLabel="Kích hoạt"
              icon={Power}
              variant="outline"
              description="Các combo được chọn sẽ hiển thị lại và có thể được đặt mua."
              onConfirm={() => handleBulkSetActive(selectedIds, true)}
              isPending={bulkSetActive.isPending}
            />
            <BulkActionButton
              count={selectedIds.length}
              itemLabel="combo"
              actionLabel="Ngừng bán"
              icon={PowerOff}
              variant="destructive"
              description="Các combo được chọn sẽ bị ẩn khỏi danh sách mua hàng cho đến khi kích hoạt lại."
              onConfirm={() => handleBulkSetActive(selectedIds, false)}
              isPending={bulkSetActive.isPending}
            />
          </>
        )}
      />
    </div>
  );
};

export default AdminCombosPage;