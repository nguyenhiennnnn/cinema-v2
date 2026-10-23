"use client";

import { Bar, BarChart, CartesianGrid, XAxis, YAxis } from "recharts";
import { TrendingUp } from "lucide-react";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import {
  ChartContainer,
  ChartTooltip,
  ChartTooltipContent,
  ChartLegend,
  ChartLegendContent,
  type ChartConfig,
} from "@/components/ui/chart";
import { Skeleton } from "@/components/ui/skeleton";
import { EmptyState } from "@/components/common/empty-state";
import { formatCurrency } from "@/lib/formatters";
import type { DashboardRevenueTrendItemDto } from "../types";

const chartConfig = {
  ticketRevenue: { label: "Doanh thu vé", color: "var(--chart-1)" },
  comboRevenue: { label: "Doanh thu combo", color: "var(--chart-2)" },
} satisfies ChartConfig;

interface RevenueTrendChartProps {
  data: DashboardRevenueTrendItemDto[] | undefined;
  isLoading: boolean;
}

export const RevenueTrendChart = ({ data, isLoading }: RevenueTrendChartProps) => {
  return (
    <Card className="lg:col-span-2">
      <CardHeader>
        <CardTitle>Doanh thu theo ngày</CardTitle>
        <CardDescription>Vé và combo trong khoảng thời gian đã chọn</CardDescription>
      </CardHeader>
      <CardContent>
        {isLoading ? (
          <Skeleton className="h-70 w-full" />
        ) : !data || data.length === 0 ? (
          <EmptyState
            icon={TrendingUp}
            title="Chưa có dữ liệu doanh thu"
            description="Không có giao dịch nào trong khoảng thời gian này."
          />
        ) : (
          <ChartContainer config={chartConfig} className="h-70 w-full">
            <BarChart accessibilityLayer data={data}>
              <CartesianGrid vertical={false} />
              <XAxis dataKey="period" tickLine={false} axisLine={false} tickMargin={10} />
              <YAxis
                tickLine={false}
                axisLine={false}
                tickMargin={8}
                width={80}
                tickFormatter={(value: number) => formatCurrency(value)}
              />
              <ChartTooltip
                content={
                  <ChartTooltipContent
                    formatter={(value, name) => (
                      <div className="flex w-full items-center justify-between gap-4">
                        <span className="text-muted-foreground">
                          {chartConfig[name as keyof typeof chartConfig]?.label ?? name}
                        </span>
                        <span className="font-medium tabular-nums text-foreground">
                          {formatCurrency(Number(value))}
                        </span>
                      </div>
                    )}
                  />
                }
              />
              <ChartLegend content={<ChartLegendContent />} />
              <Bar dataKey="ticketRevenue" stackId="revenue" fill="var(--color-ticketRevenue)" radius={0} />
              <Bar dataKey="comboRevenue" stackId="revenue" fill="var(--color-comboRevenue)" radius={0} />
            </BarChart>
          </ChartContainer>
        )}
      </CardContent>
    </Card>
  );
}