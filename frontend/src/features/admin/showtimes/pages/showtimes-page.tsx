import { CalendarPlus, Plus } from "lucide-react";

import { PageHeader } from "@/components/common/page-header";
import { Button } from "@/components/ui/button";
import { toast } from "@/components/ui/toast";
import { DataTable } from "@/components/common/data-table/data-table";
import { EnumSelectFilter } from "@/components/common/filter/enum-select-filter";
import { DateFilterPicker } from "@/components/common/filter/date-filter-picker";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { useDialogClose, useDialogOpen } from "@/stores/dialog-store";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";
import { useAdminShowtimeParams } from "@/hooks/use-query-params";
import { useTablePagination } from "@/hooks/use-table-pagination";
import { stripNulls } from "@/lib/query-params";
import { showtimeStatusOptions } from "@/lib/enum-options";
import { DEFAULT_PAGE } from "@/config/default-constants";

import { useAdminShowtimes, useAdminDeleteShowtime } from "../hooks/use-admin-showtimes";
import type { AdminShowtimeDto, ShowtimeCalendarItemDto } from "../types";
import { createShowtimeColumns } from "../components/showtime-columns";
import { ShowtimeCreateForm } from "../components/forms/showtime-create-form";
import { ShowtimeUpdateForm } from "../components/forms/showtime-update-form";
import { ShowtimeBulkCreateForm } from "../components/forms/showtime-bulk-create-form";
import { ShowtimeCalendarView } from "../components/calendar/showtime-calendar-view";
import { useAuthRole } from "@/stores/auth-store";
import { Role } from "@/types/enum";

const AdminShowtimesPage = () => {
  const [params, setParams] = useAdminShowtimeParams();
  const { pagination, onPaginationChange } = useTablePagination(params, setParams);
  const role = useAuthRole();
  const canManage = role === Role.Admin;

  const showtimes = useAdminShowtimes(stripNulls(params));
  const deleteShowtime = useAdminDeleteShowtime();

  const openDialog = useDialogOpen();
  const closeDialog = useDialogClose();
  const openAlertDialog = useAlertDialogOpen();

  const handleCreate = () => {
    openDialog({
      title: "Tạo suất chiếu mới",
      description: "Chọn phim, phòng chiếu và thời gian bắt đầu.",
      content: <ShowtimeCreateForm onSuccess={closeDialog} />,
    });
  };

  const handleEdit = (showtime: AdminShowtimeDto) => {
    openDialog({
      title: "Cập nhật suất chiếu",
      description: `Chỉnh sửa suất chiếu phim "${showtime.movieTitle}".`,
      content: <ShowtimeUpdateForm showtime={showtime} onSuccess={closeDialog} />,
    });
  };

  const handleDelete = (showtime: AdminShowtimeDto) => {
    openAlertDialog({
      title: `Hủy suất chiếu "${showtime.movieTitle}"?`,
      description: "Vé đã đặt cho suất chiếu này (nếu có) sẽ cần được xử lý hoàn tiền thủ công.",
      confirmText: "Hủy suất chiếu",
      variant: "destructive",
      onConfirm: async () => {
        const res = await deleteShowtime.mutateAsync(showtime.showtimeId);
        const refundCount = res.data?.manualRefundRequiredCount ?? 0;
        toast.add({
          title: "Đã hủy suất chiếu.",
          description:
            refundCount > 0
              ? `Có ${refundCount} vé cần được hoàn tiền thủ công.`
              : undefined,
          type: "success",
        });
      },
    });
  };

  const handleBulkCreate = () => {
    openDialog({
      title: "Tạo suất chiếu hàng loạt",
      description: "Lên lịch nhiều suất chiếu lặp lại theo ngày trong tuần.",
      content: <ShowtimeBulkCreateForm onSuccess={closeDialog} />,
    });
  };

  const handleSelectFromCalendar = (showtime: ShowtimeCalendarItemDto) => {
    const mapped: AdminShowtimeDto = {
      showtimeId: showtime.showtimeId,
      movieId: showtime.movieId,
      movieTitle: showtime.movieTitle,
      roomId: showtime.roomId,
      roomName: showtime.roomName,
      startTime: showtime.startTime,
      endTime: showtime.endTime,
      basePrice: showtime.basePrice,
      status: showtime.status,
    };
    handleEdit(mapped);
  };

  const columns = createShowtimeColumns({
    onEdit: handleEdit,
    onDelete: handleDelete,
    canManage,
  });

  return (
    <div>
      <PageHeader
        title="Quản lý Suất chiếu"
        description="Lên lịch và theo dõi trạng thái các suất chiếu trong rạp."
        action={
          canManage ? (
            <div className="flex gap-2">
              <Button variant="outline" onClick={handleBulkCreate}>
                <CalendarPlus className="size-4" />
                <span>Tạo hàng loạt</span>
              </Button>
              <Button onClick={handleCreate}>
                <Plus className="size-4" />
                <span>Thêm suất chiếu</span>
              </Button>
            </div>
          ) : undefined
        }
      />

      <Tabs defaultValue="list">
        <TabsList className="mb-4">
          <TabsTrigger value="list">Danh sách</TabsTrigger>
          <TabsTrigger value="calendar">Lịch</TabsTrigger>
        </TabsList>

        <TabsContent value="list">
          <div className="mb-4 flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-end">
            <DateFilterPicker
              value={params.date}
              onChange={(date) => setParams({ date, page: DEFAULT_PAGE })}
              placeholder="Lọc theo ngày chiếu"
              className="sm:w-48"
            />
            <EnumSelectFilter
              value={params.status}
              onChange={(status) => setParams({ status, page: DEFAULT_PAGE })}
              options={showtimeStatusOptions}
              placeholder="Trạng thái suất chiếu"
              className="sm:w-52"
            />
          </div>

          <DataTable
            columns={columns}
            data={showtimes.data?.items ?? []}
            isLoading={showtimes.isLoading}
            totalItems={showtimes.data?.totalItems ?? 0}
            pagination={pagination}
            onPaginationChange={onPaginationChange}
            emptyMessage="Chưa có suất chiếu nào."
          />
        </TabsContent>

        <TabsContent value="calendar">
          <ShowtimeCalendarView onSelectShowtime={handleSelectFromCalendar} />
        </TabsContent>
      </Tabs>
    </div>
  );
};

export default AdminShowtimesPage;