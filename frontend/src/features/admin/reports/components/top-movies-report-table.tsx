import { Trophy } from "lucide-react";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Skeleton } from "@/components/ui/skeleton";
import { EmptyState } from "@/components/common/empty-state";
import { formatCurrency, formatNumber } from "@/lib/formatters";
import type { TopMovieDto } from "../types";

interface TopMoviesReportTableProps {
  movies: TopMovieDto[] | undefined;
  isLoading: boolean;
}

export const TopMoviesReportTable = ({ movies, isLoading }: TopMoviesReportTableProps) => {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Phim bán chạy nhất</CardTitle>
        <CardDescription>Xếp hạng theo số vé và doanh thu</CardDescription>
      </CardHeader>
      <CardContent>
        {isLoading ? (
          <div className="space-y-2">
            {Array.from({ length: 5 }).map((_, i) => (
              <Skeleton key={i} className="h-10 w-full" />
            ))}
          </div>
        ) : !movies || movies.length === 0 ? (
          <EmptyState icon={Trophy} title="Không có dữ liệu" description="Chưa có phim nào bán vé trong khoảng này." />
        ) : (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead className="w-10">#</TableHead>
                <TableHead>Phim</TableHead>
                <TableHead className="text-right">Vé bán</TableHead>
                <TableHead className="text-right">Doanh thu</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {movies.map((movie, index) => (
                <TableRow key={movie.movieId}>
                  <TableCell className="text-muted-foreground">{index + 1}</TableCell>
                  <TableCell className="font-medium text-foreground">{movie.title}</TableCell>
                  <TableCell className="text-right tabular-nums">{formatNumber(movie.ticketsSold)}</TableCell>
                  <TableCell className="text-right tabular-nums font-medium text-primary">
                    {formatCurrency(movie.revenue)}
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

