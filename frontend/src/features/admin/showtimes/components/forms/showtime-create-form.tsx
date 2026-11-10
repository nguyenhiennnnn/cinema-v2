import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";
import { FormDateTimePickerField } from "@/components/common/form/form-datetime-picker-field";

import { useAdminMovies } from "@/features/admin/movies/hooks/use-admin-movies";
import { useAdminRooms } from "@/features/admin/rooms/hooks/use-admin-rooms";
import { useAdminCreateShowtime } from "../../hooks/use-admin-showtimes";
import { LOOKUP_PAGE_SIZE, SEARCH_LOOKUP_PAGE_SIZE } from "@/config/default-constants";
import { getBusinessTodayDate, toBusinessIso } from "@/lib/business-time";
import { MovieStatus } from "@/types/enum";
import { FormSearchSelectField } from "@/components/common/form/form-search-select-field";
import { FormSwitchField } from "@/components/common/form/form-switch-field";
import { FormActions } from "@/components/common/form/form-actions";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";
import { useDebouncedValue } from "@/hooks/use-debounced-value";

const showtimeFormSchema = z.object({
  movieId: z.coerce.number({ message: "Vui lòng chọn phim." }),
  roomId: z.coerce.number({ message: "Vui lòng chọn phòng chiếu." }),
  startTime: z.string().min(1, "Vui lòng chọn thời gian chiếu."),
  basePrice: z.coerce.number().min(0, "Giá vé không hợp lệ."),
  allowAfterHours: z.boolean(),
});

type ShowtimeFormValues = z.infer<typeof showtimeFormSchema>;

interface ShowtimeCreateFormProps {
  onSuccess: () => void;
}

export const ShowtimeCreateForm = ({ onSuccess }: ShowtimeCreateFormProps) => {
  const createShowtime = useAdminCreateShowtime();

  const [movieSearch, setMovieSearch] = useState("");
  const [roomSearch, setRoomSearch] = useState("");

  const movies = useAdminMovies({ search: useDebouncedValue(movieSearch), pageSize: SEARCH_LOOKUP_PAGE_SIZE });
  const rooms = useAdminRooms({ pageSize: LOOKUP_PAGE_SIZE });

  const form = useForm<ShowtimeFormValues>({
    resolver: zodResolver(showtimeFormSchema),
    defaultValues: { startTime: "", basePrice: 75000, allowAfterHours: false },
  });

  const applyServerErrors = useServerFieldErrors(form);

  const onSubmit = (values: ShowtimeFormValues) => {
    createShowtime.mutate({
      movieId: values.movieId,
      roomId: values.roomId,
      startTime: toBusinessIso(values.startTime),
      basePrice: values.basePrice,
      allowAfterHours: values.allowAfterHours,
    }, {
      onSuccess: () => onSuccess(),
      onError: applyServerErrors,
    });
  };

  return (
    <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
      <FieldGroup disabled={createShowtime.isPending}>
        <FormSearchSelectField
          name="movieId"
          control={form.control}
          label="Phim"
          options={(movies.data?.items ?? []).filter((m) => m.status !== MovieStatus.Ended)}
          getOptionValue={(o) => o.movieId}
          getOptionLabel={(o) => o.title}
          searchValue={movieSearch}
          onSearchChange={setMovieSearch}
          isLoading={movies.isLoading}
          placeholder="Chọn phim"
        />

        <FormSearchSelectField
          name="roomId"
          control={form.control}
          label="Phòng chiếu"
          options={(rooms.data?.items ?? []).filter((r) => r.isActive)}
          getOptionValue={(o) => o.roomId}
          getOptionLabel={(o) => o.roomName}
          searchValue={roomSearch}
          onSearchChange={setRoomSearch}
          isLoading={rooms.isLoading}
          placeholder="Chọn phòng chiếu"
        />

        <FormDateTimePickerField
          name="startTime"
          control={form.control}
          label="Thời gian bắt đầu chiếu"
          placeholder="Chọn ngày & giờ chiếu"
          clearable={false}
          calendarDisabled={{ before: getBusinessTodayDate() }}
        />

        <FormInputField
          name="basePrice"
          control={form.control}
          label="Giá vé (VNĐ)"
          type="number"
          min={0}
          step={1000}
        />

        <FormSwitchField name="allowAfterHours" control={form.control} label="Cho phép suất khuya (kết thúc sau giờ đóng cửa)" />
      </FieldGroup>

      <FormActions
        isPending={createShowtime.isPending}
        isDirty={form.formState.isDirty}
        submitLabel="Tạo suất chiếu"
        onCancel={onSuccess}
      />
    </form>
  );
};