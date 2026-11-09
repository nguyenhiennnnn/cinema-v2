import type { ComponentProps } from "react";
import { cva, type VariantProps } from "class-variance-authority";
import { Loader2 } from "lucide-react";
import { BrandMark } from "@/components/common/brand-mark";
import { cn } from "@/lib/utils";

const DEFAULT_LABEL = "Đang tải";

export const spinnerVariants = cva(
  "shrink-0 animate-spin text-primary motion-reduce:animate-[spin_2s_linear_infinite]",
  {
    variants: {
      size: {
        sm: "size-4",
        md: "size-8",
        lg: "size-12",
      },
    },
    defaultVariants: {
      size: "md",
    },
  },
);

interface SpinnerProps
  extends Omit<ComponentProps<typeof Loader2>, "size">,
  VariantProps<typeof spinnerVariants> {
  label?: string;
  decorative?: boolean;
}

export const Spinner = ({
  className,
  size,
  label = DEFAULT_LABEL,
  decorative = false,
  ...props
}: SpinnerProps) => (
  <Loader2
    {...(decorative ? { "aria-hidden": true } : { role: "status", "aria-label": label })}
    className={cn(spinnerVariants({ size }), className)}
    {...props}
  />
);

interface PageLoaderProps {
  label?: string;
  className?: string;
  variant?: "page" | "section";
}

export const PageLoader = ({ label, className, variant = "page" }: PageLoaderProps) => (
  <div
    role="status"
    aria-live="polite"
    aria-busy="true"
    className={cn(
      "flex flex-col items-center justify-center gap-6 px-4",
      variant === "page" ? "min-h-screen bg-background" : "min-h-[50vh]",
      className,
    )}
  >
    <BrandMark loading className="w-16 shrink-0" />
    {label ? (
      <p className="text-sm text-muted-foreground">{label}</p>
    ) : (
      <span className="sr-only">{DEFAULT_LABEL}</span>
    )}
  </div>
);
