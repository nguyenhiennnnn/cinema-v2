import { useState } from "react";
import { Controller, useFieldArray, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { Plus, Trash2 } from "lucide-react";

import { toast } from "@/components/ui/toast";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Field, FieldError, FieldGroup, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { Switch } from "@/components/ui/switch";
import { FormInputField } from "@/components/common/form/form-input-field";
import { FormDatePickerField } from "@/components/common/form/form-date-picker-field";
import { SingleSelectField } from "@/components/common/form/single-select-field";

import { useAdminMovies } from "@/features/admin/movies/hooks/use-admin-movies";
import {
  useAdminApplyAutoSchedule,
  useAdminPreviewAutoSchedule,
} from "../../hooks/use-admin-showtimes";
import type { AutoScheduleResponseDto } from "../../types";

const formSchema = z.object({
  date: z.string().min(1, "Vui lòng chọn ngày."),
  movies: z
    .array(
      z.object({
        movieId: z.coerce.number({ message: "Vui lòng chọn phim." }),
        shows: z.coerce
          .number()
          .int("Số suất không hợp lệ.")
          .min(1, "Tối thiểu 1 suất.")
          .max(20, "Tối đa 20 suất."),
      }),
    )
    .min(1, "Thêm ít nhất một phim."),
  basePrice: z.coerce.number().min(0, "Giá vé không hợp lệ."),
  allowAfterHours: z.boolean(),
});

type FormValues = z.infer<typeof formSchema>;

const hhmm = (iso: string) => iso.slice(11, 16);

interface ShowtimeAutoScheduleFormProps {
  onSuccess: () => void;
}

export const ShowtimeAutoScheduleForm = ({ onSuccess }: ShowtimeAutoScheduleFormProps) => {
  const preview = useAdminPreviewAutoSchedule();
  const apply = useAdminApplyAutoSchedule();

  const [movieSearch, setMovieSearch] = useState("");
  const [draft, setDraft] = useState<AutoScheduleResponseDto | null>(null);

  const movies = useAdminMovies({ search: movieSearch, pageSize: 20 });

  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      date: "",
      movies: [{ movieId: undefined, shows: 3 }],
      basePrice: 75000,
      allowAfterHours: false,
    },
  });

  const { fields, append, remove } = useFieldArray({ control: form.control, name: "movies" });

  const onPreview = async (values: FormValues) => {
    const res = await preview.mutateAsync(values);
    setDraft(res.data);
  };

  const onApply = async () => {
    if (!draft) return;
    const allowAfterHours = form.getValues("allowAfterHours");
    const res = await apply.mutateAsync({
      items: draft.items.map((i) => ({
        movieId: i.movieId,
        roomId: i.roomId,
        startTime: i.startTime,
        basePrice: i.basePrice,
        allowAfterHours,
      })),
    });
    toast.add({
      title: `Đã tạo ${res.data.totalCreated} suất chiếu.`,
      description:
        res.data.totalSkipped > 0
          ? `Bỏ qua ${res.data.totalSkipped} suất do trùng lịch hoặc không hợp lệ.`
          : undefined,
      type: "success",
    });
    onSuccess();
  };

  const removeDraftItem = (index: number) =>
    setDraft((d) => d && { ...d, items: d.items.filter((_, i) => i !== index) });

  if (draft) {
    return (
      <div className="space-y-4">
        {draft.unplaced.length > 0 && (
          <div className="space-y-1 rounded-lg border border-destructive/30 bg-destructive/10 p-3 text-sm text-destructive">
            {draft.unplaced.map((u) => (
              <p key={u.movieId}>
                {u.movieTitle}: chỉ xếp được {u.placed}/{u.requested} suất.
              </p>
            ))}
          </div>
        )}

        {draft.items.length === 0 ? (
          <p className="text-sm text-muted-foreground">Không có suất chiếu nào để áp dụng.</p>
        ) : (
          <ul className="divide-y divide-border rounded-lg border border-border">
            {draft.items.map((item, index) => (
              <li key={`${item.roomId}-${item.startTime}`} className="flex items-center gap-3 px-3 py-2">
                <span className="w-28 shrink-0 font-mono text-sm font-medium tabular-nums text-foreground">
                  {hhmm(item.startTime)} - {hhmm(item.endTime)}
                </span>
                <div className="min-w-0 flex-1">
                  <p className="truncate text-sm font-medium text-foreground">{item.movieTitle}</p>
                  <p className="text-xs text-muted-foreground">{item.roomName}</p>
                </div>
                {item.isPrime && (
                  <Badge variant="outline" className="border-accent text-accent">
                    Giờ vàng
                  </Badge>
                )}
                <Button
                  type="button"
                  variant="ghost"
                  size="icon"
                  className="size-8 shrink-0 text-destructive"
                  onClick={() => removeDraftItem(index)}
                >
                  <Trash2 className="size-4" />
                </Button>
              </li>
            ))}
          </ul>
        )}

        <div className="flex items-center justify-between gap-2 pt-2">
          <p className="text-sm text-muted-foreground">
            {draft.date} · {draft.items.length} suất
          </p>
          <div className="flex gap-2">
            <Button type="button" variant="outline" onClick={() => setDraft(null)} disabled={apply.isPending}>
              Quay lại
            </Button>
            <Button type="button" onClick={onApply} disabled={apply.isPending || draft.items.length === 0}>
              {apply.isPending ? "Đang áp dụng..." : "Áp dụng lịch"}
            </Button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <form onSubmit={form.handleSubmit(onPreview)} className="space-y-4">
      <FieldGroup>
        <FormDatePickerField
          name="date"
          control={form.control}
          label="Ngày chiếu"
          calendarDisabled={{ before: new Date(new Date().setHours(0, 0, 0, 0)) }}
        />

        <Field>
          <FieldLabel>Phim và số suất mong muốn</FieldLabel>
          <div className="space-y-2">
            {fields.map((item, index) => (
              <div key={item.id} className="flex items-start gap-2">
                <div className="min-w-0 flex-1">
                  <Controller
                    name={`movies.${index}.movieId`}
                    control={form.control}
                    render={({ field, fieldState }) => (
                      <>
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
                      </>
                    )}
                  />
                </div>
                <div className="w-24 shrink-0">
                  <Controller
                    name={`movies.${index}.shows`}
                    control={form.control}
                    render={({ field, fieldState }) => (
                      <>
                        <Input
                          {...field}
                          type="number"
                          min={1}
                          max={20}
                          aria-invalid={fieldState.invalid}
                        />
                        {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
                      </>
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
            <Button
              type="button"
              variant="outline"
              size="sm"
              onClick={() => append({ movieId: undefined as unknown as number, shows: 3 })}
              disabled={fields.length >= 20}
            >
              <Plus className="size-4" /> Thêm phim
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
        <Button type="button" variant="outline" onClick={onSuccess} disabled={preview.isPending}>
          Hủy
        </Button>
        <Button type="submit" disabled={preview.isPending}>
          {preview.isPending ? "Đang xếp lịch..." : "Xếp lịch đề xuất"}
        </Button>
      </div>
    </form>
  );
};
