import { useQuery } from "@tanstack/react-query";
import { reportsApi } from "../api/reports-api";
import { queryKeys } from "@/api/query-keys";
import type { RevenueReportParams, TopMovieReportParams, ReportDateParams } from "../types";

export function useRevenueReport(params: RevenueReportParams, enabled = true) {
  return useQuery({
    queryKey: queryKeys.adminReports.revenue(params),
    queryFn: () => reportsApi.getRevenue(params),
    enabled,
    select: (res) => res.data,
  });
}

export function useTopMoviesReport(params: TopMovieReportParams, enabled = true) {
  return useQuery({
    queryKey: queryKeys.adminReports.topMovies(params),
    queryFn: () => reportsApi.getTopMovies(params),
    enabled,
    select: (res) => res.data,
  });
}

export function useEmployeePerformanceReport(params: ReportDateParams, enabled = true) {
  return useQuery({
    queryKey: queryKeys.adminReports.employeePerformance(params),
    queryFn: () => reportsApi.getEmployeePerformance(params),
    enabled,
    select: (res) => res.data,
  });
}
