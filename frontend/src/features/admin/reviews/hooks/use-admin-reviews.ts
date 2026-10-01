import { useMutation, useQuery } from "@tanstack/react-query";
import { reviewsAdminApi } from "../api/reviews-admin-api";
import { queryKeys } from "@/api/query-keys";
import { queryClient } from "@/api/query-client";
import type { AdminReviewListParams } from "../types";

export function useAdminReviews(params: AdminReviewListParams) {
  return useQuery({
    queryKey: queryKeys.adminReviews.list(params),
    queryFn: () => reviewsAdminApi.getReviews(params),
    select: (res) => res.data,
  });
}

export function useAdminDeleteReview() {
  return useMutation({
    mutationFn: (id: number) => reviewsAdminApi.deleteReview(id),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: queryKeys.adminReviews.all() }),
  });
}
