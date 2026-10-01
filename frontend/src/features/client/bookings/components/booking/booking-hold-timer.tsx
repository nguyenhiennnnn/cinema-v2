import { Clock } from "lucide-react";
import { useCountdown } from "@/hooks/use-countdown";
import { Tooltip, TooltipContent, TooltipTrigger } from "@/components/ui/tooltip";

export const BookingHoldTimer = ({ expiresAt }: { expiresAt: string }) => {
  const { label, remainingMs } = useCountdown(expiresAt);
  const isLow = remainingMs > 0 && remainingMs < 60_000;

  return (
    <Tooltip>
      <TooltipTrigger
        render={
          <div
            className={
              isLow
                ? "flex items-center gap-1.5 text-xs font-semibold text-destructive"
                : "flex items-center gap-1.5 text-xs font-medium text-warning"
            }
          >
            <Clock className="size-3.5" />
            <span>Giữ ghế còn {label}</span>
          </div>
        }
      />
      <TooltipContent>Ghế sẽ tự động được giải phóng nếu bạn chưa hoàn tất thanh toán trước khi hết giờ.</TooltipContent>
    </Tooltip>
  );
}
