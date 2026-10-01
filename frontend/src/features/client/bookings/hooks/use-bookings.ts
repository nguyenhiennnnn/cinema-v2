import { useInfiniteQuery, useMutation, useQuery } from "@tanstack/react-query";
import { bookingsApi } from "../api/bookings-api";
import { queryKeys } from "@/api/query-keys";
import { queryClient } from "@/api/query-client";
import type {
  HoldSeatsRequest,
  CheckoutRequest,
  BookingListParams,
} from "../types";

export function useMyBookings(params: Omit<BookingListParams, "cursor">) {
  return useInfiniteQuery({
    queryKey: queryKeys.bookings.myList(params),
    queryFn: ({ pageParam }) =>
      bookingsApi.getMyBookings({ ...params, cursor: pageParam }),
    initialPageParam: undefined as string | undefined,
    getNextPageParam: (lastPage) =>
      lastPage.data.hasMore
        ? (lastPage.data.nextCursor ?? undefined)
        : undefined,
    select: (data) => ({
      bookings: data.pages.flatMap((p) => p.data.items),
      hasMore: data.pages.at(-1)?.data.hasMore ?? false,
    }),
  });
}

export function useBookingDetail(bookingId: number) {
  return useQuery({
    queryKey: queryKeys.bookings.detail(bookingId),
    queryFn: () => bookingsApi.getBookingDetail(bookingId),
    enabled: bookingId > 0,
    select: (res) => res.data,
  });
}

export function useHoldSeats() {
  return useMutation({
    mutationFn: (body: HoldSeatsRequest) => bookingsApi.holdSeats(body),
  });
}

export function useCheckout(bookingId: number) {
  return useMutation({
    mutationFn: (body: CheckoutRequest) =>
      bookingsApi.checkout(bookingId, body),
  });
}

export function useCancelBooking() {
  return useMutation({
    mutationFn: (bookingId: number) => bookingsApi.cancelBooking(bookingId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.bookings.all() });
    },
  });
}
