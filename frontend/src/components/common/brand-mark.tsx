import type { CSSProperties } from "react";
import { cn } from "@/lib/utils";
import { BRAND } from "@/config/brand";

interface BrandMarkProps {
  className?: string;
  /** Chạy hiệu ứng đèn chạy trên hàng lỗ xé của vé (dùng cho màn hình loading). */
  loading?: boolean;
}

export const BrandMark = ({ className, loading = false }: BrandMarkProps) => (
  <svg viewBox="4 14 56 36" aria-hidden="true" className={cn("h-auto", className)}>
    <path
      className="fill-primary"
      d="M6 14H35A5 5 0 0 0 45 14H58A2 2 0 0 1 60 16V48A2 2 0 0 1 58 50H45A5 5 0 0 0 35 50H6A2 2 0 0 1 4 48V16A2 2 0 0 1 6 14Z"
    />
    <g className="fill-primary-foreground">
      {[22, 27, 32, 37, 42].map((y, i) => (
        <circle
          key={y}
          cx="40"
          cy={y}
          r="1.5"
          className={loading ? "perf-dot" : undefined}
          style={loading ? ({ "--i": i } as CSSProperties) : undefined}
        />
      ))}
      <rect x="47.5" y="24" width="2" height="16" />
      <rect x="51.5" y="26" width="1.2" height="12" />
      <rect x="54.5" y="24" width="2" height="16" />
    </g>
    <path
      className="stroke-primary-foreground"
      d="M15 43V23.5H21.5L24 26V30.5L21.5 33H15"
      fill="none"
      strokeWidth="5"
      strokeMiterlimit="10"
    />
  </svg>
);

export const BrandWordmark = ({ className }: { className?: string }) => (
  <svg viewBox="0 0 184 48" role="img" aria-label={BRAND.name} className={cn("h-auto", className)}>
    <clipPath id="perfo-wm-clip">
      <rect x="-2" width="190" height="48" />
    </clipPath>
    <g
      clipPath="url(#perfo-wm-clip)"
      className="stroke-foreground"
      fill="none"
      strokeWidth="9"
      strokeMiterlimit="10"
    >
      <path d="M4.5 48V4.5H23L27.5 9V24L23 28.5H4.5" />
      <path transform="translate(38 0)" d="M27.5 4.5H4.5V43.5H27.5M4.5 24H23" />
      <path transform="translate(76 0)" d="M4.5 48V4.5H23L27.5 9V24L23 28.5H4.5M14 28.5L30.3 52" />
      <path transform="translate(114 0)" d="M27.5 4.5H4.5V48M4.5 24H23" />
      <path transform="translate(152 0)" d="M9 4.5H23L27.5 9V39L23 43.5H9L4.5 39V9Z" />
    </g>
    <circle cx="168" cy="24" r="4.2" className="fill-primary" />
  </svg>
);