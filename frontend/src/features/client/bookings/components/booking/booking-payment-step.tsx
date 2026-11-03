import { Button } from "@/components/ui/button";
import { PaymentMethod } from "@/types/enum";
import type { HoldBookingResponse, PricedSeat } from "../../types";
import { BookingHoldTimer } from "./booking-hold-timer";
import { BookingPaymentMethod } from "./booking-payment-method";
import { BookingPriceSummary } from "./booking-price-summary";

interface BookingPaymentStepProps {
  seats: PricedSeat[];
  hold: HoldBookingResponse;
  paymentMethod: PaymentMethod;
  onPaymentMethodChange: (value: PaymentMethod) => void;
  isSubmitting: boolean;
  onSubmit: () => void;
  onCancel: () => void;
  isCancelling: boolean;
}

export const BookingPaymentStep = ({
  seats,
  hold,
  paymentMethod,
  onPaymentMethodChange,
  isSubmitting,
  onSubmit,
  onCancel,
  isCancelling,
}: BookingPaymentStepProps) => {
  return (
    <div className="mx-auto flex w-full max-w-lg flex-col gap-4">
      <BookingHoldTimer
        expiresAt={hold.expiresAt}
        className="sticky top-16 z-40 justify-center border border-border bg-background px-4 py-2.5"
      />

      <BookingPriceSummary seats={seats} combos={undefined} comboQuantities={{}} hold={hold} />

      <BookingPaymentMethod value={paymentMethod} onChange={onPaymentMethodChange} />

      <Button disabled={isSubmitting} onClick={onSubmit}>
        {isSubmitting ? "Đang chuyển đến cổng thanh toán..." : "Thanh toán"}
      </Button>
      <Button variant="outline" disabled={isCancelling} onClick={onCancel}>
        {isCancelling ? "Đang huỷ..." : "Huỷ, chọn lại ghế"}
      </Button>
    </div>
  );
}