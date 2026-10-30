import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { toast } from "@/components/ui/toast";
import { Button } from "@/components/ui/button";
import { Switch } from "@/components/ui/switch";
import { Field, FieldError, FieldGroup, FieldLabel } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";
import { FormDateTimePickerField } from "@/components/common/form/form-datetime-picker-field";
import { SingleSelectField } from "@/components/common/form/single-select-field";

import { useAdminMovies } from "@/features/admin/movies/hooks/use-admin-movies";
import { useAdminRooms } from "@/features/admin/rooms/hooks/use-admin-rooms";
import { useAdminCreateShowtime } from "../../hooks/use-admin-showtimes";
import { LOOKUP_PAGE_SIZE, SEARCH_LOOKUP_PAGE_SIZE } from "@/config/default-constants";
import { toBusinessIso } from "@/lib/formatters";

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

  const movies = useAdminMovies({ search: movieSearch, pageSize: SEARCH_LOOKUP_PAGE_SIZE });
  const rooms = useAdminRooms({ pageSize: LOOKUP_PAGE_SIZE });

  const form = useForm<ShowtimeFormValues>({
    resolver: zodResolver(showtimeFormSchema),
    defaultValues: { startTime: "", basePrice: 75000, allowAfterHours: false },
  });

  const onSubmit = async (values: ShowtimeFormValues) => {
    await createShowtime.mutateAsync({
      movieId: values.movieId,
      roomId: values.roomId,
      startTime: toBusinessIso(values.startTime),
      basePrice: values.basePrice,
      allowAfterHours: values.allowAfterHours,
    }, {
      onSuccess: () => {
        toast.add({ title: "Đã tạo suất chiếu mới.", type: "success" });
        onSuccess();
      }
    });
  };

  return (
    <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
      <FieldGroup>
        <Controller
          name="movieId"
          control={form.control}
          render={({ field, fieldState }) => (
            <Field data-invalid={fieldState.invalid}>
              <FieldLabel>Phim</FieldLabel>
              <SingleSelectField
                value={field.value}
                onChange={(v) => field.onChange(v)}
                options={movies.data?.items ?? []}
                getOptionValue={(o) => o.movieId}
                getOptionLabel={(o) => o.title}
                searchValue={movieSearch}
                onSearchChange={setMovieSearch}
                isLoading={movies.isLoading}
                placeholder="Chọn phim"
              />
              {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
            </Field>
          )}
        />

        <Controller
          name="roomId"
          control={form.control}
          render={({ field, fieldState }) => (
            <Field data-invalid={fieldState.invalid}>
              <FieldLabel>Phòng chiếu</FieldLabel>
              <SingleSelectField
                value={field.value}
                onChange={(v) => field.onChange(v)}
                options={rooms.data?.items ?? []}
                getOptionValue={(o) => o.roomId}
                getOptionLabel={(o) => o.roomName}
                searchValue={roomSearch}
                onSearchChange={setRoomSearch}
                isLoading={rooms.isLoading}
                placeholder="Chọn phòng chiếu"
              />
              {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
            </Field>
          )}
        />

        <FormDateTimePickerField
          name="startTime"
          control={form.control}
          label="Thời gian bắt đầu chiếu"
          placeholder="Chọn ngày & giờ chiếu"
          clearable={false}
          calendarDisabled={{ before: new Date(new Date().setHours(0, 0, 0, 0)) }}
        />

        <FormInputField
          name="basePrice"
          control={form.control}
          label="Giá vé (VNĐ)"
          type="number"
          min={0}
          step={1000}
        />

        <Controller
          name="allowAfterHours"
          control={form.control}
          render={({ field }) => (
            <Field className="flex flex-row items-center justify-between">
              <FieldLabel>Cho phép suất khuya (kết thúc sau giờ đóng cửa)</FieldLabel>
              <Switch checked={field.value} onCheckedChange={field.onChange} />
            </Field>
          )}
        />
      </FieldGroup>

      <div className="flex justify-end gap-2 pt-2">
        <Button type="button" variant="outline" onClick={onSuccess} disabled={createShowtime.isPending}>
          Hủy
        </Button>
        <Button type="submit" disabled={createShowtime.isPending}>
          {createShowtime.isPending ? "Đang lưu..." : "Tạo suất chiếu"}
        </Button>
      </div>
    </form>
  );
};