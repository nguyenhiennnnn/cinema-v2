import { useEffect, useState } from "react";
import { Clock, HandCoins } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { formatCurrency } from "@/lib/formatters";
import { useCountdown } from "@/hooks/use-countdown";
import type { CounterHoldResponse } from "@/features/admin/bookings/types";

interface CounterCheckoutPanelProps {
  hold: CounterHoldResponse;
  onConfirm: (amountReceived: number) => void;
  isSubmitting?: boolean;
}

export const CounterCheckoutPanel = ({ hold, onConfirm, isSubmitting }: CounterCheckoutPanelProps) => {
  const [amountReceived, setAmountReceived] = useState(hold.finalAmount);
  const { remainingMs, label } = useCountdown(hold.expiresAt);

  useEffect(() => {
    setAmountReceived(hold.finalAmount);
  }, [hold.finalAmount]);

  const change = Math.max(0, amountReceived - hold.finalAmount);
  const isInsufficient = amountReceived < hold.finalAmount;
  const isExpired = remainingMs <= 0;

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
      {isInsufficient && (
        <p className="text-xs text-destructive">Số tiền khách đưa chưa đủ so với số tiền cần thanh toán.</p>
      )}
    </div>
  );
}
