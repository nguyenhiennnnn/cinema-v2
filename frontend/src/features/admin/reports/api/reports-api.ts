import { apiGet } from "@/api/axios";
import type { ApiResponse } from "@/types/api";
import type {
  RevenueReportParams,
  RevenueReportDto,
  TopMovieReportParams,
  TopMovieDto,
  ReportDateParams,
  EmployeePerformanceDto,
} from "../types";

const BASE = "/admin/reports";

export const reportsApi = {
  getRevenue: (params: RevenueReportParams) =>
    apiGet<ApiResponse<RevenueReportDto>>(`${BASE}/revenue`, { params }),

  getTopMovies: (params: TopMovieReportParams) =>
    apiGet<ApiResponse<TopMovieDto[]>>(`${BASE}/top-movies`, { params }),

  getEmployeePerformance: (params: ReportDateParams) =>
    apiGet<ApiResponse<EmployeePerformanceDto[]>>(`${BASE}/employee-performance`, { params }),
};
