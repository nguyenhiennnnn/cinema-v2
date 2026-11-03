import { useMemo, useCallback } from "react";
import { useNavigate } from "react-router";
import { cn } from "@/lib/utils";
import { formatShowtimeRange } from "@/lib/cinema-format";
import { formatCurrency } from "@/lib/formatters";
import { useAuthStore, useIsAuthenticated } from "@/stores/auth-store";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";

export interface ShowtimeSlotItem {
  showtimeId: number;
  roomName: string;
  screenType?: string | null;
  startTime: string;
  endTime: string;
  availableSeats: number;
  totalSeats: number;
  basePrice: number;
}

interface ShowtimeGroupListProps {
  showtimes: ShowtimeSlotItem[];
  className?: string;
}

export const ShowtimeGroupList = ({ showtimes, className }: ShowtimeGroupListProps) => {
  const navigate = useNavigate();
  const isAuthenticated = useIsAuthenticated();
  const isInitialized = useAuthStore((s) => s.isInitialized);
  const openAlertDialog = useAlertDialogOpen();

  const handleBooking = useCallback(
    (showtimeId: number) => {
      if (isAuthenticated) {
        navigate(`/booking/${showtimeId}`);
        return;
      }

      const redirectTo = encodeURIComponent(`/booking/${showtimeId}`);
      openAlertDialog({
        title: "Yêu cầu đăng nhập",
        description: "Bạn cần đăng nhập để đặt vé. Bạn có muốn đăng nhập ngay không?",
        confirmText: "Đăng nhập ngay",
        cancelText: "Để sau",
        onConfirm: () => {
          navigate(`/auth/login?redirect=${redirectTo}`);
        },
      });
    },
    [isAuthenticated, navigate, openAlertDialog],
  );

  const groups = useMemo(() => {
    const map = new Map<string, ShowtimeSlotItem[]>();
    for (const item of showtimes) {
      const key = item.screenType ?? "2D";
      if (!map.has(key)) map.set(key, []);
      map.get(key)!.push(item);
    }
    return Array.from(map.entries());
  }, [showtimes]);

  if (groups.length === 0) return null;

  return (
    <div className={cn("space-y-4", className)}>
      {groups.map(([format, items]) => (
        <div key={format} className="flex flex-col gap-2.5 sm:flex-row sm:items-start">
          <span className="w-24 shrink-0 pt-2 text-[13px] font-semibold text-muted-foreground">{format}</span>
          <div className="flex flex-wrap gap-2.5">
            {items.map((item) => {
              const soldOut = item.availableSeats === 0;
              return (
                <button
                  key={item.showtimeId}
                  type="button"
                  data-slot="showtime-slot"
                  disabled={soldOut || !isInitialized}
                  onClick={() => handleBooking(item.showtimeId)}
                  className={cn(
                    "flex flex-col gap-0.5 cut-sm bg-card px-3.5 py-2 text-left transition-colors cursor-pointer hover:bg-muted outline-none",
                    soldOut && "cursor-not-allowed opacity-40 hover:bg-card",
                  )}
                >
                  <span className="t-numeral text-base font-bold text-foreground">
                    {formatShowtimeRange(item.startTime, item.endTime)}
                  </span>
                  <span className="text-xs text-muted-foreground">
                    Phòng {item.roomName}, còn {item.availableSeats}/{item.totalSeats} ghế
                  </span>
                  <span className="t-numeral text-xs font-semibold text-primary">
                    {formatCurrency(item.basePrice)}
                  </span>
                </button>
              );
            })}
          </div>
        </div>
      ))}
    </div>
  );
};
