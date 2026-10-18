import { Skeleton } from "@/components/ui/skeleton";
import { formatCurrency, formatDate } from "@/lib/formatters";

import { useAdminPaymentDetail } from "../hooks/use-admin-payments";
import { PaymentStatusBadge } from "@/components/common/status-badge";
import type { PaymentStatus } from "@/types/enum";

interface PaymentDetailViewProps {
  paymentId: number;
}

export const PaymentDetailView = ({ paymentId }: PaymentDetailViewProps) => {
  const detail = useAdminPaymentDetail(paymentId);

  if (detail.isLoading || !detail.data) {
    return <Skeleton className="h-48 w-full rounded-xl" />;
  }

  const p = detail.data;

  const rows: Array<[string, string]> = [
    ["Mã giao dịch", `#${p.paymentId}`],
    ["Đơn đặt vé", p.booking.bookingCode],
    ["Phương thức", p.paymentMethod],
    ["Mã giao dịch cổng", p.transactionId ?? "—"],
    ["Mã phản hồi cổng", p.gatewayResponseCode ?? "—"],
    ["Số tiền", formatCurrency(p.amount)],
    ["Ngày thanh toán", p.paidAt ? formatDate(p.paidAt) : "—"],
    ["Ngày tạo", formatDate(p.createdAt)],
  ];

  if (p.amountReceived != null) rows.push(["Tiền khách đưa", formatCurrency(p.amountReceived)]);
  if (p.changeGiven != null) rows.push(["Tiền thối lại", formatCurrency(p.changeGiven)]);
  if (p.refundedAmount != null) rows.push(["Số tiền hoàn", formatCurrency(p.refundedAmount)]);
  if (p.refundTransactionId) rows.push(["Mã giao dịch hoàn", p.refundTransactionId]);
  if (p.refundedAt) rows.push(["Ngày hoàn tiền", formatDate(p.refundedAt)]);

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between rounded-lg border border-border bg-muted/30 p-3">
        <span className="text-sm text-muted-foreground">Trạng thái</span>
        <PaymentStatusBadge status={p.status as PaymentStatus} />
      </div>

      <div className="space-y-2">
        {rows.map(([label, value]) => (
          <div key={label} className="flex items-center justify-between border-b border-border/50 pb-2 text-sm">
            <span className="text-muted-foreground">{label}</span>
            <span className="font-medium text-foreground">{value}</span>
          </div>
        ))}
      </div>

      <div className="flex items-center justify-between rounded-lg border border-border bg-muted/30 p-3 text-sm">
        <span className="text-muted-foreground">Tổng đơn đặt vé</span>
        <span className="font-semibold tabular-nums text-primary">
          {formatCurrency(p.booking.finalAmount)}
        </span>
      </div>
    </div>
  );
};