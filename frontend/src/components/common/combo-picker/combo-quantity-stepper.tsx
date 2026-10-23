import { Minus, Plus } from "lucide-react";
import { cn } from "@/lib/utils";

interface ComboQuantityStepperProps {
  value: number;
  onChange: (value: number) => void;
  max?: number;
  className?: string;
}

export const ComboQuantityStepper = ({ value, onChange, max = 20, className }: ComboQuantityStepperProps) => {
  return (
    <div className={cn("flex items-center gap-2", className)}>
      <button
        type="button"
        disabled={value <= 0}
        onClick={() => onChange(Math.max(0, value - 1))}
        className="flex size-7 items-center justify-center border border-border bg-card text-foreground transition-colors hover:bg-muted disabled:cursor-not-allowed disabled:opacity-40"
      >
        <Minus className="size-3.5" />
      </button>
      <span className="w-5 text-center text-sm font-semibold tabular-nums">{value}</span>
      <button
        type="button"
        disabled={value >= max}
        onClick={() => onChange(Math.min(max, value + 1))}
        className="flex size-7 items-center justify-center border border-border bg-card text-foreground transition-colors hover:bg-muted disabled:cursor-not-allowed disabled:opacity-40"
      >
        <Plus className="size-3.5" />
      </button>
    </div>
  );
}
