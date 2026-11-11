import { useState } from "react";
import {
  useCounterCheckout,
  useAdminBookingDetail,
} from "@/features/admin/bookings/hooks/use-admin-booking";
import type {
  CounterHoldResponse,
  CounterCheckoutResponse,
} from "@/features/admin/bookings/types";

export function useCounterCheckoutFlow(holdResult: CounterHoldResponse | null) {
  const [checkoutResult, setCheckoutResult] =
    useState<CounterCheckoutResponse | null>(null);

  const bookingDetail = useAdminBookingDetail(holdResult?.bookingId ?? 0);
  const counterCheckout = useCounterCheckout();

  const confirmCheckout = async (
    amountReceived: number,
    onCheckedOut: () => void,
  ) => {
    if (!holdResult) return;
    await counterCheckout.mutateAsync(
      {
        bookingId: holdResult.bookingId,
        body: { amountReceived },
      },
      {
        onSuccess: (res) => {
          setCheckoutResult(res.data);
          onCheckedOut();
        },
      },
    );
  };

  const clearCheckout = () => setCheckoutResult(null);

  return {
    checkoutResult,
    bookingDetail,
    confirmCheckout,
    clearCheckout,
    isCheckingOut: counterCheckout.isPending,
  };
}
