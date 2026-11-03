import { Clock } from "lucide-react";
import { cn } from "@/lib/utils";
import { useCountdown } from "@/hooks/use-countdown";
import { Tooltip, TooltipContent, TooltipTrigger } from "@/components/ui/tooltip";

export const BookingHoldTimer = ({ expiresAt, className }: { expiresAt: string; className?: string }) => {
  const { label, remainingMs } = useCountdown(expiresAt);
  const isLow = remainingMs > 0 && remainingMs < 60_000;

  return (
    <Tooltip>
      <TooltipTrigger
        render={
          <div
            role="timer"
            className={cn(
              "flex items-center gap-1.5 text-xs",
              isLow ? "font-semibold text-destructive" : "font-medium text-warning",
              className,
            )}
          >
            <Clock className="size-3.5" />
            <span>Giữ ghế còn {label}</span>
            <span className="sr-only" aria-live="polite">
              {isLow ? "Giữ ghế còn dưới 60 giây." : ""}
            </span>
          </div>
        }
      />
      <TooltipContent>Ghế sẽ tự động được giải phóng nếu bạn chưa hoàn tất thanh toán trước khi hết giờ.</TooltipContent>
    </Tooltip>
  );
}
