import { cn } from "@/lib/utils";

interface JobProgressBarProps {
  value: number;
  label: string;
  indeterminate?: boolean;
  className?: string;
}

export const JobProgressBar = ({
  value,
  label,
  indeterminate = false,
  className,
}: JobProgressBarProps) => {
  const clamped = Math.min(Math.max(Math.round(value), 0), 100);

  return (
    <div
      role="progressbar"
      aria-label={label}
      aria-valuemin={0}
      aria-valuemax={100}
      aria-valuenow={indeterminate ? undefined : clamped}
      className={cn("h-1.5 w-full overflow-hidden bg-secondary", className)}
    >
      <div
        className={cn(
          "h-full bg-primary transition-[width] duration-300 motion-reduce:transition-none",
          indeterminate && "w-1/3 animate-pulse",
        )}
        style={indeterminate ? undefined : { width: `${clamped}%` }}
      />
    </div>
  );
};
