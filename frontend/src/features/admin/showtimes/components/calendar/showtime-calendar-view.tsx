import { useEffect, useState } from "react";
import { endOfWeek, format, startOfWeek } from "date-fns";

import { DateFilterPicker } from "@/components/common/filter/date-filter-picker";
import { EnumSelectFilter } from "@/components/common/filter/enum-select-filter";
import { SelectField } from "@/components/common/select-field";
import { Skeleton } from "@/components/ui/skeleton";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { showtimeStatusOptions } from "@/lib/enum-options";
import { useAdminShowtimeCalendarParams } from "@/hooks/use-query-params";
import { useAdminRooms } from "@/features/admin/rooms/hooks/use-admin-rooms";

import { useAdminShowtimesCalendar } from "../../hooks/use-admin-showtimes";
import type { ShowtimeCalendarItemDto } from "../../types";
import { ShowtimeCalendarDay } from "./showtime-calendar-day";

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
      const today = new Date();
      void setParams({
        from: format(startOfWeek(today, { weekStartsOn: 1 }), "yyyy-MM-dd"),
        to: format(endOfWeek(today, { weekStartsOn: 1 }), "yyyy-MM-dd"),
      });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const rooms = useAdminRooms({ page: 1, pageSize: 100 });

  const queryParams = {
    from: params.from ?? "",
    to: params.to ?? "",
    roomId: params.roomId ?? undefined,
    status: params.status ?? undefined,
  };

  const calendar = useAdminShowtimesCalendar(queryParams);

  const days = calendar.data?.days ?? [];
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
        <SelectField
          value={params.roomId ?? null}
          onChange={(value) => setParams({ roomId: value ? Number(value) : undefined })}
          options={rooms.data?.items ?? []}
          getOptionValue={(room) => room.roomId}
          getOptionLabel={(room) => room.roomName}
          placeholder="Phòng chiếu"
          className="sm:w-44"
        />
        <EnumSelectFilter
          value={params.status}
          onChange={(status) => setParams({ status })}
          options={showtimeStatusOptions}
          placeholder="Trạng thái suất chiếu"
          className="sm:w-52"
        />
      </div>

      {calendar.isLoading && (
        <Skeleton className="h-64 w-full rounded-xl" />
      )}

      {!calendar.isLoading && days.length === 0 && (
        <div className="flex h-40 items-center justify-center rounded-lg border border-dashed text-sm text-muted-foreground">
          Không có suất chiếu nào trong khoảng thời gian đã chọn.
        </div>
      )}

      {!calendar.isLoading && days.length > 0 && currentDay && (
        <Tabs
          value={currentDay.date}
          onValueChange={(value) => setActiveDay(value as string)}
        >
          <TabsList variant="line" className="flex-wrap">
            {days.map((day) => (
              <TabsTrigger key={day.date} value={day.date}>
                {day.date}
                <span className="ml-1 text-xs text-muted-foreground">
                  ({day.showtimes.length})
                </span>
              </TabsTrigger>
            ))}
          </TabsList>

          {days.map((day) => (
            <TabsContent key={day.date} value={day.date}>
              <ShowtimeCalendarDay
                day={day}
                onSelectShowtime={onSelectShowtime}
              />
            </TabsContent>
          ))}
        </Tabs>
      )}
    </div>
  );
};