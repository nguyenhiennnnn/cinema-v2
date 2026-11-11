import { useEffect, useState } from "react";
import { Armchair, Clock, HandCoins, X } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { formatCurrency } from "@/lib/formatters";
import { useCountdown } from "@/hooks/use-countdown";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";
import type { CounterHoldResponse } from "@/features/admin/bookings/types";

interface CounterCheckoutPanelProps {
  hold: CounterHoldResponse;
  onConfirm: (amountReceived: number) => void;
  onChangeSeats: () => void | Promise<void>;
  onCancelOrder: () => void | Promise<void>;
  isSubmitting?: boolean;
}

export const CounterCheckoutPanel = ({
  hold,
  onConfirm,
  onChangeSeats,
  onCancelOrder,
  isSubmitting,
}: CounterCheckoutPanelProps) => {
  const openAlertDialog = useAlertDialogOpen();
  const [amountReceived, setAmountReceived] = useState(hold.finalAmount);
  const { remainingMs, label } = useCountdown(hold.expiresAt);

  useEffect(() => {
    setAmountReceived(hold.finalAmount);
  }, [hold.finalAmount]);

  const change = Math.max(0, amountReceived - hold.finalAmount);
  const isInsufficient = amountReceived < hold.finalAmount;
  const isExpired = remainingMs <= 0;

  const handleCancelOrder = () => {
    openAlertDialog({
      title: `Huỷ đơn ${hold.bookingCode}?`,
      description: "Ghế đang giữ sẽ được nhả ngay và đơn này sẽ bị huỷ.",
      confirmText: "Huỷ đơn",
      cancelText: "Giữ lại",
      variant: "destructive",
      onConfirm: onCancelOrder,
    });
  };

  return (
    <div className="flex flex-col gap-4 bg-card p-4">
      <div className="flex items-center justify-between">
        <p className="t-code text-sm text-foreground">
          {hold.bookingCode}
        </p>
        <div className="flex items-center gap-1.5 text-xs font-medium text-warning">
          <Clock className="size-3.5" />
          <span>{isExpired ? "Đã hết hạn giữ ghế" : `Còn ${label}`}</span>
        </div>
      </div>

      <div className="flex items-center justify-between text-sm">
        <span className="text-muted-foreground">Số tiền cần thanh toán</span>
        <span className="text-lg font-bold tabular-nums text-primary">
          {formatCurrency(hold.finalAmount)}
        </span>
      </div>

      <div className="flex flex-col gap-1.5">
        <Label htmlFor="amount-received">Số tiền khách đưa (tiền mặt / chuyển khoản)</Label>
        <Input
          id="amount-received"
          type="number"
          min={0}
          value={amountReceived}
          onChange={(e) => setAmountReceived(Number(e.target.value) || 0)}
        />
      </div>

      <div className="flex items-center justify-between text-sm">
        <span className="text-muted-foreground">Tiền thừa trả khách</span>
        <span className="font-semibold tabular-nums text-foreground">{formatCurrency(change)}</span>
      </div>

      <Button
        onClick={() => onConfirm(amountReceived)}
        disabled={isSubmitting || isInsufficient || isExpired}
      >
        <HandCoins className="size-4" />
        <span>{isSubmitting ? "Đang xử lý..." : "Xác nhận thanh toán"}</span>
      </Button>
      <div className="grid grid-cols-2 gap-2">
        <Button variant="outline" onClick={onChangeSeats} disabled={isSubmitting}>
          <Armchair className="size-4" />
          <span>Đổi ghế</span>
        </Button>
        <Button variant="outline" onClick={handleCancelOrder} disabled={isSubmitting}>
          <X className="size-4" />
          <span>Huỷ đơn</span>
        </Button>
      </div>
      {isInsufficient && (
        <p className="text-xs text-destructive">Số tiền khách đưa chưa đủ so với số tiền cần thanh toán.</p>
      )}
    </div>
  );
}
