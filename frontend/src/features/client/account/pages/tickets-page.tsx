import { useState } from "react";
import { Ticket as TicketIcon } from "lucide-react";
import { Skeleton } from "@/components/ui/skeleton";
import { EmptyState } from "@/components/common/empty-state";
import { InfiniteScrollTrigger } from "@/components/common/infinite-scroll-trigger";
import { TicketCard } from "@/features/client/account/components/ticket-card";
import { useMyBookings, useCancelBooking } from "@/features/client/bookings/hooks/use-bookings";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";
import { bookingStatusOptions } from "@/lib/enum-options";
import { cn } from "@/lib/utils";

const TABS = [{ value: null as string | null, label: "Tất cả" }, ...bookingStatusOptions.map((o) => ({ value: o.value as string | null, label: o.label }))];

const AccountTicketsPage = () => {
  const [status, setStatus] = useState<string | null>(null);
  const { data, isLoading, hasNextPage, isFetchingNextPage, fetchNextPage } = useMyBookings({ status: status ?? undefined, limit: 10 });
  const cancelBooking = useCancelBooking();
  const openAlert = useAlertDialogOpen();

  const bookings = data?.bookings ?? [];

  const handleCancel = (bookingId: number) => {
    openAlert({
      title: "Hủy vé & hoàn tiền",
      description: "Bạn có chắc muốn hủy vé này? Số tiền sẽ được hoàn theo chính sách của rạp.",
      confirmText: "Hủy vé",
      variant: "destructive",
      onConfirm: () => cancelBooking.mutate(bookingId),
    });
  };

  return (
    <div>
      <h1 className="font-heading text-xl font-semibold text-foreground">Vé của tôi</h1>

      <div className="mt-4 flex flex-wrap gap-2">
        {TABS.map((tab) => (
          <button
            key={tab.label}
            type="button"
            onClick={() => setStatus(tab.value)}
            className={cn(
              "border px-3.5 py-1.5 text-sm font-medium transition-colors",
              status === tab.value
                ? "border-primary bg-primary text-primary-foreground shadow-sm"
                : "border-border bg-secondary text-secondary-foreground hover:border-primary/50 hover:bg-muted",
            )}
          >
            {tab.label}
          </button>
        ))}
      </div>

      <div className="mt-5 space-y-3">
        {isLoading ? (
          Array.from({ length: 3 }).map((_, i) => (
            <Skeleton key={i} className="h-24 w-full rounded-xl" />
          ))
        ) : bookings.length === 0 ? (
          <EmptyState icon={TicketIcon} title="Chưa có vé nào" description="Hãy đặt vé để bắt đầu trải nghiệm điện ảnh." />
        ) : (
          <>
            {bookings.map((booking) => (
              <TicketCard key={booking.bookingId} booking={booking} onCancel={() => handleCancel(booking.bookingId)} />
            ))}
            <InfiniteScrollTrigger hasNextPage={hasNextPage} isFetchingNextPage={isFetchingNextPage} fetchNextPage={fetchNextPage} />
          </>
        )}
      </div>
    </div>
  );
}

export default AccountTicketsPage;