import { useMutation, useQuery } from "@tanstack/react-query";
import { queryKeys } from "@/api/query-keys";
import { queryClient } from "@/api/query-client";
import type {
  AdminBookingListParams,
  CounterHoldRequest,
  CounterCheckoutRequest,
  CheckinRequest,
} from "../types";
import { bookingsAdminApi } from "../api/bookings-admin-api";

export function useAdminBookings(params: AdminBookingListParams) {
  return useQuery({
    queryKey: queryKeys.adminBookings.list(params),
    queryFn: () => bookingsAdminApi.getBookings(params),
    select: (res) => res.data,
  });
}

export function useAdminBookingDetail(bookingId: number) {
  return useQuery({
    queryKey: queryKeys.adminBookings.detail(bookingId),
    queryFn: () => bookingsAdminApi.getBookingDetail(bookingId),
    enabled: bookingId > 0,
    select: (res) => res.data,
  });
}

export function useCounterHold() {
  return useMutation({
    mutationFn: (body: CounterHoldRequest) => bookingsAdminApi.counterHold(body),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: queryKeys.adminBookings.all() }),
  });
}

export function useCounterCheckout() {
  return useMutation({
    mutationFn: ({ bookingId, body }: { bookingId: number; body: CounterCheckoutRequest }) =>
      bookingsAdminApi.counterCheckout(bookingId, body),
    onSuccess: (_, { bookingId }) => {
      queryClient.invalidateQueries({ queryKey: queryKeys.adminBookings.all() });
      queryClient.invalidateQueries({ queryKey: queryKeys.adminBookings.detail(bookingId) });
    },
  });
}

export function useCheckin() {
  return useMutation({
    mutationFn: (body: CheckinRequest) => bookingsAdminApi.checkin(body),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: queryKeys.adminBookings.all() }),
  });
}