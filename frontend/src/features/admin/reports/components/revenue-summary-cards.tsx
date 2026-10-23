import { Card, CardContent } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { formatCurrency } from "@/lib/formatters";
import type { RevenueSummaryDto } from "../types";
import { Wallet, Receipt } from "lucide-react";

interface RevenueSummaryCardsProps {
  summary: RevenueSummaryDto | undefined;
  isLoading: boolean;
}

export const RevenueSummaryCards = ({ summary, isLoading }: RevenueSummaryCardsProps) => {
  const items = [
    { label: "Tổng doanh thu", value: summary ? formatCurrency(summary.totalRevenue) : "—", icon: Wallet },
    { label: "Tổng đơn đặt vé", value: summary ? summary.totalBookings.toLocaleString("vi-VN") : "—", icon: Receipt },
  ];

  return (
    <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
      {items.map((item) => (
        <Card key={item.label} className="gap-2 py-4">
          <CardContent className="flex items-center justify-between gap-3 px-4">
            <div>
              <p className="text-xs font-medium text-muted-foreground">{item.label}</p>
              {isLoading ? (
                <Skeleton className="mt-1.5 h-7 w-full" />
              ) : (
                <p className="mt-1.5 text-2xl font-semibold tabular-nums text-foreground">{item.value}</p>
              )}
            </div>
            <div className="flex size-9 shrink-0 items-center justify-center bg-primary/10 text-primary">
              <item.icon className="size-4.5" />
            </div>
          </CardContent>
        </Card>
      ))}
    </div>
  );
}