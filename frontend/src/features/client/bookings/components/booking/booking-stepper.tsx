import { Fragment } from "react";
import { Check } from "lucide-react";
import { cn } from "@/lib/utils";
import type { BookingStep } from "../../hooks/use-booking-flow";

const STEPS: { key: BookingStep | "ticket"; label: string }[] = [
  { key: "select", label: "Chọn ghế" },
  { key: "payment", label: "Thanh toán" },
  { key: "ticket", label: "Vé" },
];

export const BookingStepper = ({ step }: { step: BookingStep }) => {
  const currentIndex = STEPS.findIndex((s) => s.key === step);

  return (
    <ol className="flex items-center gap-2" aria-label="Các bước đặt vé">
      {STEPS.map((item, index) => {
        const isDone = index < currentIndex;
        const isCurrent = index === currentIndex;

        return (
          <Fragment key={item.key}>
            {index > 0 && <li aria-hidden className={cn("h-px flex-1", isDone ? "bg-primary" : "bg-border")} />}
            <li
              aria-current={isCurrent ? "step" : undefined}
              className={cn(
                "flex items-center gap-2 text-sm font-medium",
                isCurrent ? "text-foreground" : isDone ? "text-primary" : "text-muted-foreground",
              )}
            >
              <span
                className={cn(
                  "t-numeral flex size-7 shrink-0 items-center justify-center cut-sm text-sm",
                  isCurrent
                    ? "bg-primary text-primary-foreground"
                    : isDone
                      ? "bg-primary/15 text-primary"
                      : "bg-secondary text-muted-foreground",
                )}
              >
                {isDone ? <Check className="size-4" /> : index + 1}
              </span>
              <span className={cn(!isCurrent && "hidden sm:inline")}>{item.label}</span>
            </li>
          </Fragment>
        );
      })}
    </ol>
  );
};
