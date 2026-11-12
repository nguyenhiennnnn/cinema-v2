import { keepPreviousData, useMutation, useQuery } from "@tanstack/react-query";
import { reviewsAdminApi } from "../api/reviews-admin-api";
import { queryKeys } from "@/api/query-keys";
import type { AdminReviewListParams, BulkDeleteReviewsRequest } from "../types";

export function useAdminReviews(params: AdminReviewListParams) {
  return useQuery({
    queryKey: queryKeys.adminReviews.list(params),
    queryFn: () => reviewsAdminApi.getReviews(params),
    placeholderData: keepPreviousData,
    select: (res) => res.data,
  });
}

export function useAdminDeleteReview() {
  return useMutation({
    meta: { invalidates: "reviewChanged" },
    mutationFn: (id: number) => reviewsAdminApi.deleteReview(id),
  });
}

export function useAdminBulkDeleteReviews() {
  return useMutation({
    meta: { invalidates: "reviewChanged" },
    mutationFn: (body: BulkDeleteReviewsRequest) =>
      reviewsAdminApi.bulkDeleteReviews(body),
  });
}
