import type { ReactNode } from "react";
import { cn } from "@/lib/utils";

interface ImportWizardFooterProps {
  children: ReactNode;
  className?: string;
}

export const ImportWizardFooter = ({
  children,
  className,
}: ImportWizardFooterProps) => (
  <div
    data-slot="form-actions"
    className={cn(
      "sticky bottom-0 z-10 flex justify-end gap-2 border-t border-border py-3",
      "-mx-(--modal-gutter,0px) px-(--modal-gutter,0px)",
      "bg-(--modal-surface,var(--background))",
      className,
    )}
  >
    {children}
  </div>
);
