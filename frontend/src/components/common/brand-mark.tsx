import { useId } from "react";

import { cn } from "@/lib/utils";

export const BrandMark = ({ className }: { className?: string }) => {
  const maskId = useId();

  return (
    <svg
      viewBox="0 0 48 48"
      fill="none"
      className={cn("size-6 shrink-0", className)}
      aria-hidden="true"
    >
      <mask id={maskId} maskUnits="userSpaceOnUse">
        <rect width="48" height="48" rx="7" fill="#fff" />
        <circle cx="0" cy="24" r="5" fill="#000" />
        <circle cx="48" cy="24" r="5" fill="#000" />
      </mask>

      <rect width="48" height="48" rx="7" className="fill-primary" mask={`url(#${maskId})`} />

      <path
        d="M15.8 5v3.4M32.2 5v3.4M15.8 39.6V43M32.2 39.6V43"
        stroke="currentColor"
        className="text-accent"
        strokeWidth="2.2"
        strokeLinecap="round"
      />

      <circle cx="24" cy="24" r="9.5" className="fill-primary-foreground" />
      <path
        d="M21.2 19.6c0-.86.94-1.39 1.67-.94l7.36 4.4a1.1 1.1 0 0 1 0 1.88l-7.36 4.4a1.1 1.1 0 0 1-1.67-.94V19.6Z"
        className="fill-accent"
      />
    </svg>
  );
};