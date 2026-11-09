import { keepPreviousData, useQuery } from "@tanstack/react-query";
import { showtimesApi } from "../api/showtimes-api";
import { queryKeys } from "@/api/query-keys";
import {
  SEAT_SUGGESTIONS_GC_TIME_MS,
  SEAT_SUGGESTIONS_STALE_TIME_MS,
} from "@/config/default-constants";

export function useSchedule(date: string, screenType?: string | null) {
  return useQuery({
    queryKey: queryKeys.showtimes.schedule(date, screenType),
    queryFn: () => showtimesApi.getSchedule(date, screenType),
    enabled: !!date,
    placeholderData: keepPreviousData,
    select: (res) => res.data,
  });
}

export function useShowtimeSeats(showtimeId: number) {
  return useQuery({
    queryKey: queryKeys.showtimes.seats(showtimeId),
    queryFn: () => showtimesApi.getSeats(showtimeId),
    enabled: showtimeId > 0,
    select: (res) => res.data,
  });
}

export function useSeatSuggestions(showtimeId: number, count: number | null) {
  return useQuery({
    queryKey: queryKeys.showtimes.seatSuggestions(showtimeId, count ?? 0),
    queryFn: () => showtimesApi.getSeatSuggestions(showtimeId, count!),
    enabled: showtimeId > 0 && count != null,
    staleTime: SEAT_SUGGESTIONS_STALE_TIME_MS,
    gcTime: SEAT_SUGGESTIONS_GC_TIME_MS,
    select: (res) => res.data,
  });
}
