import { useState } from "react";
import type { RowSelectionState } from "@tanstack/react-table";

import { PageHeader } from "@/components/common/page-header";
import { toast } from "@/components/ui/toast";
import { DataTable } from "@/components/common/data-table/data-table";
import { BulkDeleteButton } from "@/components/common/data-table/bulk-delete-button";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";
import { SelectField } from "@/components/common/select-field";

import {
  useAdminReviews,
  useAdminDeleteReview,
  useAdminBulkDeleteReviews,
} from "../hooks/use-admin-reviews";
import type { AdminReviewDto } from "../types";
import { createReviewColumns } from "../components/review-columns";
import { createSelectionColumn } from "@/components/common/data-table/data-table-selection-column";
import { useAdminReviewParams } from "@/hooks/use-query-params";
import { useTablePagination } from "@/hooks/use-table-pagination";
import { stripNulls } from "@/lib/query-params";

const ratingOptions = [
  { value: "all", label: "Tất cả số sao" },
  ...[5, 4, 3, 2, 1].map((r) => ({ value: String(r), label: `${r} sao` })),
];

const AdminReviewsPage = () => {
  const [params, setParams] = useAdminReviewParams();
  const { pagination, onPaginationChange } = useTablePagination(params, setParams);

  const [rowSelection, setRowSelection] = useState<RowSelectionState>({});

  const reviews = useAdminReviews(stripNulls(params));
  const deleteReview = useAdminDeleteReview();
  const bulkDeleteReviews = useAdminBulkDeleteReviews();

  const openAlertDialog = useAlertDialogOpen();

  const handleDelete = (review: AdminReviewDto) => {
    openAlertDialog({
      title: `Xóa đánh giá của "${review.userName}"?`,
      description: "Đánh giá sẽ bị xóa vĩnh viễn và không thể khôi phục.",
      confirmText: "Xóa",
      variant: "destructive",
      onConfirm: async () => {
        await deleteReview.mutateAsync(review.reviewId);
        setRowSelection((prev) =>
          Object.fromEntries(Object.entries(prev).filter(([key]) => key !== String(review.reviewId))),
        );
      },
    });
  };

  const handleBulkDelete = async (selectedIds: string[]) => {
    const ids = selectedIds.map(Number);
    const res = await bulkDeleteReviews.mutateAsync({ reviewIds: ids });
    const result = res.data;
    setRowSelection({});
    if (result) {
      const { totalSuccess, totalFailed, results } = result;
      const failedReasons = results
        .filter((r) => !r.success && r.reason)
        .map((r) => `#${r.reviewId}: ${r.reason}`)
        .join("; ");
      toast.add({
        title: `Đã xóa ${totalSuccess} đánh giá${totalFailed > 0 ? `, ${totalFailed} thất bại` : ""}.`,
        description: failedReasons || undefined,
        type: totalFailed > 0 ? "warning" : "success",
      });
    }
  };

  const selectionCol = createSelectionColumn<AdminReviewDto>();
  const columns = [
    selectionCol,
    ...createReviewColumns({ onDelete: handleDelete }),
  ];

  return (
    <div>
      <PageHeader
        title="Đánh giá phim"
        description="Quản lý và kiểm duyệt các đánh giá từ người dùng."
      />

      <div className="mb-4 flex justify-end">
        <SelectField
          value={params.rating ? String(params.rating) : "all"}
          onChange={(v) =>
            setParams({ rating: v === "all" ? null : Number(v), page: 1 })
          }
          options={ratingOptions}
          getOptionValue={(o) => o.value}
          getOptionLabel={(o) => o.label}
          className="w-40"
        />
      </div>

      <DataTable
        columns={columns}
        data={reviews.data?.items ?? []}
        isLoading={reviews.isLoading}
        isRefreshing={reviews.isPlaceholderData}
        totalItems={reviews.data?.totalItems ?? 0}
        pagination={pagination}
        onPaginationChange={onPaginationChange}
        emptyMessage="Chưa có đánh giá nào."
        rowSelection={rowSelection}
        onRowSelectionChange={setRowSelection}
        getRowId={(row) => String(row.reviewId)}
        bulkActions={(selectedIds) => (
          <BulkDeleteButton
            count={selectedIds.length}
            itemLabel="đánh giá"
            onConfirm={() => handleBulkDelete(selectedIds)}
            isPending={bulkDeleteReviews.isPending}
          />
        )}
      />
    </div>
  );
};

export default AdminReviewsPage;
