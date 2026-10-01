import { CheckCircle2, XCircle, Film, Armchair, Clock } from "lucide-react";
import { Badge } from "@/components/ui/badge";
import { formatDateTime, formatTime } from "@/lib/formatters";
import type { CheckinResponse } from "../../bookings/types";


interface CheckinResultCardProps {
  status: "idle" | "success" | "error";
  result?: CheckinResponse | null;
  errorMessage?: string | null;
}

export const CheckinResultCard = ({ status, result, errorMessage }: CheckinResultCardProps) => {
  if (status === "idle") {
    return (
      <div className="flex h-full min-h-55 flex-col items-center justify-center gap-2 rounded-xl border border-dashed border-border text-muted-foreground">
        <Film className="size-8" />
        <p className="text-sm">Chưa có vé nào được soát</p>
      </div>
    );
  }

  if (status === "error") {
    return (
      <div className="flex h-full min-h-55 flex-col items-center justify-center gap-3 rounded-xl border border-destructive/40 bg-destructive/5 px-4 text-center">
        <XCircle className="size-10 text-destructive" />
        <p className="font-heading text-lg font-semibold text-destructive">Soát vé thất bại</p>
        <p className="text-sm text-muted-foreground">
          {errorMessage ?? "Mã vé không hợp lệ hoặc đã được sử dụng"}
        </p>
      </div>
    );
  }

  if (!result) return null;

  return (
    <div className="flex h-full min-h-55 flex-col gap-3 rounded-xl border border-success/40 bg-success/5 p-4">
      <div className="flex items-center gap-2">
        <CheckCircle2 className="size-8 text-success" />
        <div>
          <p className="font-heading text-lg font-semibold text-success">Check-in thành công</p>
          <p className="text-xs text-muted-foreground">Lúc {formatDateTime(result.checkedInAt)}</p>
        </div>
      </div>

      <div className="flex items-center gap-2 text-sm text-foreground">
        <Film className="size-4 text-muted-foreground" />
        <span className="line-clamp-1 font-medium">{result.movieTitle}</span>
      </div>

      <div className="flex items-center gap-2 text-sm text-foreground">
        <Armchair className="size-4 text-muted-foreground" />
        <span>
          Ghế{" "}
          <Badge variant="secondary" className="ml-1 font-mono">
            {result.seatRow}
            {result.seatNumber}
          </Badge>
        </span>
      </div>

      <div className="flex items-center gap-2 text-sm text-foreground">
        <Clock className="size-4 text-muted-foreground" />
        <span>Suất chiếu {formatTime(result.startTime)}</span>
      </div>
    </div>
  );
}