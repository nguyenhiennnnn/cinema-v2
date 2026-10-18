import { Popcorn } from "lucide-react";
import { Skeleton } from "@/components/ui/skeleton";
import { formatCurrency } from "@/lib/formatters";
import { useActiveCombos } from "@/features/client/combos/hooks/use-combos";

export const ComboCarousel = () => {
  const { data: combos, isLoading } = useActiveCombos();

  if (!isLoading && (!combos || combos.length === 0)) return null;

  return (
    <section>
      <h2 className="font-heading text-2xl font-semibold tracking-tight text-foreground uppercase md:text-3xl">Combo bắp nước bán chạy</h2>

      <div className="mt-5 flex gap-4 overflow-x-auto pb-2">
        {isLoading
          ? <Skeleton className="h-56 w-full rounded-xl" />
          : combos!.map((combo) => (
            <div key={combo.comboId} className="flex w-48 shrink-0 flex-col overflow-hidden rounded-xl border border-border bg-card">
              <div className="flex aspect-square items-center justify-center bg-muted">
                {combo.imageUrl ? (
                  <img src={combo.imageUrl} alt={combo.name} className="w-full aspect-2/3 object-cover" />
                ) : (
                  <Popcorn className="size-8 text-muted-foreground" />
                )}
              </div>
              <div className="flex flex-1 flex-col p-3">
                <p className="line-clamp-1 text-sm font-semibold text-foreground">{combo.name}</p>
                {combo.description && <p className="mt-1 line-clamp-2 text-xs text-muted-foreground">{combo.description}</p>}
                <p className="mt-2 text-sm font-bold text-primary">{formatCurrency(combo.price)}</p>
              </div>
            </div>
          ))}
      </div>
    </section>
  );
}
