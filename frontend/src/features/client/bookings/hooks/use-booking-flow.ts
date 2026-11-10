import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useLocation, useNavigate } from "react-router";
import { toast } from "@/components/ui/toast";
import { BOOKING_HOLD_POLL_INTERVAL_MS } from "@/config/default-constants";
import {
  ShowtimeSeatStatus,
  PaymentMethod,
  BookingStatus,
  SeatGapRuleMode,
} from "@/types/enum";
import { findLoneSeats } from "@/components/common/seat-map/seat-map-utils";
import { useCinemaSettings } from "@/features/client/settings/hooks/use-settings";
import { useShowtimeSeats } from "@/features/client/showtimes/hooks/use-showtimes";
import { useRealtimeSeatMap } from "@/hooks/use-realtime-seat-map";
import { isValidVnPhone } from "@/lib/validators";
import { useAuthUser, useIsAuthenticated } from "@/stores/auth-store";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";
import {
  useBookingDetail,
  useCancelBooking,
  useCheckout,
  useHoldSeats,
} from "./use-bookings";
import { bookingsApi } from "../api/bookings-api";
import type { HoldBookingResponse, PricedSeat } from "../types";
import { ROUTES } from "@/config/routes";

export type BookingStep = "select" | "payment";

export function useBookingFlow(showtimeId: number, resumeBookingId?: number) {
  const isResumeMode = !!resumeBookingId && resumeBookingId > 0;

  const [step, setStep] = useState<BookingStep>(
    isResumeMode ? "payment" : "select",
  );
  const [selectedSeatIds, setSelectedSeatIds] = useState<Set<number>>(
    new Set(),
  );
  const [comboQuantities, setComboQuantities] = useState<
    Record<number, number>
  >({});
  const [paymentMethod, setPaymentMethod] = useState<PaymentMethod>(
    PaymentMethod.VNPay,
  );
  const [freshHoldResult, setFreshHoldResult] =
    useState<HoldBookingResponse | null>(null);

  const authUser = useAuthUser();
  const isAuthenticated = useIsAuthenticated();
  const navigate = useNavigate();
  const location = useLocation();
  const openAlertDialog = useAlertDialogOpen();
  const [customerName, setCustomerName] = useState(authUser?.fullName ?? "");
  const [customerPhone, setCustomerPhone] = useState("");

  const promptLogin = (description: string) => {
    const redirectTo = encodeURIComponent(
      `${location.pathname}${location.search}`,
    );
    openAlertDialog({
      title: "Yêu cầu đăng nhập",
      description,
      confirmText: "Đăng nhập ngay",
      cancelText: "Để sau",
      onConfirm: () => {
        navigate(`${ROUTES.auth.login}?redirect=${redirectTo}`);
      },
    });
  };

  useEffect(() => {
    if (authUser?.fullName && !customerName) {
      setCustomerName(authUser.fullName);
    }
  }, [authUser?.fullName, customerName]);

  const showtimeSeats = useShowtimeSeats(showtimeId);

  const resumeBookingQuery = useBookingDetail(
    isResumeMode ? resumeBookingId! : 0,
  );

  const resumeInvalidReason = useMemo<
    "not-pending" | "expired" | "load-error" | null
  >(() => {
    if (!isResumeMode) return null;
    if (resumeBookingQuery.isError) return "load-error";
    const booking = resumeBookingQuery.data;
    if (!booking) return null;
    if (booking.status !== BookingStatus.Pending || !booking.holdExpiresAt)
      return "not-pending";
    if (new Date(booking.holdExpiresAt).getTime() <= Date.now())
      return "expired";
    return null;
  }, [isResumeMode, resumeBookingQuery.isError, resumeBookingQuery.data]);

  const holdResult: HoldBookingResponse | null = useMemo(() => {
    if (!isResumeMode) return freshHoldResult;
    const booking = resumeBookingQuery.data;
    if (!booking || !booking.holdExpiresAt || resumeInvalidReason) return null;
    return {
      bookingId: booking.bookingId,
      bookingCode: booking.bookingCode,
      status: booking.status,
      totalAmount: booking.totalAmount,
      discountAmount: booking.discountAmount,
      finalAmount: booking.finalAmount,
      expiresAt: booking.holdExpiresAt,
    };
  }, [
    isResumeMode,
    freshHoldResult,
    resumeBookingQuery.data,
    resumeInvalidReason,
  ]);

  const resumedSeats: PricedSeat[] = resumeBookingQuery.data?.seats ?? [];

  const seatStatusMap = useRealtimeSeatMap({
    showtimeId,
    initialSeats: showtimeSeats.data?.seats,
    ownBookingId: holdResult?.bookingId ?? null,
    onHoldExpired: () => {
      if (expiredNotifiedRef.current) return;
      expiredNotifiedRef.current = true;
      stopPolling();
      toast.add({
        title: "Thời gian giữ ghế đã hết hạn, vui lòng chọn lại.",
        type: "warning",
      });
      if (!isResumeMode) resetToSelection();
    },
  });

  const holdSeatsMutation = useHoldSeats();
  const checkoutMutation = useCheckout(holdResult?.bookingId ?? 0);
  const cancelBookingMutation = useCancelBooking();

  const selectedSeatsInfo = useMemo(
    () =>
      (showtimeSeats.data?.seats ?? []).filter((s) =>
        selectedSeatIds.has(s.showtimeSeatId),
      ),
    [showtimeSeats.data, selectedSeatIds],
  );

  const cinemaSettings = useCinemaSettings();
  const loneSeats = useMemo(
    () =>
      findLoneSeats(
        showtimeSeats.data?.seats ?? [],
        seatStatusMap,
        selectedSeatIds,
        cinemaSettings.data?.seatGapRuleMode ?? SeatGapRuleMode.Strict,
      ),
    [
      showtimeSeats.data,
      seatStatusMap,
      selectedSeatIds,
      cinemaSettings.data?.seatGapRuleMode,
    ],
  );

  const combosPayload = useMemo(
    () =>
      Object.entries(comboQuantities)
        .filter(([, qty]) => qty > 0)
        .map(([comboId, quantity]) => ({ comboId: Number(comboId), quantity })),
    [comboQuantities],
  );

  const toggleSeat = (showtimeSeatId: number) => {
    const allSeats = showtimeSeats.data?.seats ?? [];
    const coupleId = allSeats.find(
      (s) => s.showtimeSeatId === showtimeSeatId,
    )?.coupleId;
    const group = coupleId
      ? allSeats
          .filter((s) => s.coupleId === coupleId)
          .map((s) => s.showtimeSeatId)
      : [showtimeSeatId];
    if (
      group.some((id) => {
        const status = seatStatusMap.get(id);
        return status && status !== ShowtimeSeatStatus.Available;
      })
    )
      return;

    setSelectedSeatIds((prev) => {
      const next = new Set(prev);
      const remove = next.has(showtimeSeatId);
      group.forEach((id) => (remove ? next.delete(id) : next.add(id)));
      return next;
    });
  };

  const selectSeats = (ids: number[]) => setSelectedSeatIds(new Set(ids));

  const setComboQuantity = (comboId: number, quantity: number) => {
    setComboQuantities((prev) => ({ ...prev, [comboId]: quantity }));
  };

  const effectiveCustomerName = customerName.trim() || (authUser?.fullName ?? '').trim();

  const canHold =
    selectedSeatIds.size > 0 &&
    loneSeats.length === 0 &&
    (!isAuthenticated || effectiveCustomerName.length > 0) &&
    (customerPhone.trim() === "" || isValidVnPhone(customerPhone.trim()));

  const holdAndContinue = async () => {
    if (!isAuthenticated) {
      promptLogin(
        "Bạn cần đăng nhập tài khoản để thực hiện đặt vé và giữ ghế. Bạn có muốn đăng nhập ngay không?",
      );
      return;
    }

    await holdSeatsMutation.mutateAsync(
      {
        showtimeId,
        seatIds: Array.from(selectedSeatIds),
        combos: combosPayload.length > 0 ? combosPayload : undefined,
        customerName: customerName.trim(),
        customerPhone: customerPhone.trim(),
      },
      {
        onSuccess: (res) => {
          setFreshHoldResult(res.data);
          setStep("payment");
        },
      },
    );
  };

  const resetToSelection = () => {
    setStep("select");
    setSelectedSeatIds(new Set());
    setFreshHoldResult(null);
  };

  const expiredNotifiedRef = useRef(false);
  const pollIntervalRef = useRef<ReturnType<typeof setInterval> | null>(null);

  const stopPolling = useCallback(() => {
    if (pollIntervalRef.current !== null) {
      clearInterval(pollIntervalRef.current);
      pollIntervalRef.current = null;
    }
  }, []);

  useEffect(() => {
    const bookingId = freshHoldResult?.bookingId;

    if (!bookingId || step !== "payment") {
      stopPolling();
      expiredNotifiedRef.current = false;
      return;
    }

    if (pollIntervalRef.current !== null) return;

    const poll = async () => {
      if (expiredNotifiedRef.current) {
        stopPolling();
        return;
      }
      try {
        const res = await bookingsApi.getBookingDetail(bookingId);
        if (res.data.status === BookingStatus.Expired) {
          expiredNotifiedRef.current = true;
          stopPolling();
          toast.add({
            title: "Thời gian giữ ghế đã hết hạn, vui lòng chọn lại.",
            type: "warning",
          });
          if (!isResumeMode) resetToSelection();
        }
      } catch {
        // ignore transient errors, keep polling
      }
    };

    poll();
    pollIntervalRef.current = setInterval(poll, BOOKING_HOLD_POLL_INTERVAL_MS);

    return stopPolling;
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [freshHoldResult?.bookingId, step]);

  const cancelHold = () => {
    if (!holdResult) return;
    cancelBookingMutation.mutate(holdResult.bookingId, {
      onSuccess: () => {
        if (!isResumeMode) resetToSelection();
      },
    });
  };

  const submitPayment = () => {
    if (!holdResult) return;

    if (!isAuthenticated) {
      promptLogin(
        "Bạn cần đăng nhập tài khoản để thực hiện thanh toán. Bạn có muốn đăng nhập ngay không?",
      );
      return;
    }

    checkoutMutation.mutate(
      { paymentMethod },
      {
        onSuccess: (res) => {
          window.location.href = res.data.paymentUrl;
        },
      },
    );
  };

  return {
    step,
    isResumeMode,
    isResumeLoading: isResumeMode && resumeBookingQuery.isLoading,
    resumeInvalidReason,
    showtimeSeats,
    seatStatusMap,
    selectedSeatIds,
    selectedSeatsInfo: isResumeMode ? resumedSeats : selectedSeatsInfo,
    toggleSeat,
    selectSeats,
    loneSeats,
    comboQuantities,
    setComboQuantity,
    isAuthenticated,
    customerName,
    setCustomerName,
    customerPhone,
    setCustomerPhone,
    paymentMethod,
    setPaymentMethod,
    canHold,
    holdAndContinue,
    isHolding: holdSeatsMutation.isPending,
    holdResult,
    cancelHold,
    isCancelling: cancelBookingMutation.isPending,
    submitPayment,
    isSubmittingPayment: checkoutMutation.isPending,
  };
}
