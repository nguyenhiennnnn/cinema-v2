import { useNavigate, useParams } from "react-router";
import { ArrowLeft, CheckCircle2, XCircle } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { PageHeader } from "@/components/common/page-header";
import { formatCurrency, formatDate } from "@/lib/formatters";

import { useAdminBookingDetail } from "../hooks/use-admin-booking";
import { BookingStatusBadge } from "@/components/common/status-badge";

const AdminBookingDetailPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const bookingId = Number(id);

  const detail = useAdminBookingDetail(bookingId);

  if (detail.isLoading || !detail.data) {
    return (
      <div>
        <PageHeader title="Chi tiết đơn đặt vé" description="" />
        <Skeleton className="h-64 w-full " />
      </div>
    );
  }

  const b = detail.data;

  return (
    <div>
      <PageHeader
        title={`Đơn ${b.bookingCode}`}
        description={`Đặt ngày ${formatDate(b.createdAt)} qua kênh ${b.channel}.`}
        action={
          <Button variant="outline" onClick={() => navigate("/admin/bookings")}>
            <ArrowLeft className="size-4" />
            <span>Quay lại</span>
          </Button>
        }
      />

      <div className="grid gap-4 md:grid-cols-3">
        <div className="md:col-span-2 space-y-4">
          <div className="border border-border bg-card p-4">
            <h3 className="font-heading text-base font-medium text-foreground mb-3">Phim & Suất chiếu</h3>
            <div className="flex items-center gap-3">
              {b.movie.posterUrl && (
                <img
                  src={b.movie.posterUrl}
                  alt={b.movie.title}
                  className="h-20 w-14 object-cover"
                />
              )}
              <div>
                <p className="font-medium text-foreground">{b.movie.title}</p>
                <p className="text-sm text-muted-foreground">
                  {formatDate(b.showtime.startTime)} · Phòng {b.showtime.roomName}
                </p>
              </div>
            </div>
          </div>

          <div className="border border-border bg-card p-4">
            <h3 className="font-heading text-base font-medium text-foreground mb-3">Ghế đã chọn</h3>
            <div className="flex flex-wrap gap-2">
              {b.seats.map((s) => (
                <Badge
                  key={`${s.seatRow}${s.seatNumber}`}
                  variant="outline"
                  className="tabular-nums"
                >
                  {s.seatRow}{s.seatNumber} · {formatCurrency(s.price)}
                </Badge>
              ))}
            </div>
          </div>

          {b.combos.length > 0 && (
            <div className="border border-border bg-card p-4">
              <h3 className="font-heading text-base font-medium text-foreground mb-3">Combo bắp nước</h3>
              <div className="space-y-2">
                {b.combos.map((c, i) => (
                  <div key={`${c.name}-${i}`} className="flex items-center justify-between text-sm">
                    <span className="text-foreground">{c.name} × {c.quantity}</span>
                    <span className="tabular-nums text-muted-foreground">
                      {formatCurrency(c.unitPrice * c.quantity)}
                    </span>
                  </div>
                ))}
              </div>
            </div>
          )}

          {b.tickets && b.tickets.length > 0 && (
            <div className="border border-border bg-card p-4">
              <h3 className="font-heading text-base font-medium text-foreground mb-3">Vé</h3>
              <div className="space-y-2">
                {b.tickets.map((t) => (
                  <div key={t.ticketId} className="flex items-center justify-between text-sm">
                    <span className="tabular-nums text-foreground">
                      #{t.ticketId} · {t.seatRow}{t.seatNumber}
                    </span>
                    {t.isCheckedIn ? (
                      <span className="flex items-center gap-1 text-primary">
                        <CheckCircle2 className="size-3.5" />
                        {t.checkedInAt ? formatDate(t.checkedInAt) : "Đã check-in"}
                      </span>
                    ) : (
                      <span className="flex items-center gap-1 text-muted-foreground">
                        <XCircle className="size-3.5" />
                        Chưa check-in
                      </span>
                    )}
                  </div>
                ))}
              </div>
            </div>
          )}

          <div className="border border-border bg-card p-4">
            <h3 className="font-heading text-base font-medium text-foreground mb-3">Thanh toán</h3>
            <div className="space-y-2">
              {b.payments.map((p) => (
                <div key={p.paymentId} className="flex items-center justify-between text-sm">
                  <span className="text-foreground">{p.paymentMethod}</span>
                  <span className="tabular-nums font-medium text-foreground">
                    {formatCurrency(p.amount)}
                  </span>
                  <span className="text-muted-foreground">{p.status}</span>
                </div>
              ))}
              {b.payments.some((p) => p.refundedAmount != null) && (
                <div className="pt-2 border-t border-border/50 space-y-2">
                  {b.payments
                    .filter((p) => p.refundedAmount != null)
                    .map((p) => (
                      <div key={`refund-${p.paymentId}`} className="flex items-center justify-between text-sm">
                        <span className="text-muted-foreground">Hoàn tiền</span>
                        <span className="tabular-nums text-destructive font-medium">
                          -{formatCurrency(p.refundedAmount!)}
                        </span>
                        <span className="text-muted-foreground">
                          {p.refundedAt ? formatDate(p.refundedAt) : "—"}
                        </span>
                      </div>
                    ))}
                </div>
              )}
            </div>
          </div>
        </div>

        <div className="space-y-4">
          <div className="border border-border bg-card p-4">
            <div className="flex items-center justify-between mb-3">
              <span className="text-sm text-muted-foreground">Trạng thái</span>
              <BookingStatusBadge status={b.status} />
            </div>
            <div className="space-y-2 text-sm">
              <div className="flex justify-between border-b border-border/50 pb-2">
                <span className="text-muted-foreground">Tạm tính</span>
                <span className="tabular-nums text-foreground">{formatCurrency(b.totalAmount)}</span>
              </div>
              <div className="flex justify-between border-b border-border/50 pb-2">
                <span className="text-muted-foreground">Giảm giá</span>
                <span className="tabular-nums text-foreground">-{formatCurrency(b.discountAmount)}</span>
              </div>
              <div className="flex justify-between pt-1">
                <span className="font-medium text-foreground">Tổng cộng</span>
                <span className="font-semibold tabular-nums text-primary">
                  {formatCurrency(b.finalAmount)}
                </span>
              </div>
            </div>
          </div>

          {b.createdByEmployee && (
            <div className="border border-border bg-card p-4 text-sm">
              <span className="text-muted-foreground">Nhân viên tạo đơn</span>
              <p className="font-medium text-foreground mt-1">{b.createdByEmployee.fullName}</p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default AdminBookingDetailPage;