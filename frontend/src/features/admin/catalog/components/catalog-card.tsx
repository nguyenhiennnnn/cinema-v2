import { Link } from "react-router";
import type { LucideIcon } from "lucide-react";
import { Skeleton } from "@/components/ui/skeleton";
import { cn } from "@/lib/utils";

export interface CatalogCardProps {
  to: string;
  icon: LucideIcon;
  title: string;
  description: string;
  count?: number;
  isLoading: boolean;
  accent: string;
}

export const CatalogCard = ({
  to,
  icon: Icon,
  title,
  description,
  count,
  isLoading,
  accent,
}: CatalogCardProps) => {
  return (
    <Link
      to={to}
      className={cn(
        "group relative flex flex-col justify-between overflow-hidden border border-border bg-card p-5 transition-all duration-300",
        "hover:border-primary",
      )}
    >
      <div className="flex items-center justify-between">
        <div
          className={cn(
            "flex size-11 items-center justify-center border border-border",
            accent,
          )}
        >
          <Icon className="size-5" />
        </div>
        <div className="text-right">
          {isLoading ? (
            <Skeleton className="ml-auto h-7 w-10" />
          ) : (
            <span className="font-heading text-2xl font-semibold tabular text-foreground">
              {count ?? 0}
            </span>
          )}
          <p className="text-[11px] text-muted-foreground">bản ghi</p>
        </div>
      </div>

      <div className="mt-4">
        <h3 className="font-heading text-base font-semibold text-foreground uppercase transition-colors group-hover:text-primary">
          {title}
        </h3>
        <p className="mt-1 line-clamp-2 text-xs text-muted-foreground">
          {description}
        </p>
      </div>
    </Link>
  );
};
