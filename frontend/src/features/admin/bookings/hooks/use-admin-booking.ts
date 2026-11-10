import { useMutation, useQuery } from "@tanstack/react-query";
import { queryKeys } from "@/api/query-keys";
import type {
  AdminBookingListParams,
  CounterHoldRequest,
  CounterCheckoutRequest,
  CheckinRequest,
} from "@/features/admin/bookings/types";
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
    meta: { invalidates: "bookingChanged" },
    mutationFn: (body: CounterHoldRequest) => bookingsAdminApi.counterHold(body),
  });
}

export function useCounterCheckout() {
  return useMutation({
    meta: { invalidates: "bookingChanged" },
    mutationFn: ({ bookingId, body }: { bookingId: number; body: CounterCheckoutRequest }) =>
      bookingsAdminApi.counterCheckout(bookingId, body),
  });
}

export function useCheckin() {
  return useMutation({
    meta: { invalidates: "bookingChanged" },
    mutationFn: (body: CheckinRequest) => bookingsAdminApi.checkin(body),
  });
}