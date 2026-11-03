import { Link, useParams, useSearchParams } from "react-router";
import { AlertTriangle } from "lucide-react";

import { useBookingFlow } from "../hooks/use-booking-flow";
import { BookingShowtimeHeader } from "../components/booking/booking-showtime-header";
import { BookingSelectionStep } from "../components/booking/booking-selection-step";
import { BookingPaymentStep } from "../components/booking/booking-payment-step";
import { Skeleton } from "@/components/ui/skeleton";
import { Button } from "@/components/ui/button";
import { ROUTES } from "@/config/routes";

const RESUME_INVALID_MESSAGE: Record<string, string> = {
  "not-pending":
    "Booking này không còn ở trạng thái chờ thanh toán (có thể đã được thanh toán hoặc đã huỷ).",
  expired: "Thời gian giữ ghế cho booking này đã hết hạn.",
  "load-error":
    "Không tải được thông tin booking. Booking có thể không tồn tại hoặc không thuộc về bạn.",
};

const BookingPage = () => {
  const { showtimeId } = useParams<{ showtimeId: string }>();
  const [searchParams] = useSearchParams();
  const resumeBookingId = Number(searchParams.get("resume") ?? 0) || undefined;

  const flow = useBookingFlow(Number(showtimeId), resumeBookingId);

  return (
    <div className="mx-auto flex w-full max-w-5xl flex-col gap-6 px-4 py-8">
      <BookingShowtimeHeader data={flow.showtimeSeats.data} isLoading={flow.showtimeSeats.isLoading} />

      {flow.isResumeLoading && (
        <div className="mx-auto w-full max-w-lg space-y-3">
          <Skeleton className="h-16 w-full " />
          <Skeleton className="h-40 w-full " />
        </div>
      )}

      {flow.isResumeMode && !flow.isResumeLoading && flow.resumeInvalidReason && (
        <div className="mx-auto flex w-full max-w-lg flex-col items-center gap-3 border border-border bg-card p-6 text-center">
          <AlertTriangle className="size-8 text-warning" />
          <p className="text-sm text-foreground">
            {RESUME_INVALID_MESSAGE[flow.resumeInvalidReason]}
          </p>
          <div className="flex gap-2">
            <Button variant="outline" nativeButton={false} render={<Link to={ROUTES.account.tickets} />}>
              Xem vé của tôi
            </Button>
            <Button nativeButton={false} render={<Link to={`/booking/${showtimeId}`} />}>
              Đặt vé mới
            </Button>
          </div>
        </div>
      )}

      {flow.step === "select" && !flow.isResumeMode && (
        <BookingSelectionStep
          isLoading={flow.showtimeSeats.isLoading}
          data={flow.showtimeSeats.data}
          statusMap={flow.seatStatusMap}
          selectedIds={flow.selectedSeatIds}
          onToggleSeat={flow.toggleSeat}
          onSelectSeats={flow.selectSeats}
          comboQuantities={flow.comboQuantities}
          onComboChange={flow.setComboQuantity}
          customerName={flow.customerName}
          onCustomerNameChange={flow.setCustomerName}
          customerPhone={flow.customerPhone}
          onCustomerPhoneChange={flow.setCustomerPhone}
          isAuthenticated={flow.isAuthenticated}
          canHold={flow.canHold}
          loneSeats={flow.loneSeats}
          isHolding={flow.isHolding}
          onContinue={flow.holdAndContinue}
        />
      )}

      {flow.step === "payment" && flow.holdResult && !flow.resumeInvalidReason && (
        <>
          {flow.isResumeMode && (
            <p className="mx-auto w-full max-w-lg text-center text-xs text-muted-foreground">
              Đang tiếp tục thanh toán cho booking{" "}
              <span className="tabular-nums font-medium text-foreground">
                {flow.holdResult.bookingCode}
              </span>
            </p>
          )}
          <BookingPaymentStep
            seats={flow.selectedSeatsInfo}
            hold={flow.holdResult}
            paymentMethod={flow.paymentMethod}
            onPaymentMethodChange={flow.setPaymentMethod}
            isSubmitting={flow.isSubmittingPayment}
            onSubmit={flow.submitPayment}
            onCancel={flow.cancelHold}
            isCancelling={flow.isCancelling}
          />
        </>
      )}
    </div>
  );
};

export default BookingPage;