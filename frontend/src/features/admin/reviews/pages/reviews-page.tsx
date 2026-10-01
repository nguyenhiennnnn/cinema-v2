import { PageHeader } from "@/components/common/page-header";
import { toast } from "@/components/ui/toast";
import { DataTable } from "@/components/common/data-table/data-table";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";

import { useAdminReviews, useAdminDeleteReview } from "../hooks/use-admin-reviews";
import type { AdminReviewDto } from "../types";
import { createReviewColumns } from "../components/review-columns";
import { useAdminReviewParams } from "@/hooks/use-query-params";
import { useTablePagination } from "@/hooks/use-table-pagination";
import { stripNulls } from "@/lib/query-params";
import { SelectField } from "@/components/common/select-field";

const ratingOptions = [
  { value: "all", label: "Tất cả số sao" },
  ...[5, 4, 3, 2, 1].map((r) => ({ value: String(r), label: `${r} sao` })),
];

const AdminReviewsPage = () => {
  const [params, setParams] = useAdminReviewParams();
  const { pagination, onPaginationChange } = useTablePagination(params, setParams);

  const reviews = useAdminReviews(stripNulls(params));
  const deleteReview = useAdminDeleteReview();

  const openAlertDialog = useAlertDialogOpen();

  const handleDelete = (review: AdminReviewDto) => {
    openAlertDialog({
      title: `Xóa đánh giá của "${review.userName}"?`,
      description: "Đánh giá sẽ bị xóa vĩnh viễn và không thể khôi phục.",
      confirmText: "Xóa",
      variant: "destructive",
      onConfirm: async () => {
        await deleteReview.mutateAsync(review.reviewId);
        toast.add({ title: "Đã xóa đánh giá.", type: "success" });
      },
    });
  };

  const columns = createReviewColumns({ onDelete: handleDelete });



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
        totalItems={reviews.data?.totalItems ?? 0}
        pagination={pagination}
        onPaginationChange={onPaginationChange}
        emptyMessage="Chưa có đánh giá nào."
      />
    </div>
  );
};

export default AdminReviewsPage;