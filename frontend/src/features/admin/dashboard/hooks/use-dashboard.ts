import { useQuery } from "@tanstack/react-query";
import { dashboardApi } from "../api/dashboard-api";
import { queryKeys } from "@/api/query-keys";
import type { DashboardInsightsParams } from "../types";
import { DASHBOARD_REFETCH_INTERVAL_MS } from "@/config/default-constants";

export function useDashboardSummary() {
  return useQuery({
    queryKey: queryKeys.adminDashboard.summary(),
    queryFn: () => dashboardApi.getSummary(),
    select: (res) => res.data,
    refetchInterval: DASHBOARD_REFETCH_INTERVAL_MS,
  });
}

export function useDashboardInsights(params: DashboardInsightsParams = {}) {
  return useQuery({
    queryKey: queryKeys.adminDashboard.insights(params),
    queryFn: () => dashboardApi.getInsights(params),
    select: (res) => res.data,
  });
}
