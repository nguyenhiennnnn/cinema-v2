import { apiGet } from "@/api/axios";
import type { ApiResponse } from "@/types/api";
import type {
  DashboardSummaryDto,
  DashboardInsightsDto,
  DashboardInsightsParams,
} from "../types";

const BASE = "/admin/dashboard";

export const dashboardApi = {
  getSummary: () => apiGet<ApiResponse<DashboardSummaryDto>>(`${BASE}/summary`),

  getInsights: (params: DashboardInsightsParams) =>
    apiGet<ApiResponse<DashboardInsightsDto>>(`${BASE}/insights`, { params }),
};
