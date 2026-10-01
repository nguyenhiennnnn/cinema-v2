"use client";

import { Bar, CartesianGrid, ComposedChart, Line, XAxis, YAxis } from "recharts";
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
import type { RevenueReportItemDto } from "../types";

const chartConfig = {
  ticketRevenue: { label: "Doanh thu vé", color: "var(--chart-1)" },
  comboRevenue: { label: "Doanh thu combo", color: "var(--chart-2)" },
  bookingCount: { label: "Số đơn", color: "var(--chart-3)" },
} satisfies ChartConfig;

interface RevenueReportChartProps {
  items: RevenueReportItemDto[] | undefined;
  isLoading: boolean;
}

export const RevenueReportChart = ({ items, isLoading }: RevenueReportChartProps) => {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Biểu đồ doanh thu</CardTitle>
        <CardDescription>Doanh thu vé, combo và số đơn theo kỳ báo cáo</CardDescription>
      </CardHeader>
      <CardContent>
        {isLoading ? (
          <Skeleton className="h-80 w-full rounded-lg" />
        ) : !items || items.length === 0 ? (
          <EmptyState
            icon={TrendingUp}
            title="Không có dữ liệu"
            description="Không có doanh thu trong khoảng thời gian đã chọn."
          />
        ) : (
          <ChartContainer config={chartConfig} className="h-80 w-full">
            <ComposedChart accessibilityLayer data={items}>
              <CartesianGrid vertical={false} />
              <XAxis dataKey="period" tickLine={false} axisLine={false} tickMargin={10} />
              <YAxis
                yAxisId="revenue"
                tickLine={false}
                axisLine={false}
                width={80}
                tickFormatter={(value: number) => formatCurrency(value)}
              />
              <YAxis yAxisId="count" orientation="right" tickLine={false} axisLine={false} width={40} />
              <ChartTooltip content={<ChartTooltipContent />} />
              <ChartLegend content={<ChartLegendContent />} />
              <Bar yAxisId="revenue" dataKey="ticketRevenue" stackId="revenue" fill="var(--color-ticketRevenue)" radius={[0, 0, 4, 4]} />
              <Bar yAxisId="revenue" dataKey="comboRevenue" stackId="revenue" fill="var(--color-comboRevenue)" radius={[4, 4, 0, 0]} />
              <Line yAxisId="count" dataKey="bookingCount" stroke="var(--color-bookingCount)" strokeWidth={2} dot={false} />
            </ComposedChart>
          </ChartContainer>
        )}
      </CardContent>
    </Card>
  );
}