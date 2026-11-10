export interface RevenueReportParams {
  fromDate: string;
  toDate: string;
  groupBy: string;
}

export interface RevenueReportItemDto {
  period: string;
  ticketRevenue: number;
  comboRevenue: number;
  totalRevenue: number;
  bookingCount: number;
}

export interface RevenueSummaryDto {
  totalRevenue: number;
  totalBookings: number;
}

export interface RevenueReportDto {
  groupBy: string;
  items: RevenueReportItemDto[];
  summary: RevenueSummaryDto;
}

export interface TopMovieReportParams {
  fromDate: string;
  toDate: string;
  limit?: number;
}

export interface TopMovieDto {
  movieId: number;
  title: string;
  ticketsSold: number;
  revenue: number;
}

export interface ReportDateParams {
  fromDate: string;
  toDate: string;
}

export interface EmployeePerformanceDto {
  employeeId: number;
  fullName: string;
  ticketsSold: number;
  revenue: number;
  bookingCount: number;
}
