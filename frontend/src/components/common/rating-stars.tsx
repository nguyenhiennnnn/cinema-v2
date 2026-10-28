import { Star } from "lucide-react";

import { cn } from "@/lib/utils";

interface RatingStarsProps {
  value: number;
  onChange?: (value: number) => void;
  max?: number;
  size?: "sm" | "md";
  className?: string;
}

export const RatingStars = ({
  value,
  onChange,
  max = 5,
  size = "md",
  className,
}: RatingStarsProps) => {
  const readOnly = !onChange;
  const starSize = size === "sm" ? "size-3.5" : "size-4";

  return (
    <div
      className={cn("inline-flex items-center gap-0.5", className)}
      role={readOnly ? undefined : "radiogroup"}
    >
      {Array.from({ length: max }, (_, i) => i + 1).map((star) => (
        <button
          key={star}
          type="button"
          disabled={readOnly}
          onClick={() => onChange?.(star)}
          aria-label={`${star} sao`}
          className={cn(
            !readOnly && "cursor-pointer",
            readOnly && "cursor-default",
          )}
        >
          <Star
            className={cn(
              starSize,
              star <= Math.round(value)
                ? "fill-accent text-accent"
                : "text-muted-foreground/40 fill-none",
            )}
          />
        </button>
      ))}
    </div>
  );
};
