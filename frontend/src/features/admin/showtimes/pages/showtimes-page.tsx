import { useState } from "react";
import { CalendarPlus, Copy, Plus, Sparkles, Upload } from "lucide-react";

import { PageHeader } from "@/components/common/page-header";
import { ExportButton } from "@/components/common/export-button";
import { ENDPOINTS } from "@/api/endpoints";
import { Button } from "@/components/ui/button";
import { toast } from "@/components/ui/toast";
import { DataTable } from "@/components/common/data-table/data-table";
import { BulkDeleteButton } from "@/components/common/data-table/bulk-delete-button";
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
import type { RowSelectionState } from "@tanstack/react-table";

import {
  useAdminShowtimes,
  useAdminDeleteShowtime,
  useAdminBulkDeleteShowtimes,
} from "../hooks/use-admin-showtimes";
import type { AdminShowtimeDto, ShowtimeCalendarItemDto } from "../types";
import { createShowtimeColumns } from "../components/showtime-columns";
import { createSelectionColumn } from "@/components/common/data-table/data-table-selection-column";
import { ShowtimeCreateForm } from "../components/forms/showtime-create-form";
import { ShowtimeUpdateForm } from "../components/forms/showtime-update-form";
import { ShowtimeBulkCreateForm } from "../components/forms/showtime-bulk-create-form";
import { ShowtimeCloneForm } from "../components/forms/showtime-clone-form";
import { ShowtimeAutoScheduleForm } from "../components/forms/showtime-auto-schedule-form";
import { ShowtimeCalendarView } from "../components/calendar/showtime-calendar-view";
import { ImportWizard } from "@/components/common/import-wizard/import-wizard";
import { showtimeImportConfig } from "../config/showtime-import-config";
import { useAuthRole } from "@/stores/auth-store";
import { Role } from "@/types/enum";

const AdminShowtimesPage = () => {
  const [params, setParams] = useAdminShowtimeParams();
  const { pagination, onPaginationChange } = useTablePagination(params, setParams);
  const role = useAuthRole();
  const canManage = role === Role.Admin;

  const [rowSelection, setRowSelection] = useState<RowSelectionState>({});

  const showtimes = useAdminShowtimes(stripNulls(params));
  const deleteShowtime = useAdminDeleteShowtime();
  const bulkDeleteShowtimes = useAdminBulkDeleteShowtimes();

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
      description:
        "Vé đã đặt sẽ bị hủy. Thanh toán online được hoàn tiền tự động; vé thanh toán tiền mặt cần hoàn thủ công tại quầy.",
      confirmText: "Hủy suất chiếu",
      variant: "destructive",
      onConfirm: async () => {
        const res = await deleteShowtime.mutateAsync(showtime.showtimeId);
        setRowSelection((prev) =>
          Object.fromEntries(Object.entries(prev).filter(([key]) => key !== String(showtime.showtimeId))),
        );
        const refundCount = res.data?.manualRefundRequiredCount ?? 0;
        const autoCount = res.data?.autoRefundQueuedCount ?? 0;
        const notes = [
          autoCount > 0 ? `${autoCount} giao dịch online đang được hoàn tiền tự động.` : null,
          refundCount > 0 ? `${refundCount} vé cần được hoàn tiền thủ công.` : null,
        ].filter(Boolean);
        if (notes.length > 0) {
          toast.add({
            title: "Lưu ý hoàn tiền",
            description: notes.join(" "),
            type: "info",
          });
        }
      },
    });
  };

  const handleBulkDelete = async (selectedIds: string[]) => {
    const ids = selectedIds.map(Number);
    const res = await bulkDeleteShowtimes.mutateAsync({ showtimeIds: ids });
    const result = res.data;
    setRowSelection({});
    if (result) {
      const { totalSuccess, totalFailed, results } = result;
      const failedReasons = results
        .filter((r) => !r.success && r.reason)
        .map((r) => `#${r.showtimeId}: ${r.reason}`)
        .join("; ");
      toast.add({
        title: `Đã xóa ${totalSuccess} suất chiếu${totalFailed > 0 ? `, ${totalFailed} thất bại` : ""}.`,
        description: failedReasons || undefined,
        type: totalFailed > 0 ? "warning" : "success",
      });
    }
  };

  const handleBulkCreate = () => {
    openDialog({
      title: "Tạo suất chiếu hàng loạt",
      description: "Lên lịch nhiều suất chiếu lặp lại theo ngày trong tuần.",
      content: <ShowtimeBulkCreateForm onSuccess={closeDialog} />,
    });
  };

  const handleClone = () => {
    openDialog({
      title: "Nhân bản lịch chiếu",
      description: "Sao chép các suất chiếu của một khoảng ngày sang tuần khác.",
      content: <ShowtimeCloneForm onSuccess={closeDialog} />,
    });
  };

  const handleAutoSchedule = () => {
    openDialog({
      title: "Tự động xếp lịch",
      description: "Đề xuất lịch chiếu cả ngày cho toàn bộ phòng, xem lại rồi áp dụng.",
      content: <ShowtimeAutoScheduleForm onSuccess={closeDialog} />,
      size: "xl",
    });
  };

  const handleImport = () => {
    openDialog({
      title: showtimeImportConfig.title,
      description: showtimeImportConfig.description,
      size: "xl",
      className: "sm:max-w-5xl",
      content: <ImportWizard config={showtimeImportConfig} onClose={closeDialog} />,
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

  const selectionCol = createSelectionColumn<AdminShowtimeDto>();
  const columns = [
    selectionCol,
    ...createShowtimeColumns({
      onEdit: handleEdit,
      onDelete: handleDelete,
      canManage,
    }),
  ];

  return (
    <div>
      <PageHeader
        title="Quản lý Suất chiếu"
        description="Lên lịch và theo dõi trạng thái các suất chiếu trong rạp."
        action={
          canManage ? (
            <div className="flex gap-2">
              <Button variant="outline" onClick={handleImport}>
                <Upload className="size-4" />
                <span>Import</span>
              </Button>
              <Button variant="outline" onClick={handleAutoSchedule}>
                <Sparkles className="size-4" />
                <span>Tự động xếp lịch</span>
              </Button>
              <Button variant="outline" onClick={handleClone}>
                <Copy className="size-4" />
                <span>Nhân bản lịch</span>
              </Button>
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
            <ExportButton
              endpoint={ENDPOINTS.admin.showtimes.export}
              params={{
                movieId: params.movieId,
                roomId: params.roomId,
                date: params.date,
                status: params.status,
              }}
              fallbackFilename="showtimes"
              size="default"
              className="h-10"
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
            rowSelection={rowSelection}
            onRowSelectionChange={setRowSelection}
            getRowId={(row) => String(row.showtimeId)}
            bulkActions={
              canManage
                ? (selectedIds) => (
                    <BulkDeleteButton
                      count={selectedIds.length}
                      itemLabel="suất chiếu"
                      onConfirm={() => handleBulkDelete(selectedIds)}
                      isPending={bulkDeleteShowtimes.isPending}
                    />
                  )
                : undefined
            }
          />
        </TabsContent>

        <TabsContent value="calendar">
          <ShowtimeCalendarView
            onSelectShowtime={canManage ? handleSelectFromCalendar : undefined}
          />
        </TabsContent>
      </Tabs>
    </div>
  );
};

export default AdminShowtimesPage;
