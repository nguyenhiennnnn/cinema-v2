import { Users } from "lucide-react";
import type { ReactNode } from "react";
import { Card, CardAction, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Skeleton } from "@/components/ui/skeleton";
import { EmptyState } from "@/components/common/empty-state";
import { formatCurrency, formatNumber } from "@/lib/formatters";
import type { EmployeePerformanceDto } from "../types";

interface EmployeePerformanceTableProps {
  employees: EmployeePerformanceDto[] | undefined;
  isLoading: boolean;
  action?: ReactNode;
}

export const EmployeePerformanceTable = ({ employees, isLoading, action }: EmployeePerformanceTableProps) => {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Hiệu suất nhân viên</CardTitle>
        <CardDescription>Số vé, số đơn và doanh thu theo nhân viên</CardDescription>
        {action && <CardAction>{action}</CardAction>}
      </CardHeader>
      <CardContent>
        {isLoading ? (
          <div className="space-y-2">
            {Array.from({ length: 5 }).map((_, i) => (
              <Skeleton key={i} className="h-10 w-full" />
            ))}
          </div>
        ) : !employees || employees.length === 0 ? (
          <EmptyState icon={Users} title="Không có dữ liệu" description="Không có hoạt động nào trong khoảng thời gian này." />
        ) : (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Nhân viên</TableHead>
                <TableHead className="text-right">Vé bán</TableHead>
                <TableHead className="text-right">Số đơn</TableHead>
                <TableHead className="text-right">Doanh thu</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {employees.map((emp) => (
                <TableRow key={emp.employeeId}>
                  <TableCell className="font-medium text-foreground">{emp.fullName}</TableCell>
                  <TableCell className="text-right tabular-nums">{formatNumber(emp.ticketsSold)}</TableCell>
                  <TableCell className="text-right tabular-nums">{formatNumber(emp.bookingCount)}</TableCell>
                  <TableCell className="text-right tabular-nums font-medium text-primary">
                    {formatCurrency(emp.revenue)}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        )}
      </CardContent>
    </Card>
  );
}

