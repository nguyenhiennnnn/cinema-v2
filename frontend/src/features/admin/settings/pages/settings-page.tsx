import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useEffect, type ReactNode } from "react";

import { PageHeader } from "@/components/common/page-header";
import { FieldGroup } from "@/components/ui/field";
import { Skeleton } from "@/components/ui/skeleton";
import { FormInputField } from "@/components/common/form/form-input-field";
import { SubmitButton } from "@/components/common/form/submit-button";
import { FormSelectField } from "@/components/common/form/form-select-field";
import { FormTimePickerField } from "@/components/common/form/form-time-picker-field";

import { FORM_PENDING_LABEL, PERCENT_MAX, SETTINGS_FIELD_MAX } from "@/config/default-constants";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";
import { SeatGapRuleMode } from "@/types/enum";
import { useAdminSettings, useAdminUpdateSettings } from "../hooks/use-admin-settings";
import { FormImageField } from "@/components/common/form/form-upload-field";
import { getCurrencyLabel } from "@/lib/regional-settings";

const SEAT_GAP_RULE_OPTIONS = [
  { value: SeatGapRuleMode.Strict, label: "Chặn mọi ghế lẻ" },
  { value: SeatGapRuleMode.AllowEdge, label: "Cho phép ghế lẻ sát mép hàng / lối đi" },
  { value: SeatGapRuleMode.Off, label: "Tắt quy tắc ghế lẻ" },
];

const settingsFormSchema = z.object({
  name: z.string().min(1, "Vui lòng nhập tên rạp."),
  address: z.string().min(1, "Vui lòng nhập địa chỉ."),
  phone: z.string().optional().or(z.literal("")),
  email: z.string().email("Email không hợp lệ.").optional().or(z.literal("")),
  taxCode: z.string().optional().or(z.literal("")),
  logoUrl: z.string().optional().or(z.literal("")),
  openingTime: z.string().optional().or(z.literal("")),
  closingTime: z.string().optional().or(z.literal("")),
  pointsPerAmount: z.coerce.number().min(0, "Giá trị không được âm."),
  cancelRefundPercent: z.coerce.number().min(0).max(PERCENT_MAX, `Tối đa ${PERCENT_MAX}%.`),
  cancelDeadlineHours: z.coerce.number().min(0, "Giá trị không được âm."),
  stopSellingMinutesBefore: z.coerce.number().min(0, "Giá trị không được âm.").max(SETTINGS_FIELD_MAX.stopSellingMinutesBefore, `Tối đa ${SETTINGS_FIELD_MAX.stopSellingMinutesBefore} phút.`),
  seatGapRuleMode: z.enum([SeatGapRuleMode.Strict, SeatGapRuleMode.AllowEdge, SeatGapRuleMode.Off]),
  roomBufferMinutes: z.coerce.number().min(0, "Tối thiểu 0.").max(SETTINGS_FIELD_MAX.roomBufferMinutes, `Tối đa ${SETTINGS_FIELD_MAX.roomBufferMinutes}.`),
  maxSeatsPerBooking: z.coerce.number().min(1, "Tối thiểu 1.").max(SETTINGS_FIELD_MAX.maxSeatsPerBooking, `Tối đa ${SETTINGS_FIELD_MAX.maxSeatsPerBooking}.`),
  maxComboQuantity: z.coerce.number().min(1, "Tối thiểu 1.").max(SETTINGS_FIELD_MAX.maxComboQuantity, `Tối đa ${SETTINGS_FIELD_MAX.maxComboQuantity}.`),
  loyaltyWindowMonths: z.coerce.number().min(1, "Tối thiểu 1.").max(SETTINGS_FIELD_MAX.loyaltyWindowMonths, `Tối đa ${SETTINGS_FIELD_MAX.loyaltyWindowMonths}.`),
  movieEndGraceDays: z.coerce.number().min(0, "Tối thiểu 0.").max(SETTINGS_FIELD_MAX.movieEndGraceDays, `Tối đa ${SETTINGS_FIELD_MAX.movieEndGraceDays}.`),
  showtimeLookaheadDays: z.coerce.number().min(1, "Tối thiểu 1.").max(SETTINGS_FIELD_MAX.showtimeLookaheadDays, `Tối đa ${SETTINGS_FIELD_MAX.showtimeLookaheadDays}.`),
  schedulerStepMinutes: z.coerce.number().min(1, "Tối thiểu 1.").max(SETTINGS_FIELD_MAX.schedulerStepMinutes, `Tối đa ${SETTINGS_FIELD_MAX.schedulerStepMinutes}.`),
  schedulerMinSameMovieGapMinutes: z.coerce.number().min(0, "Tối thiểu 0.").max(SETTINGS_FIELD_MAX.schedulerMinSameMovieGapMinutes, `Tối đa ${SETTINGS_FIELD_MAX.schedulerMinSameMovieGapMinutes}.`),
  schedulerHeatLookbackDays: z.coerce.number().min(1, "Tối thiểu 1.").max(SETTINGS_FIELD_MAX.schedulerHeatLookbackDays, `Tối đa ${SETTINGS_FIELD_MAX.schedulerHeatLookbackDays}.`),
  schedulerHeatSalesWeight: z.coerce.number().min(0, "Tối thiểu 0.").max(SETTINGS_FIELD_MAX.schedulerHeatSalesWeight, `Tối đa ${SETTINGS_FIELD_MAX.schedulerHeatSalesWeight}.`),
  schedulerHeatRatingWeight: z.coerce.number().min(0, "Tối thiểu 0.").max(SETTINGS_FIELD_MAX.schedulerHeatRatingWeight, `Tối đa ${SETTINGS_FIELD_MAX.schedulerHeatRatingWeight}.`),
  schedulerMaxRating: z.coerce.number().min(1, "Tối thiểu 1.").max(SETTINGS_FIELD_MAX.schedulerMaxRating, `Tối đa ${SETTINGS_FIELD_MAX.schedulerMaxRating}.`),
});

