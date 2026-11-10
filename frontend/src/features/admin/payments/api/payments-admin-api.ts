import { apiGet } from "@/api/axios";
import type { ApiResponse, OffsetPageResult } from "@/types/api";
import type { AdminPaymentListParams, AdminPaymentListItemDto, AdminPaymentDetailDto } from "../types";
import { ENDPOINTS } from "@/api/endpoints";

export const paymentsAdminApi = {
  getPayments: (params: AdminPaymentListParams) =>
    apiGet<ApiResponse<OffsetPageResult<AdminPaymentListItemDto>>>(ENDPOINTS.admin.payments.root, { params }),

  getPaymentDetail: (id: number) =>
    apiGet<ApiResponse<AdminPaymentDetailDto>>(ENDPOINTS.admin.payments.byId(id)),
};
