import type { ReactNode } from "react";
import type { LucideIcon } from "lucide-react";
import { cn } from "@/lib/utils";

const DEFAULT_ACTIVE = "border-primary bg-primary text-primary-foreground";
const INACTIVE = "border-border bg-card text-foreground hover:bg-muted";

interface SeatEditorToolButtonProps {
  active: boolean;
  onClick: () => void;
  icon?: LucideIcon;
  activeClassName?: string;
  children: ReactNode;
}

export const SeatEditorToolButton = ({
  active,
  onClick,
  icon: Icon,
  activeClassName = DEFAULT_ACTIVE,
  children,
}: SeatEditorToolButtonProps) => (
  <button
    type="button"
    onClick={onClick}
    className={cn(
      "border px-3 py-1.5 text-xs font-medium transition-all",
      Icon && "flex items-center gap-1.5",
      active ? activeClassName : INACTIVE,
    )}
  >
    {Icon && <Icon className="size-3.5" />}
    {children}
  </button>
);
