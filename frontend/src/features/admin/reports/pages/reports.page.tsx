import { useState } from "react";
import { PageHeader } from "@/components/common/page-header";
import { toDateInputValue } from "@/lib/formatters";
import { useRevenueReport, useTopMoviesReport, useEmployeePerformanceReport } from "../hooks/use-reports";
import { ReportsFilterBar } from "../components/reports-filter-bar";
import { RevenueSummaryCards } from "../components/revenue-summary-cards";
import { RevenueReportChart } from "../components/revenue-report-chart";
import { TopMoviesReportTable } from "../components/top-movies-report-table";
import { EmployeePerformanceTable } from "../components/employee-performance-table";

function getDefaultRange() {
  const to = new Date();
  const from = new Date();
  from.setDate(from.getDate() - 29);
  return { fromDate: toDateInputValue(from) ?? "", toDate: toDateInputValue(to) ?? "" };
}

const AdminReportsPage = () => {
  const defaultRange = getDefaultRange();
  const [fromDate, setFromDate] = useState(defaultRange.fromDate);
  const [toDate, setToDate] = useState(defaultRange.toDate);
  const [groupBy, setGroupBy] = useState("day");

  const hasValidRange = Boolean(fromDate && toDate);

  const { data: revenueReport, isLoading: isRevenueLoading } = useRevenueReport(
    { fromDate, toDate, groupBy },
    hasValidRange,
  );
  const { data: topMovies, isLoading: isTopMoviesLoading } = useTopMoviesReport(
    { fromDate, toDate, limit: 10 },
    hasValidRange,
  );
  const { data: employeePerformance, isLoading: isEmployeeLoading } = useEmployeePerformanceReport(
    { fromDate, toDate },
    hasValidRange,
  );

  const handleReset = () => {
    const range = getDefaultRange();
    setFromDate(range.fromDate);
    setToDate(range.toDate);
    setGroupBy("day");
  };

  return (
    <div className="space-y-6">
      <PageHeader title="Báo cáo" description="Phân tích doanh thu, phim bán chạy và hiệu suất nhân viên." />

      <ReportsFilterBar
        fromDate={fromDate}
        toDate={toDate}
        groupBy={groupBy}
        onFromDateChange={(value) => setFromDate(value ?? "")}
        onToDateChange={(value) => setToDate(value ?? "")}
        onGroupByChange={setGroupBy}
        onReset={handleReset}
      />

      <RevenueSummaryCards summary={revenueReport?.summary} isLoading={isRevenueLoading} />

      <RevenueReportChart items={revenueReport?.items} isLoading={isRevenueLoading} />

      <div className="grid grid-cols-1 gap-4 lg:grid-cols-2">
        <TopMoviesReportTable movies={topMovies} isLoading={isTopMoviesLoading} />
        <EmployeePerformanceTable employees={employeePerformance} isLoading={isEmployeeLoading} />
      </div>
    </div>
  );
};

export default AdminReportsPage;