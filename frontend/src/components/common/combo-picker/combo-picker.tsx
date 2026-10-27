import { PackageOpen } from "lucide-react";
import { formatCurrency } from "@/lib/formatters";
import { Badge } from "@/components/ui/badge";
import { Skeleton } from "@/components/ui/skeleton";
import { ComboQuantityStepper } from "./combo-quantity-stepper";

export interface ComboOption {
  comboId: number;
  name: string;
  description?: string | null;
  price: number;
  imageUrl?: string | null;
}

interface ComboPickerProps {
  combos: ComboOption[] | undefined;
  quantities: Record<number, number>;
  onChange: (comboId: number, quantity: number) => void;
  isLoading?: boolean;
  labels?: Record<number, string>;
}

export const ComboPicker = ({ combos, quantities, onChange, isLoading, labels }: ComboPickerProps) => {
  if (isLoading) {
    return (
      <div className="@container grid grid-cols-1 gap-3 @sm:grid-cols-2">
        {Array.from({ length: 4 }).map((_, i) => (
          <Skeleton key={i} className="h-20 w-full" />
        ))}
      </div>
    );
  }

  if (!combos || combos.length === 0) {
    return (
      <div className="flex flex-col items-center gap-2 border border-dashed border-border py-8 text-muted-foreground">
        <PackageOpen className="size-6" />
        <p className="text-sm">Hiện chưa có combo nào đang bán.</p>
      </div>
    );
  }

  return (
    <div className="@container">
      <div className="grid grid-cols-1 gap-3 @sm:grid-cols-2">
        {combos.map((combo) => (
          <div
            key={combo.comboId}
            className="flex items-center gap-3 border border-border bg-card p-3"
          >
            <div className="size-14 shrink-0 overflow-hidden bg-muted">
              {combo.imageUrl ? (
                <img src={combo.imageUrl} alt={combo.name} className="h-full w-full object-cover" />
              ) : (
                <div className="flex h-full w-full items-center justify-center text-muted-foreground">
                  <PackageOpen className="size-5" />
                </div>
              )}
            </div>
            <div className="min-w-0 flex-1">
              <p className="line-clamp-1 text-sm font-medium text-foreground">{combo.name}</p>
              <p className="text-xs font-semibold text-primary">{formatCurrency(combo.price)}</p>
              {labels?.[combo.comboId] && (
                <Badge variant="success" className="mt-1">
                  {labels[combo.comboId]}
                </Badge>
              )}
            </div>
            <div className="shrink-0">
              <ComboQuantityStepper
                value={quantities[combo.comboId] ?? 0}
                onChange={(qty) => onChange(combo.comboId, qty)}
              />
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};