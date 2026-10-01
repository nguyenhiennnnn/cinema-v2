import { apiDelete, apiGet } from "@/api/axios";
import type { ApiResponse, OffsetPageResult } from "@/types/api";
import type { AdminReviewListParams, AdminReviewDto, DeleteReviewResponse } from "../types";

const BASE = "/admin/reviews";

export const reviewsAdminApi = {
  getReviews: (params: AdminReviewListParams) =>
    apiGet<ApiResponse<OffsetPageResult<AdminReviewDto>>>(BASE, { params }),

  deleteReview: (id: number) =>
    apiDelete<ApiResponse<DeleteReviewResponse>>(`${BASE}/${id}`),
};
