import { useState } from "react";
import { Controller, useFieldArray, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { Plus, Trash2 } from "lucide-react";

import { toast } from "@/components/ui/toast";
import { Button } from "@/components/ui/button";
import { Field, FieldGroup, FieldLabel } from "@/components/ui/field";
import { FormActions } from "@/components/common/form/form-actions";
import { FormInputField } from "@/components/common/form/form-input-field";
import { FormDatePickerField } from "@/components/common/form/form-date-picker-field";
import { TimePickerField } from "@/components/common/date-time/time-picker-field";

import { useAdminMovies } from "@/features/admin/movies/hooks/use-admin-movies";
import { useAdminRooms } from "@/features/admin/rooms/hooks/use-admin-rooms";
import { useAdminBulkCreateShowtimes } from "@/features/admin/showtimes/hooks/use-admin-showtimes";
import { LOOKUP_PAGE_SIZE, SEARCH_LOOKUP_PAGE_SIZE } from "@/config/default-constants";
import { getBusinessTodayDate } from "@/lib/business-time";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";
import { FormSwitchField } from "@/components/common/form/form-switch-field";
import { FormSearchSelectField } from "@/components/common/form/form-search-select-field";
import { useDebouncedValue } from "@/hooks/use-debounced-value";

const daysOfWeekOptions = [
  { value: "Mon", label: "Th 2" },
  { value: "Tue", label: "Th 3" },
  { value: "Wed", label: "Th 4" },
  { value: "Thu", label: "Th 5" },
  { value: "Fri", label: "Th 6" },
  { value: "Sat", label: "Th 7" },
  { value: "Sun", label: "CN" },
];

const bulkFormSchema = z.object({
  movieId: z.coerce.number({ message: "Vui lòng chọn phim." }),
  roomId: z.coerce.number({ message: "Vui lòng chọn phòng chiếu." }),
  fromDate: z.string().min(1, "Vui lòng chọn ngày bắt đầu."),
  toDate: z.string().min(1, "Vui lòng chọn ngày kết thúc."),
  daysOfWeek: z.array(z.string()).min(1, "Chọn ít nhất một thứ trong tuần."),
  times: z.array(z.object({ value: z.string().min(1, "Vui lòng chọn giờ chiếu.") })).min(1, "Thêm ít nhất một khung giờ."),
  basePrice: z.coerce.number().min(0, "Giá vé không hợp lệ."),
  allowAfterHours: z.boolean(),
});

type BulkFormValues = z.infer<typeof bulkFormSchema>;

interface ShowtimeBulkCreateFormProps {
  onSuccess: () => void;
}

export const ShowtimeBulkCreateForm = ({ onSuccess }: ShowtimeBulkCreateFormProps) => {
  const bulkCreate = useAdminBulkCreateShowtimes();

  const [movieSearch, setMovieSearch] = useState("");
  const [roomSearch, setRoomSearch] = useState("");

  const movies = useAdminMovies({ search: useDebouncedValue(movieSearch), pageSize: SEARCH_LOOKUP_PAGE_SIZE });
  const rooms = useAdminRooms({ pageSize: LOOKUP_PAGE_SIZE });

  const form = useForm<BulkFormValues>({
    resolver: zodResolver(bulkFormSchema),
    defaultValues: {
      fromDate: "",
      toDate: "",
      daysOfWeek: [],
      times: [{ value: "" }],
      basePrice: 75000,
      allowAfterHours: false,
    },
  });

  const applyServerErrors = useServerFieldErrors(form);

  const { fields, append, remove } = useFieldArray({ control: form.control, name: "times" });

  const toggleDay = (day: string) => {
    const current = form.getValues("daysOfWeek");
    if (current.includes(day)) {
      form.setValue("daysOfWeek", current.filter((d) => d !== day), { shouldDirty: true });
    } else {
      form.setValue("daysOfWeek", [...current, day], { shouldDirty: true });
    }
  };

  const onSubmit = (values: BulkFormValues) => {
    bulkCreate.mutate({
      movieId: values.movieId,
      roomId: values.roomId,
      dateRange: { from: values.fromDate, to: values.toDate },
      daysOfWeek: values.daysOfWeek,
      times: values.times.map((t) => t.value),
      basePrice: values.basePrice,
      allowAfterHours: values.allowAfterHours,
    }, {
      onSuccess: (res) => {
        toast.add({
          title: `Đã tạo ${res.data.totalCreated} suất chiếu.`,
          description:
            res.data.totalSkipped > 0
              ? `Bỏ qua ${res.data.totalSkipped} suất do trùng lịch hoặc không hợp lệ.`
              : undefined,
          type: "success",
        });
        onSuccess();
      },
      onError: applyServerErrors,
    });
  };

  const selectedDays = form.watch("daysOfWeek");

  return (
    <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
      <FieldGroup disabled={bulkCreate.isPending}>
        <FormSearchSelectField
          name="movieId"
          control={form.control}
          label="Phim"
          options={movies.data?.items ?? []}
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
          options={rooms.data?.items ?? []}
          getOptionValue={(o) => o.roomId}
          getOptionLabel={(o) => o.roomName}
          searchValue={roomSearch}
          onSearchChange={setRoomSearch}
          isLoading={rooms.isLoading}
          placeholder="Chọn phòng chiếu"
        />

        <div className="grid grid-cols-2 gap-3">
          <FormDatePickerField
            name="fromDate"
            control={form.control}
            label="Từ ngày"
            calendarDisabled={{ before: getBusinessTodayDate() }}
          />
          <FormDatePickerField
            name="toDate"
            control={form.control}
            label="Đến ngày"
            calendarDisabled={{ before: getBusinessTodayDate() }}
          />
        </div>

        <Field>
          <FieldLabel>Các thứ trong tuần lặp lại</FieldLabel>
          <div className="flex flex-wrap gap-1.5">
            {daysOfWeekOptions.map((day) => (
              <Button
                key={day.value}
                type="button"
                size="sm"
                variant={selectedDays.includes(day.value) ? "default" : "outline"}
                onClick={() => toggleDay(day.value)}
              >
                {day.label}
              </Button>
            ))}
          </div>
        </Field>

        <Field>
          <FieldLabel>Khung giờ chiếu</FieldLabel>
          <div className="space-y-2">
            {fields.map((item, index) => (
              <div key={item.id} className="flex items-center gap-2">
                <div className="flex-1">
                  <Controller
                    name={`times.${index}.value`}
                    control={form.control}
                    render={({ field, fieldState }) => (
                      <TimePickerField
                        value={field.value}
                        onChange={(v) => field.onChange(v ?? "")}
                        aria-invalid={fieldState.invalid}
                      />
                    )}
                  />
                </div>
                <Button
                  type="button"
                  variant="ghost"
                  size="icon"
                  className="size-8 shrink-0 text-destructive"
                  onClick={() => remove(index)}
                  disabled={fields.length === 1}
                >
                  <Trash2 className="size-4" />
                </Button>
              </div>
            ))}
            <Button type="button" variant="outline" size="sm" onClick={() => append({ value: "" })}>
              <Plus className="size-4" /> Thêm khung giờ
            </Button>
          </div>
        </Field>

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
        isPending={bulkCreate.isPending}
        isDirty={form.formState.isDirty}
        submitLabel="Tạo hàng loạt"
        pendingLabel="Đang tạo..."
        onCancel={onSuccess}
      />
    </form>
  );
};