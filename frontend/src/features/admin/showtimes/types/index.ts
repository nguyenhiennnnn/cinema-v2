export interface AdminShowtimeListParams {
  roomId?: number;
  movieId?: number;
  date?: string;
  status?: string;
  page?: number;
  pageSize?: number;
}

export interface AdminShowtimeDto {
  showtimeId: number;
  movieId: number;
  movieTitle: string;
  roomId: number;
  roomName: string;
  startTime: string;
  endTime: string;
  basePrice: number;
  status: string;
}

export interface UpsertShowtimeRequest {
  movieId: number;
  roomId: number;
  startTime: string;
  basePrice: number;
  allowAfterHours?: boolean;
}

export interface CreateShowtimeResponse {
  showtimeId: number;
  movieId: number;
  movieTitle: string;
  roomId: number;
  roomName: string;
  startTime: string;
  endTime: string;
  basePrice: number;
  status: string;
  seatsGenerated: number;
}

export interface DeleteShowtimeResponse {
  showtimeId: number;
  status: string;
  manualRefundRequiredCount: number;
  autoRefundQueuedCount?: number;
}

export interface BulkDateRange {
  from: string;
  to: string;
}

export interface BulkCreateShowtimesRequest {
  movieId: number;
  roomId: number;
  dateRange: BulkDateRange;
  daysOfWeek: string[];
  times: string[];
  basePrice: number;
  allowAfterHours?: boolean;
}

export interface BulkCreatedShowtimeItem {
  showtimeId: number;
  startTime: string;
  seatsGenerated: number;
}

export interface BulkSkippedShowtimeItem {
  date: string;
  time: string;
  reason: string;
}

export interface BulkCreateShowtimesResponse {
  created: BulkCreatedShowtimeItem[];
  skipped: BulkSkippedShowtimeItem[];
  totalCreated: number;
  totalSkipped: number;
}

export interface BulkDeleteShowtimesRequest {
  showtimeIds: number[];
}

export interface BulkDeleteShowtimesResultItem {
  showtimeId: number;
  success: boolean;
  reason?: string | null;
}

export interface BulkDeleteShowtimesResponse {
  results: BulkDeleteShowtimesResultItem[];
  totalSuccess: number;
  totalFailed: number;
}

export interface AdminShowtimeCalendarParams {
  from: string;
  to: string;
  roomId?: number;
  movieId?: number;
  status?: string;
}

export interface ShowtimeCalendarItemDto {
  showtimeId: number;
  movieId: number;
  movieTitle: string;
  moviePosterUrl?: string | null;
  roomId: number;
  roomName: string;
  screenType?: string | null;
  startTime: string;
  endTime: string;
  basePrice: number;
  availableSeats: number;
  totalSeats: number;
  status: string;
}

export interface ShowtimeCalendarDayDto {
  date: string;
  showtimes: ShowtimeCalendarItemDto[];
}

export interface ShowtimeCalendarResponseDto {
  from: string;
  to: string;
  days: ShowtimeCalendarDayDto[];
}

export interface AutoScheduleMovieRequest {
  movieId: number;
  shows: number;
  basePrice?: number;
}

export interface AutoScheduleRequest {
  date: string;
  movies: AutoScheduleMovieRequest[];
  basePrice: number;
  roomIds?: number[];
  allowAfterHours?: boolean;
  toDate?: string;
}

export interface AutoScheduleItemDto {
  movieId: number;
  movieTitle: string;
  roomId: number;
  roomName: string;
  startTime: string;
  endTime: string;
  basePrice: number;
}

export interface AutoScheduleUnplacedDto {
  movieId: number;
  movieTitle: string;
  requested: number;
  placed: number;
}

export interface AutoScheduleResponseDto {
  date: string;
  toDate?: string | null;
  items: AutoScheduleItemDto[];
  unplaced: AutoScheduleUnplacedDto[];
}

export interface ApplyAutoScheduleRequest {
  items: UpsertShowtimeRequest[];
}

export interface CloneShowtimesRequest {
  sourceFrom: string;
  sourceTo: string;
  shiftDays: number;
  roomIds?: number[];
  allowAfterHours?: boolean;
}
