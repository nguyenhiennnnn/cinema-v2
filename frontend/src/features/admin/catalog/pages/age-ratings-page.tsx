import type { ColumnFiltersState } from "@tanstack/react-table";
import { Plus } from "lucide-react";

import { PageHeader } from "@/components/common/page-header";
import { Button } from "@/components/ui/button";
import { DataTable } from "@/components/common/data-table/data-table";
import { useDialogClose, useDialogOpen } from "@/stores/dialog-store";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";

import {
  useAdminAgeRatings,
  useAdminDeleteAgeRating,
} from "../hooks/use-admin-age-rating";
import type { AgeRatingDto } from "../types";
import { createAgeRatingColumns } from "../components/columns/age-rating-columns";
import { AgeRatingCreateForm } from "../components/forms/age-ratings/age-rating-create-form";
import { AgeRatingUpdateForm } from "../components/forms/age-ratings/age-rating-update-form";
import { useAdminListParams } from "@/hooks/use-query-params";
import { useTablePagination } from "@/hooks/use-table-pagination";
import { DEFAULT_PAGE, IRREVERSIBLE_ACTION_MESSAGE } from "@/config/default-constants";

const AgeRatingsPage = () => {
  const [params, setParams] = useAdminListParams();
  const { pagination, onPaginationChange } = useTablePagination(params, setParams);

  const ageRatings = useAdminAgeRatings({
    page: params.page,
    pageSize: params.pageSize,
  });

  const deleteAgeRating = useAdminDeleteAgeRating();

  const openDialog = useDialogOpen();
  const closeDialog = useDialogClose();
  const openAlertDialog = useAlertDialogOpen();

  const handleCreate = () => {
    openDialog({
      title: "Thêm phân loại độ tuổi mới",
      description: "Nhập mã phân loại (P, K, T13, T16, T18, C...) và mô tả.",
      content: <AgeRatingCreateForm onSuccess={closeDialog} />,
    });
  };

  const handleEdit = (ageRating: AgeRatingDto) => {
    openDialog({
      title: "Cập nhật phân loại độ tuổi",
      description: "Chỉnh sửa mã và mô tả phân loại.",
      content: (
        <AgeRatingUpdateForm ageRating={ageRating} onSuccess={closeDialog} />
      ),
    });
  };

  const handleDelete = (ageRating: AgeRatingDto) => {
    openAlertDialog({
      title: `Xóa phân loại "${ageRating.code}"?`,
      description: IRREVERSIBLE_ACTION_MESSAGE,
      confirmText: "Xóa",
      variant: "destructive",
      onConfirm: async () => {
        await deleteAgeRating.mutateAsync(ageRating.ageRatingId);
      },
    });
  };

  const columns = createAgeRatingColumns({
    onEdit: handleEdit,
    onDelete: handleDelete,
  });

  const columnFilters: ColumnFiltersState = params.search
    ? [{ id: "code", value: params.search }]
    : [];

  return (
    <div>
      <PageHeader
        title="Phân loại độ tuổi"
        description="Quản lý danh sách phân loại độ tuổi phim (P, K, T13, T16, T18, C)."
        action={
          <Button onClick={handleCreate}>
            <Plus className="size-4" />
            <span>Thêm phân loại</span>
          </Button>
        }
      />

      <DataTable
        columns={columns}
        data={ageRatings.data?.items ?? []}
        isLoading={ageRatings.isLoading}
        isRefreshing={ageRatings.isPlaceholderData}
        totalItems={ageRatings.data?.totalItems ?? 0}
        pagination={pagination}
        onPaginationChange={onPaginationChange}
        columnFilters={columnFilters}
        searchValue={params.search}
        onSearchChange={(value) => setParams({ search: value, page: DEFAULT_PAGE })}
        searchPlaceholder="Tìm theo mã phân loại..."
        emptyMessage="Chưa có phân loại độ tuổi nào."
      />
    </div>
  );
};

export default AgeRatingsPage;