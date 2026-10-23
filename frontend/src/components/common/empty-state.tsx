import type { LucideIcon } from "lucide-react";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";

interface EmptyStateProps {
  icon: LucideIcon;
  title: string;
  description?: string;
  actionLabel?: string;
  onAction?: () => void;
  className?: string;
}

export const EmptyState = ({ icon: Icon, title, description, actionLabel, onAction, className }: EmptyStateProps) => {
  return (
    <div className={cn("flex flex-col items-center justify-center gap-3 border border-dashed border-border bg-card/50 px-6 py-16 text-center", className)}>
      <div className="flex size-14 items-center justify-center cut-sm bg-secondary text-muted-foreground">
        <Icon className="size-6" />
      </div>
      <div className="space-y-1">
        <h3 className="t-h3 text-foreground">{title}</h3>
        {description && <p className="text-sm text-muted-foreground">{description}</p>}
      </div>
      {actionLabel && onAction && (
        <Button variant="secondary" onClick={onAction} className="mt-2">
          {actionLabel}
        </Button>
      )}
    </div>
  );
}
