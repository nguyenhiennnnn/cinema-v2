// ── Admin showtime list ───────────────────────────────────────────────────────

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

// ── Create / update showtime ──────────────────────────────────────────────────

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
  /** Số khoản hoàn tiền online đã được đưa vào hàng đợi hoàn tự động. */
  autoRefundQueuedCount?: number;
}

// ── Bulk create ───────────────────────────────────────────────────────────────

export interface BulkDateRange {
  from: string; // "yyyy-MM-dd"
  to: string;
}

export interface BulkCreateShowtimesRequest {
  movieId: number;
  roomId: number;
  dateRange: BulkDateRange;
  daysOfWeek: string[]; // ["Mon", "Tue", ...]
  times: string[]; // ["09:00", "13:00", ...]
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

// ── Bulk delete ───────────────────────────────────────────────────────────────

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

// ── Admin showtime calendar (theo khoảng ngày) ─────────────────────────────────
// Backend AdminShowtimeCalendarFilter(DateOnly From, DateOnly To, int? RoomId, int? MovieId, string? Status)

export interface AdminShowtimeCalendarParams {
  from: string; // "yyyy-MM-dd"
  to: string; // "yyyy-MM-dd"
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
  date: string; // "dd/MM/yyyy"
  showtimes: ShowtimeCalendarItemDto[];
}

export interface ShowtimeCalendarResponseDto {
  from: string;
  to: string;
  days: ShowtimeCalendarDayDto[];
}

// ── Auto schedule ─────────────────────────────────────────────────────────────

export interface AutoScheduleMovieRequest {
  movieId: number;
  shows: number;
  basePrice?: number;
}

export interface AutoScheduleRequest {
  date: string; // "yyyy-MM-dd"
  movies: AutoScheduleMovieRequest[];
  basePrice: number;
  roomIds?: number[];
  allowAfterHours?: boolean;
  /** Xếp nhiều ngày: từ `date` đến `toDate` (tối đa 31 ngày). */
  toDate?: string; // "yyyy-MM-dd"
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
  date: string; // "dd/MM/yyyy"
  toDate?: string | null; // "dd/MM/yyyy", có khi xếp nhiều ngày
  items: AutoScheduleItemDto[];
  unplaced: AutoScheduleUnplacedDto[];
}

export interface ApplyAutoScheduleRequest {
  items: UpsertShowtimeRequest[];
}

// ── Clone (nhân bản lịch chiếu sang tuần khác) ───────────────────────────────

export interface CloneShowtimesRequest {
  sourceFrom: string; // "yyyy-MM-dd"
  sourceTo: string;
  shiftDays: number; // số ngày dịch chuyển, >= 1 (7 = tuần sau)
  roomIds?: number[];
  allowAfterHours?: boolean;
}
