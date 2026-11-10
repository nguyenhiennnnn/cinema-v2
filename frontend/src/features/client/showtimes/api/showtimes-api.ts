import { apiGet } from "@/api/axios";
import type { ApiResponse } from "@/types/api";
import type {
  WholeCinemaScheduleDto,
  ShowtimeSeatsResponseDto,
  SeatSuggestionDto,
} from "../types";
import { ENDPOINTS } from "@/api/endpoints";

export const showtimesApi = {
  // GET /showtimes?date=dd/MM/yyyy — date is required, pass as "dd/MM/yyyy"
  getSchedule: (date: string, screenType?: string | null) =>
    apiGet<ApiResponse<WholeCinemaScheduleDto>>(ENDPOINTS.showtimes.root, {
      params: { date, screenType: screenType || undefined },
    }),

  // GET /showtimes/{showtimeId}/seats
  getSeats: (showtimeId: number) =>
    apiGet<ApiResponse<ShowtimeSeatsResponseDto>>(
      ENDPOINTS.showtimes.seats(showtimeId),
    ),

  // GET /showtimes/{showtimeId}/seat-suggestions?count=
  getSeatSuggestions: (showtimeId: number, count: number) =>
    apiGet<ApiResponse<SeatSuggestionDto[]>>(
      ENDPOINTS.showtimes.seatSuggestions(showtimeId),
      { params: { count } },
    ),
};
