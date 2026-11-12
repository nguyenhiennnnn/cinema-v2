import { useEffect, useMemo, useState } from "react";
import { endOfWeek, startOfWeek } from "date-fns";
import { CalendarX, TriangleAlert } from "lucide-react";

import { DateFilterPicker } from "@/components/common/filter/date-filter-picker";
import { EnumSelectFilter } from "@/components/common/filter/enum-select-filter";
import { EmptyState } from "@/components/common/empty-state";
import { SelectField } from "@/components/common/select-field";
import { Skeleton } from "@/components/ui/skeleton";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { getBusinessTodayDate } from "@/lib/business-time";
import { showtimeStatusOptions } from "@/lib/enum-options";
import { formatWeekdayShort } from "@/lib/cinema-format";
import { toDateInputValue } from "@/lib/formatters";
import { useAdminShowtimeCalendarParams } from "@/hooks/use-query-params";
import { useAuthRole } from "@/stores/auth-store";
import { Role } from "@/types/enum";
import { useAdminRooms } from "@/features/admin/rooms/hooks/use-admin-rooms";

import { useAdminShowtimesCalendar } from "@/features/admin/showtimes/hooks/use-admin-showtimes";
import type { ShowtimeCalendarItemDto } from "@/features/admin/showtimes/types";
import {
  buildDayLayout,
  parseCalendarDay,
} from "@/features/admin/showtimes/utils/calendar-layout";
import { ShowtimeCalendarDay } from "./showtime-calendar-day";
import { ShowtimeCalendarLegend } from "./showtime-calendar-legend";
import { LOOKUP_PAGE_SIZE } from "@/config/default-constants";

interface ShowtimeCalendarViewProps {
  onSelectShowtime?: (showtime: ShowtimeCalendarItemDto) => void;
}

export const ShowtimeCalendarView = ({
  onSelectShowtime,
}: ShowtimeCalendarViewProps) => {
  const [params, setParams] = useAdminShowtimeCalendarParams();
  const [activeDay, setActiveDay] = useState<string>();

  useEffect(() => {
    if (!params.from || !params.to) {
      const today = getBusinessTodayDate();
      void setParams({
        from: toDateInputValue(startOfWeek(today, { weekStartsOn: 1 })) ?? "",
        to: toDateInputValue(endOfWeek(today, { weekStartsOn: 1 })) ?? "",
      });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  // GET /admin/rooms chỉ dành cho Admin; Employee gọi sẽ bị 403 và hiện toast lỗi.
  const canFilterByRoom = useAuthRole() === Role.Admin;
  const rooms = useAdminRooms(
    { page: 1, pageSize: LOOKUP_PAGE_SIZE },
    { enabled: canFilterByRoom },
  );

  const queryParams = {
    from: params.from ?? "",
    to: params.to ?? "",
    roomId: params.roomId ?? undefined,
    status: params.status ?? undefined,
  };

  const calendar = useAdminShowtimesCalendar(queryParams);

  const days = useMemo(() => calendar.data?.days ?? [], [calendar.data]);
  const layouts = useMemo(
    () => new Map(days.map((d) => [d.date, buildDayLayout(d.showtimes)])),
    [days],
  );
  const currentDay = days.find((d) => d.date === activeDay) ?? days[0];

  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-end">
        <DateFilterPicker
          value={params.from}
          onChange={(from) => setParams({ from })}
          placeholder="Từ ngày"
          className="sm:w-44"
        />
        <DateFilterPicker
          value={params.to}
          onChange={(to) => setParams({ to })}
          placeholder="Đến ngày"
          className="sm:w-44"
        />
        {canFilterByRoom && (
          <SelectField
            value={params.roomId ?? null}
            onChange={(value) => setParams({ roomId: value ? Number(value) : undefined })}
            options={rooms.data?.items ?? []}
            getOptionValue={(room) => room.roomId}
            getOptionLabel={(room) => room.roomName}
            placeholder="Phòng chiếu"
            className="sm:w-44"
          />
        )}
        <EnumSelectFilter
          value={params.status}
          onChange={(status) => setParams({ status })}
          options={showtimeStatusOptions}
          placeholder="Trạng thái suất chiếu"
          className="sm:w-52"
        />
      </div>

      <ShowtimeCalendarLegend editable={Boolean(onSelectShowtime)} />

      {calendar.isLoading && (
        <div className="flex flex-col gap-2" aria-busy>
          <Skeleton className="h-12 w-full" />
          {Array.from({ length: 4 }, (_, i) => (
            <Skeleton key={i} className="h-20 w-full" />
          ))}
        </div>
      )}

      {calendar.isError && (
        <EmptyState
          icon={CalendarX}
          title="Không tải được lịch chiếu"
          description="Kiểm tra kết nối rồi thử lại."
          actionLabel="Thử lại"
          onAction={() => void calendar.refetch()}
        />
      )}

      {!calendar.isLoading && !calendar.isError && days.length === 0 && (
        <EmptyState
          icon={CalendarX}
          title="Không có suất chiếu"
          description="Chưa có suất chiếu nào trong khoảng thời gian và bộ lọc đã chọn."
        />
      )}

      {!calendar.isLoading && days.length > 0 && currentDay && (
        <Tabs
          value={currentDay.date}
          onValueChange={(value) => setActiveDay(value as string)}
        >
          <TabsList
            variant="line"
            className="flex-nowrap overflow-x-auto overflow-y-hidden [scrollbar-width:none] [&::-webkit-scrollbar]:hidden"
          >
            {days.map((day) => {
              const parsed = parseCalendarDay(day.date);
              const conflicts = layouts.get(day.date)?.conflictCount ?? 0;
              return (
                <TabsTrigger
                  key={day.date}
                  value={day.date}
                  className="shrink-0 flex-col gap-0 leading-tight"
                >
                  <span className="text-xs font-normal">
                    {parsed ? formatWeekdayShort(parsed) : "Ngày"}
                  </span>
                  <span className="t-code text-base">
                    {parsed ? day.date.slice(0, 5) : day.date}
                  </span>
                  <span className="flex items-center gap-1 text-xs font-normal">
                    {day.showtimes.length} suất
                    {conflicts > 0 && (
                      <TriangleAlert
                        className="size-3"
                        role="img"
                        aria-label={`${conflicts} suất trùng giờ`}
                      />
                    )}
                  </span>
                </TabsTrigger>
              );
            })}
          </TabsList>

          {days.map((day) => (
            <TabsContent key={day.date} value={day.date}>
              <ShowtimeCalendarDay
                day={day}
                layout={layouts.get(day.date) ?? buildDayLayout(day.showtimes)}
                onSelectShowtime={onSelectShowtime}
              />
            </TabsContent>
          ))}
        </Tabs>
      )}
    </div>
  );
};
