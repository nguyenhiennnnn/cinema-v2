import { apiGet } from "@/api/axios";
import type { ApiResponse } from "@/types/api";
import type {
  WholeCinemaScheduleDto,
  ShowtimeSeatsResponseDto,
  SeatSuggestionDto,
} from "../types";

const BASE = "/showtimes";

export const showtimesApi = {
  // GET /showtimes?date=dd/MM/yyyy — date is required, pass as "dd/MM/yyyy"
  getSchedule: (date: string, screenType?: string | null) =>
    apiGet<ApiResponse<WholeCinemaScheduleDto>>(BASE, {
      params: { date, screenType: screenType || undefined },
    }),

  // GET /showtimes/{showtimeId}/seats
  getSeats: (showtimeId: number) =>
    apiGet<ApiResponse<ShowtimeSeatsResponseDto>>(
      `${BASE}/${showtimeId}/seats`,
    ),

  // GET /showtimes/{showtimeId}/seat-suggestions?count=
  getSeatSuggestions: (showtimeId: number, count: number) =>
    apiGet<ApiResponse<SeatSuggestionDto[]>>(
      `${BASE}/${showtimeId}/seat-suggestions`,
      { params: { count } },
    ),
};
