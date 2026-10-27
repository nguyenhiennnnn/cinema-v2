import { useQuery } from "@tanstack/react-query";
import { showtimesApi } from "../api/showtimes-api";
import { queryKeys } from "@/api/query-keys";

export function useSchedule(date: string) {
  return useQuery({
    queryKey: queryKeys.showtimes.schedule(date),
    queryFn: () => showtimesApi.getSchedule(date),
    enabled: !!date,
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
    staleTime: 0,
    gcTime: 0,
    select: (res) => res.data,
  });
}
