"use client";

import { Bar, BarChart, CartesianGrid, XAxis, YAxis } from "recharts";
import { Trophy } from "lucide-react";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { ChartContainer, ChartTooltip, ChartTooltipContent, type ChartConfig } from "@/components/ui/chart";
import { Skeleton } from "@/components/ui/skeleton";
import { EmptyState } from "@/components/common/empty-state";
import type { DashboardTopMovieDto } from "../types";

const chartConfig = {
  ticketsSold: { label: "Vé đã bán", color: "var(--chart-3)" },
} satisfies ChartConfig;

interface TopMoviesCardProps {
  movies: DashboardTopMovieDto[] | undefined;
  isLoading: boolean;
}

export const TopMoviesCard = ({ movies, isLoading }: TopMoviesCardProps) => {
  const data = (movies ?? []).map((m) => ({
    ...m,
    label: m.title.length > 18 ? `${m.title.slice(0, 18)}…` : m.title,
  }));

  return (
    <Card>
      <CardHeader>
        <CardTitle>Phim bán chạy</CardTitle>
        <CardDescription>Xếp hạng theo số vé bán ra</CardDescription>
      </CardHeader>
      <CardContent>
        {isLoading ? (
          <Skeleton className="h-65 w-full" />
        ) : data.length === 0 ? (
          <EmptyState icon={Trophy} title="Chưa có dữ liệu" description="Chưa có phim nào bán vé trong khoảng này." />
        ) : (
          <ChartContainer config={chartConfig} className="h-65 w-full">
            <BarChart accessibilityLayer data={data} layout="vertical" margin={{ left: 8 }}>
              <CartesianGrid horizontal={false} />
              <XAxis type="number" hide />
              <YAxis dataKey="label" type="category" tickLine={false} axisLine={false} width={110} />
              <ChartTooltip content={<ChartTooltipContent hideLabel />} />
              <Bar dataKey="ticketsSold" fill="var(--color-ticketsSold)" radius={0} />
            </BarChart>
          </ChartContainer>
        )}
      </CardContent>
    </Card>
  );
}