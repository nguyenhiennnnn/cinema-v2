import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { addDays, differenceInCalendarDays, parseISO, startOfWeek } from "date-fns";

import { getBusinessTodayDate } from "@/lib/business-time";
import { formatDate, toDateInputValue } from "@/lib/formatters";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";

import { toast } from "@/components/ui/toast";
import { Button } from "@/components/ui/button";
import { FieldGroup } from "@/components/ui/field";
import { FormActions } from "@/components/common/form/form-actions";
import { FormDatePickerField } from "@/components/common/form/form-date-picker-field";

import { useAdminCloneShowtimes } from "@/features/admin/showtimes/hooks/use-admin-showtimes";
import type { BulkCreateShowtimesResponse } from "@/features/admin/showtimes/types";
import { FormSwitchField } from "@/components/common/form/form-switch-field";

const MAX_SOURCE_DAYS = 31;

const formSchema = z
  .object({
    sourceFrom: z.string().min(1, "Vui lòng chọn ngày bắt đầu."),
    sourceTo: z.string().min(1, "Vui lòng chọn ngày kết thúc."),
    targetFrom: z.string().min(1, "Vui lòng chọn ngày bắt đầu của lịch mới."),
    allowAfterHours: z.boolean(),
  })
  .superRefine((v, ctx) => {
    if (!v.sourceFrom || !v.sourceTo || !v.targetFrom) return;
    const from = parseISO(v.sourceFrom);
    const to = parseISO(v.sourceTo);
    const target = parseISO(v.targetFrom);

    if (differenceInCalendarDays(to, from) < 0) {
      ctx.addIssue({ code: "custom", path: ["sourceTo"], message: "Ngày kết thúc phải sau ngày bắt đầu." });
    } else if (differenceInCalendarDays(to, from) + 1 > MAX_SOURCE_DAYS) {
      ctx.addIssue({
        code: "custom",
        path: ["sourceTo"],
        message: `Tối đa ${MAX_SOURCE_DAYS} ngày mỗi lần nhân bản.`,
      });
    }
    if (differenceInCalendarDays(target, from) < 1) {
      ctx.addIssue({
        code: "custom",
        path: ["targetFrom"],
        message: "Lịch mới phải bắt đầu sau ngày bắt đầu của lịch nguồn.",
      });
    }
  });

type FormValues = z.infer<typeof formSchema>;

interface ShowtimeCloneFormProps {
  onSuccess: () => void;
}

export const ShowtimeCloneForm = ({ onSuccess }: ShowtimeCloneFormProps) => {
  const clone = useAdminCloneShowtimes();
  const [result, setResult] = useState<BulkCreateShowtimesResponse | null>(null);

  const monday = startOfWeek(getBusinessTodayDate(), { weekStartsOn: 1 });
  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      sourceFrom: toDateInputValue(monday) ?? "",
      sourceTo: toDateInputValue(addDays(monday, 6)) ?? "",
      targetFrom: toDateInputValue(addDays(monday, 7)) ?? "",
      allowAfterHours: false,
    },
  });

  const applyServerErrors = useServerFieldErrors(form);

  const [sourceFrom, sourceTo, targetFrom] = form.watch(["sourceFrom", "sourceTo", "targetFrom"]);
  const canPreviewTarget = Boolean(sourceFrom && sourceTo && targetFrom);
  const targetRangeLabel = canPreviewTarget
    ? (() => {
        const shift = differenceInCalendarDays(parseISO(targetFrom), parseISO(sourceFrom));
        const a = formatDate(addDays(parseISO(sourceFrom), shift));
        const b = formatDate(addDays(parseISO(sourceTo), shift));
        return shift >= 1 ? `${a} – ${b}` : null;
      })()
    : null;

  const onSubmit = (values: FormValues) => {
    const shiftDays = differenceInCalendarDays(parseISO(values.targetFrom), parseISO(values.sourceFrom));
    clone.mutate(
      {
        sourceFrom: values.sourceFrom,
        sourceTo: values.sourceTo,
        shiftDays,
        allowAfterHours: values.allowAfterHours,
      },
      {
        onSuccess: (res) => {
          toast.add({
            title: `Đã nhân bản ${res.data.totalCreated} suất chiếu.`,
            description:
              res.data.totalSkipped > 0
                ? `Bỏ qua ${res.data.totalSkipped} suất do trùng lịch hoặc không hợp lệ.`
                : undefined,
            type: res.data.totalCreated > 0 ? "success" : "warning",
          });
          if (res.data.totalSkipped > 0) setResult(res.data);
          else onSuccess();
        },
        onError: applyServerErrors,
      },
    );
  };

  if (result) {
    return (
      <div className="space-y-4">
        <p className="text-sm text-foreground">
          Đã tạo <strong>{result.totalCreated}</strong> suất · bỏ qua <strong>{result.totalSkipped}</strong> suất:
        </p>
        <ul className="max-h-72 divide-y divide-border overflow-y-auto border border-border">
          {result.skipped.map((s, i) => (
            <li key={`${s.date}-${s.time}-${i}`} className="flex items-start gap-3 px-3 py-2 text-sm">
              <span className="w-32 shrink-0 font-mono tabular-nums text-foreground">
                {s.date} {s.time}
              </span>
              <span className="text-muted-foreground">{s.reason}</span>
            </li>
          ))}
        </ul>
        <div className="flex justify-end pt-2">
          <Button type="button" onClick={onSuccess}>
            Đóng
          </Button>
        </div>
      </div>
    );
  }

  return (
    <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
      <FieldGroup disabled={clone.isPending}>
        <div className="grid grid-cols-2 gap-3">
          <FormDatePickerField name="sourceFrom" control={form.control} label="Sao chép từ ngày" />
          <FormDatePickerField name="sourceTo" control={form.control} label="Đến ngày" />
        </div>

        <FormDatePickerField
          name="targetFrom"
          control={form.control}
          label="Dán vào lịch bắt đầu từ ngày"
          calendarDisabled={{ before: getBusinessTodayDate() }}
        />

        {targetRangeLabel && (
          <p className="border border-border bg-muted/40 px-3 py-2 text-sm text-muted-foreground">
            Lịch mới sẽ được tạo cho <span className="font-medium text-foreground">{targetRangeLabel}</span>. Suất trùng
            giờ với lịch đã có sẽ được bỏ qua, các suất đã hủy không được sao chép.
          </p>
        )}

        <FormSwitchField name="allowAfterHours" control={form.control} label="Cho phép suất khuya (kết thúc sau giờ đóng cửa)" />
      </FieldGroup>

      <FormActions
        isPending={clone.isPending}
        isDirty={form.formState.isDirty}
        submitLabel="Nhân bản"
        pendingLabel="Đang nhân bản..."
        onCancel={onSuccess}
      />
    </form>
  );
};
