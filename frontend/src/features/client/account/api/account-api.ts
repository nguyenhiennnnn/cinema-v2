import { apiGet, apiPost, apiPut } from "@/api/axios";
import type { ApiResponse, CursorPageResult } from "@/types/api";
import type { UserProfileDto, UpdateProfileRequest, UserMyReviewDto, MyReviewListParams } from "../types";
import type { ChangePasswordRequest, MessageResponse } from "@/features/auth/types";

export const accountApi = {
  getMyProfile: () =>
    apiGet<ApiResponse<UserProfileDto>>("/users/me"),

  updateMyProfile: (body: UpdateProfileRequest) =>
    apiPut<ApiResponse<UserProfileDto>>("/users/me", body),

  getMyReviews: (params?: MyReviewListParams) =>
    apiGet<ApiResponse<CursorPageResult<UserMyReviewDto>>>("/users/me/reviews", { params }),

  changePassword: (body: ChangePasswordRequest) =>
    apiPost<ApiResponse<MessageResponse>>("/users/me/change-password", body),
};
