import { apiGet } from "@/api/axios";
import type { ApiResponse, OffsetPageResult } from "@/types/api";
import type { AdminPaymentListParams, AdminPaymentListItemDto, AdminPaymentDetailDto } from "../types";

const BASE = "/admin/payments";

export const paymentsAdminApi = {
  getPayments: (params: AdminPaymentListParams) =>
    apiGet<ApiResponse<OffsetPageResult<AdminPaymentListItemDto>>>(BASE, { params }),

  getPaymentDetail: (id: number) =>
    apiGet<ApiResponse<AdminPaymentDetailDto>>(`${BASE}/${id}`),
};
