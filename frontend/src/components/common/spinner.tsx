import { cn } from "@/lib/utils";

interface SpinnerProps {
  className?: string;
  size?: "sm" | "md" | "lg";
}

const sizeClassMap: Record<NonNullable<SpinnerProps["size"]>, string> = {
  sm: "size-4 border-2",
  md: "size-8 border-4",
  lg: "size-12 border-4",
};

export const Spinner = ({ className, size = "md" }: SpinnerProps) => (
  <div
    role="status"
    aria-label="Đang tải"
    className={cn(
      "animate-spin border-primary border-t-transparent",
      sizeClassMap[size],
      className,
    )}
  />
);

interface PageLoaderProps {
  label?: string;
  className?: string;
}

export const PageLoader = ({ label, className }: PageLoaderProps) => (
  <div
    className={cn(
      "flex h-screen flex-col items-center justify-center gap-3 bg-background",
      className,
    )}
  >
    <Spinner size="lg" />
    {label && <p className="text-sm text-muted-foreground">{label}</p>}
  </div>
);