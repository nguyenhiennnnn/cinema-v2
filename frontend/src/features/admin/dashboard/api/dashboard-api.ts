import { apiGet } from "@/api/axios";
import type { ApiResponse } from "@/types/api";
import type {
  DashboardSummaryDto,
  DashboardInsightsDto,
  DashboardInsightsParams,
} from "../types";
import { ENDPOINTS } from "@/api/endpoints";

export const dashboardApi = {
  getSummary: () => apiGet<ApiResponse<DashboardSummaryDto>>(ENDPOINTS.admin.dashboard.summary),

  getInsights: (params: DashboardInsightsParams) =>
    apiGet<ApiResponse<DashboardInsightsDto>>(ENDPOINTS.admin.dashboard.insights, { params }),
};
