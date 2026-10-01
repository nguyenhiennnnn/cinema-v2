import { useState } from "react";
import { PageHeader } from "@/components/common/page-header";
import { useDashboardSummary, useDashboardInsights } from "../hooks/use-dashboard";
import { DashboardStatCards } from "../components/dashboard-stat-cards";
import { RevenueTrendChart } from "../components/revenue-trend-chart";
import { TopMoviesCard } from "../components/top-movies-card";
import { TodayShowtimesCard } from "../components/today-showtimes-card";
import { DashboardDaysFilter } from "../components/dashboard-days-filter";

const AdminDashboardPage = () => {
  const [days, setDays] = useState(14);

  const { data: summary, isLoading: isSummaryLoading } = useDashboardSummary();
  const { data: insights, isLoading: isInsightsLoading } = useDashboardInsights({ days });

  return (
    <div className="space-y-6">
      <PageHeader
        title="Tổng quan"
        description="Theo dõi doanh thu, vé bán và tình hình rạp theo thời gian thực."
        action={<DashboardDaysFilter value={days} onChange={setDays} />}
      />

      <DashboardStatCards summary={summary} isLoading={isSummaryLoading} />

      <div className="grid grid-cols-1 gap-4 lg:grid-cols-3">
        <RevenueTrendChart data={insights?.revenueTrend} isLoading={isInsightsLoading} />
        <TopMoviesCard movies={insights?.topMovies} isLoading={isInsightsLoading} />
      </div>

      <TodayShowtimesCard showtimes={insights?.todayShowtimes} isLoading={isInsightsLoading} />
    </div>
  );
};

export default AdminDashboardPage;