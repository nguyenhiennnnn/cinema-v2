import { apiGet } from "@/api/axios";
import type { ApiResponse } from "@/types/api";
import type {
  WholeCinemaScheduleDto,
  ShowtimeSeatsResponseDto,
  SeatSuggestionDto,
} from "../types";
import { ENDPOINTS } from "@/api/endpoints";

export const showtimesApi = {
  getSchedule: (date: string, screenType?: string | null) =>
    apiGet<ApiResponse<WholeCinemaScheduleDto>>(ENDPOINTS.showtimes.root, {
      params: { date, screenType: screenType || undefined },
    }),

  getSeats: (showtimeId: number) =>
    apiGet<ApiResponse<ShowtimeSeatsResponseDto>>(
      ENDPOINTS.showtimes.seats(showtimeId),
    ),

  getSeatSuggestions: (showtimeId: number, count: number) =>
    apiGet<ApiResponse<SeatSuggestionDto[]>>(
      ENDPOINTS.showtimes.seatSuggestions(showtimeId),
      { params: { count } },
    ),
};
