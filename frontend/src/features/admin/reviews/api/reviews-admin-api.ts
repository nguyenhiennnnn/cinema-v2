import { apiDelete, apiGet } from "@/api/axios";
import type { ApiResponse, OffsetPageResult } from "@/types/api";
import type { AdminReviewListParams, AdminReviewDto, DeleteReviewResponse, BulkDeleteReviewsRequest, BulkDeleteReviewsResponse } from "../types";
import { ENDPOINTS } from "@/api/endpoints";

export const reviewsAdminApi = {
  getReviews: (params: AdminReviewListParams) =>
    apiGet<ApiResponse<OffsetPageResult<AdminReviewDto>>>(ENDPOINTS.admin.reviews.root, { params }),

  deleteReview: (id: number) =>
    apiDelete<ApiResponse<DeleteReviewResponse>>(ENDPOINTS.admin.reviews.byId(id)),

  bulkDeleteReviews: (body: BulkDeleteReviewsRequest) =>
    apiDelete<ApiResponse<BulkDeleteReviewsResponse>>(ENDPOINTS.admin.reviews.bulk, {
      data: body,
    }),
};
