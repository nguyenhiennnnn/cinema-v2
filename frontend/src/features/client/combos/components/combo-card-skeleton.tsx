import { Skeleton } from "@/components/ui/skeleton";

export const ComboCardSkeleton = () => {
  return (
    <div className="flex w-48 shrink-0 flex-col overflow-hidden border border-border/50 bg-card">
      <Skeleton className="aspect-2/3 w-full" />
    </div>
  );
};
