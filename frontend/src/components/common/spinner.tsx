import type { ComponentProps } from "react";
import { cva, type VariantProps } from "class-variance-authority";
import { Loader2 } from "lucide-react";
import { BrandMark } from "@/components/common/brand-mark";
import { cn } from "@/lib/utils";

const DEFAULT_LABEL = "Đang tải";

export const spinnerVariants = cva(
  // Màu theo token (text-primary), không hard-code. Người dùng bật "giảm chuyển động" vẫn thấy tiến trình, chỉ quay chậm lại.
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
  /** Nội dung đọc cho screen reader. */
  label?: string;
  /** Dùng khi đã có vùng role="status" bao ngoài, tránh đọc trùng. */
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
  /** Dòng chữ hiển thị bên dưới. Không truyền thì chỉ có chữ ẩn cho screen reader. */
  label?: string;
  className?: string;
}

export const PageLoader = ({ label, className }: PageLoaderProps) => (
  <div
    role="status"
    aria-live="polite"
    aria-busy="true"
    className={cn(
      "flex min-h-screen flex-col items-center justify-center gap-6 bg-background px-4",
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