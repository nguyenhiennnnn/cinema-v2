export interface DashboardSummaryDto {
  todayRevenue: number;
  yesterdayRevenue: number;
  revenueChangePercent: number | null;
  ticketsSoldToday: number;
  confirmedBookingsToday: number;
  activePendingBookings: number;
  todayOccupancyPercent: number | null;
}

export interface DashboardInsightsParams {
  days?: number;
}

export interface DashboardRevenueTrendItemDto {
  period: string;
  ticketRevenue: number;
  comboRevenue: number;
  totalRevenue: number;
  bookingCount: number;
}

export interface DashboardShowtimeItemDto {
  showtimeId: number;
  movieTitle: string;
  roomName: string;
  startTime: string;
  bookedSeats: number;
  totalSeats: number;
}

export interface DashboardTopMovieDto {
  movieId: number;
  title: string;
  ticketsSold: number;
  revenue: number;
}

export interface DashboardInsightsDto {
  revenueTrend: DashboardRevenueTrendItemDto[];
  todayShowtimes: DashboardShowtimeItemDto[];
  topMovies: DashboardTopMovieDto[];
}
