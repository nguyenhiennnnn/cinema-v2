import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";
import { FormDateTimePickerField } from "@/components/common/form/form-datetime-picker-field";
import { toBusinessIso } from "@/lib/business-time";
import { toDateTimeInputValue } from "@/lib/formatters";

import { useAdminMovies } from "@/features/admin/movies/hooks/use-admin-movies";
import { useAdminRooms } from "@/features/admin/rooms/hooks/use-admin-rooms";
import { useAdminUpdateShowtime } from "../../hooks/use-admin-showtimes";
import type { AdminShowtimeDto } from "../../types";
import { LOOKUP_PAGE_SIZE, SEARCH_LOOKUP_PAGE_SIZE } from "@/config/default-constants";
import { FormSearchSelectField } from "@/components/common/form/form-search-select-field";

const showtimeFormSchema = z.object({
  movieId: z.coerce.number({ message: "Vui lòng chọn phim." }),
  roomId: z.coerce.number({ message: "Vui lòng chọn phòng chiếu." }),
  startTime: z.string().min(1, "Vui lòng chọn thời gian chiếu."),
  basePrice: z.coerce.number().min(0, "Giá vé không hợp lệ."),
});

type ShowtimeFormValues = z.infer<typeof showtimeFormSchema>;

interface ShowtimeUpdateFormProps {
  showtime: AdminShowtimeDto;
  onSuccess: () => void;
}

export const ShowtimeUpdateForm = ({ showtime, onSuccess }: ShowtimeUpdateFormProps) => {
  const updateShowtime = useAdminUpdateShowtime();

  const [movieSearch, setMovieSearch] = useState("");
  const [roomSearch, setRoomSearch] = useState("");

  const movies = useAdminMovies({ search: movieSearch, pageSize: SEARCH_LOOKUP_PAGE_SIZE });
  const rooms = useAdminRooms({ pageSize: LOOKUP_PAGE_SIZE });

  const form = useForm<ShowtimeFormValues>({
    resolver: zodResolver(showtimeFormSchema),
    defaultValues: {
      movieId: showtime.movieId,
      roomId: showtime.roomId,
      startTime: toDateTimeInputValue(showtime.startTime) ?? "",
      basePrice: showtime.basePrice,
    },
  });

  const onSubmit = async (values: ShowtimeFormValues) => {
    await updateShowtime.mutateAsync({
      id: showtime.showtimeId,
      body: {
        movieId: values.movieId,
        roomId: values.roomId,
        startTime: toBusinessIso(values.startTime),
        basePrice: values.basePrice,
      },
    }, {
      onSuccess: () => {
        onSuccess();
      }
    });
  };

  return (
    <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
      <FieldGroup>
        <FormSearchSelectField
          name="movieId"
          control={form.control}
          label="Phim"
          options={movies.data?.items ?? []}
          getOptionValue={(o) => o.movieId}
          getOptionLabel={(o) => o.title}
          selectedLabel={showtime.movieTitle}
          searchValue={movieSearch}
          onSearchChange={setMovieSearch}
          isLoading={movies.isLoading}
          placeholder="Chọn phim"
        />

        <FormSearchSelectField
          name="roomId"
          control={form.control}
          label="Phòng chiếu"
          options={rooms.data?.items ?? []}
          getOptionValue={(o) => o.roomId}
          getOptionLabel={(o) => o.roomName}
          selectedLabel={showtime.roomName}
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
        />

        <FormInputField
          name="basePrice"
          control={form.control}
          label="Giá vé (VNĐ)"
          type="number"
          min={0}
          step={1000}
        />
      </FieldGroup>

      <div className="flex justify-end gap-2 pt-2">
        <Button type="button" variant="outline" onClick={onSuccess} disabled={updateShowtime.isPending}>
          Hủy
        </Button>
        <Button type="submit" disabled={updateShowtime.isPending}>
          {updateShowtime.isPending ? "Đang lưu..." : "Lưu thay đổi"}
        </Button>
      </div>
    </form>
  );
};