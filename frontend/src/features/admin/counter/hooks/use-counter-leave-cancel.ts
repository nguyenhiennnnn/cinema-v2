import { useCallback, useEffect, useRef } from "react";
import { bookingsAdminApi } from "@/features/admin/bookings/api/bookings-admin-api";

interface UseCounterLeaveCancelParams {
  /** Gọi khi trang được khôi phục từ bfcache sau khi đơn đã bị hủy lúc rời trang. */
  onRestoredAfterCancel: () => void;
}

/**
 * Hủy đơn quầy đang Pending khi nhân viên rời luồng mà chưa thanh toán:
 * - Chuyển trang trong admin (unmount): gọi API qua apiClient, lỗi được nuốt.
 * - Đóng tab, tải lại, đóng trình duyệt (pagehide): fetch keepalive kèm token và header CSRF.
 * Server chỉ hủy đơn còn Pending, đơn đã thanh toán sẽ bị từ chối và không bị đụng tới.
 * Mất mạng hoặc trình duyệt crash: không gửi được, đơn tự hết hạn theo thời hạn giữ.
 */
export function useCounterLeaveCancel({
  onRestoredAfterCancel,
}: UseCounterLeaveCancelParams) {
  const cancellableIdRef = useRef<number | null>(null);
  const mountedRef = useRef(false);
  const cancelledOnHideRef = useRef(false);
  const onRestoredRef = useRef(onRestoredAfterCancel);

  useEffect(() => {
    onRestoredRef.current = onRestoredAfterCancel;
  }, [onRestoredAfterCancel]);

  const setCancellableBookingId = useCallback((id: number | null) => {
    cancellableIdRef.current = id;
  }, []);

  const cancelQuietly = useCallback(async (bookingId: number) => {
    try {
      await bookingsAdminApi.counterCancel(bookingId);
    } catch {
      // Đã hủy, đã hết hạn hoặc đã thanh toán: không báo lỗi cho nhân viên.
    }
  }, []);

  const hasLeft = useCallback(() => !mountedRef.current, []);

  useEffect(() => {
    mountedRef.current = true;

    const onPageHide = () => {
      const id = cancellableIdRef.current;
      if (id === null) return;
      cancellableIdRef.current = null;
      cancelledOnHideRef.current = true;
      bookingsAdminApi.counterCancelKeepalive(id);
    };
    const onPageShow = (event: PageTransitionEvent) => {
      if (!event.persisted || !cancelledOnHideRef.current) return;
      cancelledOnHideRef.current = false;
      onRestoredRef.current();
    };
    window.addEventListener("pagehide", onPageHide);
    window.addEventListener("pageshow", onPageShow);

    return () => {
      window.removeEventListener("pagehide", onPageHide);
      window.removeEventListener("pageshow", onPageShow);
      mountedRef.current = false;
      // Hoãn một nhịp để effect chạy hai lần (StrictMode, Fast Refresh) không hủy nhầm:
      // lần mount lại sẽ đặt mountedRef về true trước khi timer chạy.
      window.setTimeout(() => {
        if (mountedRef.current) return;
        const id = cancellableIdRef.current;
        if (id === null) return;
        cancellableIdRef.current = null;
        void cancelQuietly(id);
      }, 0);
    };
  }, [cancelQuietly]);

  return { setCancellableBookingId, cancelQuietly, hasLeft };
}

export type CounterLeaveCancel = ReturnType<typeof useCounterLeaveCancel>;
