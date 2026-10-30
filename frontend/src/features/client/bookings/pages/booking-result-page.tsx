import { useEffect, useRef, useState } from "react";
import { Link, useSearchParams } from "react-router";
import { RefreshCw } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { Confetti } from "@/components/common/confetti";
import { BookingStatus } from "@/types/enum";
import { BOOKING_RESULT_POLL_INTERVAL_MS } from "@/config/default-constants";

import { useBookingDetail } from "../hooks/use-bookings";
import { BookingResultStatus } from "../components/result/booking-result-status";
import { BookingResultTickets } from "../components/result/booking-result-tickets";
import { ROUTES } from "@/config/routes";

const BookingResultPage = () => {
  const [searchParams] = useSearchParams();
  const bookingId = Number(searchParams.get("bookingId"));

  const bookingDetail = useBookingDetail(bookingId);
  const status = bookingDetail.data?.status;
  const isPending = status === BookingStatus.Pending;

  const [celebrate, setCelebrate] = useState(false);
  const hasCelebrated = useRef(false);

  useEffect(() => {
    if (!isPending) return;
    const interval = setInterval(
      () => bookingDetail.refetch(),
      BOOKING_RESULT_POLL_INTERVAL_MS,
    );
    return () => clearInterval(interval);
  }, [isPending, bookingDetail]);

  useEffect(() => {
    if (status !== BookingStatus.Confirmed || hasCelebrated.current) return;
    hasCelebrated.current = true;
    setCelebrate(true);
  }, [status]);

  return (
    <div className="mx-auto flex min-h-[60vh] w-full max-w-lg flex-col items-center justify-center gap-6 px-4 py-12">
      <Confetti active={celebrate} />
      {!bookingId ? (
        <p className="text-sm text-muted-foreground">Thiếu thông tin đơn đặt vé.</p>
      ) : bookingDetail.isLoading ? (
        <Skeleton className="h-64 w-full " />
      ) : !bookingDetail.data ? (
        <div className="flex flex-col items-center gap-3">
          <p className="text-sm text-muted-foreground">Không tìm thấy đơn đặt vé này.</p>
          <Button variant="outline" onClick={() => bookingDetail.refetch()}>
            <RefreshCw className="size-4" />
            <span>Thử lại</span>
          </Button>
        </div>
      ) : (
        <>
          <BookingResultStatus status={bookingDetail.data.status} bookingCode={bookingDetail.data.bookingCode} />
          {bookingDetail.data.status === BookingStatus.Confirmed && (
            <BookingResultTickets booking={bookingDetail.data} />
          )}
          <div className="flex gap-3">
            <Button>
              <Link to={ROUTES.account.tickets}>Xem vé của tôi</Link>
            </Button>
            <Button variant="outline">
              <Link to="/">Về trang chủ</Link>
            </Button>
          </div>
        </>
      )}
    </div>
  );
};

export default BookingResultPage;