type SettingsFormValues = z.infer<typeof settingsFormSchema>;

const SettingsSection = ({
  title,
  disabled,
  children,
}: {
  title: string;
  disabled: boolean;
  children: ReactNode;
}) => (
  <section className="bg-card p-5">
    <h2 className="font-heading text-base font-semibold text-foreground">{title}</h2>
    <FieldGroup
      disabled={disabled}
      className="mt-4 grid items-start gap-x-4 gap-y-4 sm:grid-cols-2"
    >
      {children}
    </FieldGroup>
  </section>
);

const AdminSettingsPage = () => {
  const settings = useAdminSettings();
  const updateSettings = useAdminUpdateSettings();

  const form = useForm<SettingsFormValues>({
    resolver: zodResolver(settingsFormSchema),
    defaultValues: {
      name: "",
      address: "",
      phone: "",
      email: "",
      taxCode: "",
      logoUrl: "",
      openingTime: "",
      closingTime: "",
      pointsPerAmount: 0,
      cancelRefundPercent: 0,
      cancelDeadlineHours: 0,
      stopSellingMinutesBefore: 0,
      seatGapRuleMode: SeatGapRuleMode.Strict,
      roomBufferMinutes: 15,
      maxSeatsPerBooking: 10,
      maxComboQuantity: 10,
      loyaltyWindowMonths: 12,
      movieEndGraceDays: 1,
      showtimeLookaheadDays: 7,
      schedulerStepMinutes: 5,
      schedulerMinSameMovieGapMinutes: 60,
      schedulerHeatLookbackDays: 7,
      schedulerHeatSalesWeight: 0.7,
      schedulerHeatRatingWeight: 0.3,
      schedulerMaxRating: 5,
    },
  });

  const applyServerErrors = useServerFieldErrors(form);

  useEffect(() => {
    if (settings.data) {
      form.reset({
        name: settings.data.name,
        address: settings.data.address,
        phone: settings.data.phone ?? "",
        email: settings.data.email ?? "",
        taxCode: settings.data.taxCode ?? "",
        logoUrl: settings.data.logoUrl ?? "",
        openingTime: settings.data.openingTime ?? "",
        closingTime: settings.data.closingTime ?? "",
        pointsPerAmount: settings.data.pointsPerAmount,
        cancelRefundPercent: settings.data.cancelRefundPercent,
        cancelDeadlineHours: settings.data.cancelDeadlineHours,
        stopSellingMinutesBefore: settings.data.stopSellingMinutesBefore,
        seatGapRuleMode: settings.data.seatGapRuleMode,
        roomBufferMinutes: settings.data.roomBufferMinutes,
        maxSeatsPerBooking: settings.data.maxSeatsPerBooking,
        maxComboQuantity: settings.data.maxComboQuantity,
        loyaltyWindowMonths: settings.data.loyaltyWindowMonths,
        movieEndGraceDays: settings.data.movieEndGraceDays,
        showtimeLookaheadDays: settings.data.showtimeLookaheadDays,
        schedulerStepMinutes: settings.data.schedulerStepMinutes,
        schedulerMinSameMovieGapMinutes: settings.data.schedulerMinSameMovieGapMinutes,
        schedulerHeatLookbackDays: settings.data.schedulerHeatLookbackDays,
        schedulerHeatSalesWeight: settings.data.schedulerHeatSalesWeight,
        schedulerHeatRatingWeight: settings.data.schedulerHeatRatingWeight,
        schedulerMaxRating: settings.data.schedulerMaxRating,
      });
    }
  }, [settings.data, form]);

  const onSubmit = (values: SettingsFormValues) => {
    updateSettings.mutate(
      {
        name: values.name,
        address: values.address,
        phone: values.phone || null,
        email: values.email || null,
        taxCode: values.taxCode || null,
        logoUrl: values.logoUrl || null,
        openingTime: values.openingTime || null,
        closingTime: values.closingTime || null,
        pointsPerAmount: values.pointsPerAmount,
        cancelRefundPercent: values.cancelRefundPercent,
        cancelDeadlineHours: values.cancelDeadlineHours,
        stopSellingMinutesBefore: values.stopSellingMinutesBefore,
        seatGapRuleMode: values.seatGapRuleMode,
        roomBufferMinutes: values.roomBufferMinutes,
        maxSeatsPerBooking: values.maxSeatsPerBooking,
        maxComboQuantity: values.maxComboQuantity,
        loyaltyWindowMonths: values.loyaltyWindowMonths,
        movieEndGraceDays: values.movieEndGraceDays,
        showtimeLookaheadDays: values.showtimeLookaheadDays,
        schedulerStepMinutes: values.schedulerStepMinutes,
        schedulerMinSameMovieGapMinutes: values.schedulerMinSameMovieGapMinutes,
        schedulerHeatLookbackDays: values.schedulerHeatLookbackDays,
        schedulerHeatSalesWeight: values.schedulerHeatSalesWeight,
        schedulerHeatRatingWeight: values.schedulerHeatRatingWeight,
        schedulerMaxRating: values.schedulerMaxRating,
      },
      { onError: applyServerErrors },
    );
  };

  if (settings.isLoading) {
    return (
      <div>
        <PageHeader title="Cấu hình rạp" description="Thông tin chung, giờ hoạt động và chính sách." />
        <Skeleton className="h-64 w-full max-w-4xl" />
      </div>
    );
  }

  return (
    <div>
      <PageHeader title="Cấu hình rạp" description="Thông tin chung, giờ hoạt động và chính sách." />

      <form onSubmit={form.handleSubmit(onSubmit)} className="max-w-4xl space-y-6">
        <SettingsSection title="Thông tin rạp" disabled={updateSettings.isPending}>
          <FormImageField
            name="logoUrl"
            control={form.control}
            label="Logo rạp"
            folder="settings"
            className="sm:col-span-2"
          />
          <FormInputField name="name" control={form.control} label="Tên rạp" />
          <FormInputField name="taxCode" control={form.control} label="Mã số thuế" />
          <div className="sm:col-span-2">
            <FormInputField name="address" control={form.control} label="Địa chỉ" />
          </div>
          <FormInputField name="phone" control={form.control} label="Số điện thoại" />
          <FormInputField name="email" control={form.control} label="Email" />
        </SettingsSection>

        <SettingsSection
          title="Giờ hoạt động & chính sách bán vé"
          disabled={updateSettings.isPending}
        >
          <FormTimePickerField name="openingTime" control={form.control} label="Giờ mở cửa" />
          <FormTimePickerField name="closingTime" control={form.control} label="Giờ đóng cửa" />
          <FormInputField
            name="pointsPerAmount"
            control={form.control}
            label={`Điểm / ${getCurrencyLabel()}`}
            type="number"
            min={0}
          />
          <FormInputField
            name="cancelRefundPercent"
            control={form.control}
            label="% hoàn vé"
            type="number"
            min={0}
            max={100}
          />
          <FormInputField
            name="cancelDeadlineHours"
            control={form.control}
            label="Hạn hủy (giờ)"
            type="number"
            min={0}
          />
          <FormInputField
            name="stopSellingMinutesBefore"
            control={form.control}
            label="Ngừng bán online trước giờ chiếu (phút)"
            type="number"
            min={0}
            max={1440}
          />
          <div className="sm:col-span-2">
            <FormSelectField
              name="seatGapRuleMode"
              control={form.control}
              label="Quy tắc ghế lẻ (bán online)"
              options={SEAT_GAP_RULE_OPTIONS}
              getOptionValue={(o) => o.value}
              getOptionLabel={(o) => o.label}
            />
          </div>
        </SettingsSection>

        <SettingsSection title="Lịch chiếu & đặt vé" disabled={updateSettings.isPending}>
          <FormInputField
            name="roomBufferMinutes"
            control={form.control}
            label="Khoảng nghỉ giữa các suất (phút)"
            type="number"
            min={0}
            max={120}
          />
          <FormInputField
            name="maxSeatsPerBooking"
            control={form.control}
            label="Số ghế tối đa / booking"
            type="number"
            min={1}
            max={50}
          />
          <FormInputField
            name="maxComboQuantity"
            control={form.control}
            label="Số lượng tối đa mỗi combo"
            type="number"
            min={1}
            max={50}
          />
          <FormInputField
            name="loyaltyWindowMonths"
            control={form.control}
            label="Hiệu lực điểm thưởng (tháng)"
            type="number"
            min={1}
            max={120}
          />
          <FormInputField
            name="movieEndGraceDays"
            control={form.control}
            label="Gia hạn phim Ended sau suất cuối (ngày)"
            type="number"
            min={0}
            max={365}
          />
          <FormInputField
            name="showtimeLookaheadDays"
            control={form.control}
            label="Số ngày lịch chiếu hiển thị"
            type="number"
            min={1}
            max={60}
          />
        </SettingsSection>

        <SettingsSection title="Xếp lịch tự động & Heat" disabled={updateSettings.isPending}>
          <FormInputField
            name="schedulerStepMinutes"
            control={form.control}
            label="Xếp lịch: bước dò giờ (phút)"
            type="number"
            min={1}
            max={60}
          />
          <FormInputField
            name="schedulerMinSameMovieGapMinutes"
            control={form.control}
            label="Xếp lịch: giãn cách cùng phim (phút)"
            type="number"
            min={0}
            max={1440}
          />
          <FormInputField
            name="schedulerHeatLookbackDays"
            control={form.control}
            label="Heat: số ngày tính doanh số vé"
            type="number"
            min={1}
            max={365}
          />
          <FormInputField
            name="schedulerMaxRating"
            control={form.control}
            label="Heat: điểm đánh giá tối đa"
            type="number"
            step={0.5}
            min={1}
            max={10}
          />
          <FormInputField
            name="schedulerHeatSalesWeight"
            control={form.control}
            label="Heat: trọng số doanh số"
            type="number"
            step={0.05}
            min={0}
            max={10}
          />
          <FormInputField
            name="schedulerHeatRatingWeight"
            control={form.control}
            label="Heat: trọng số đánh giá"
            type="number"
            step={0.05}
            min={0}
            max={10}
          />
        </SettingsSection>

        <div className="flex justify-end">
          <SubmitButton
            isPending={updateSettings.isPending}
            label="Lưu cấu hình"
            pendingLabel={FORM_PENDING_LABEL}
          />
        </div>
      </form>
    </div>
  );
};

export default AdminSettingsPage;