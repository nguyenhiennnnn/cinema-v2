import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useEffect } from "react";

import { PageHeader } from "@/components/common/page-header";
import { toast } from "@/components/ui/toast";
import { Button } from "@/components/ui/button";
import { Field, FieldError, FieldGroup, FieldLabel } from "@/components/ui/field";
import { Skeleton } from "@/components/ui/skeleton";
import { FormInputField } from "@/components/common/form/form-input-field";
import { FormTimePickerField } from "@/components/common/form/form-time-picker-field";
import { ImageUploader } from "@/features/admin/uploads/components/image-uploader";

import { useAdminSettings, useAdminUpdateSettings } from "../hooks/use-admin-settings";

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
  cancelRefundPercent: z.coerce.number().min(0).max(100, "Tối đa 100%."),
  cancelDeadlineHours: z.coerce.number().min(0, "Giá trị không được âm."),
});

type SettingsFormValues = z.infer<typeof settingsFormSchema>;

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
    },
  });

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
      });
    }
  }, [settings.data, form]);

  const onSubmit = async (values: SettingsFormValues) => {
    await updateSettings.mutateAsync({
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
    }, {
      onSuccess: () => {
        toast.add({ title: "Đã cập nhật cấu hình rạp.", type: "success" });
      }
    });
  };

  if (settings.isLoading) {
    return (
      <div>
        <PageHeader title="Cấu hình rạp" description="Thông tin chung, giờ hoạt động và chính sách." />
        <Skeleton className="h-64 w-full rounded-xl" />
      </div>
    );
  }

  return (
    <div>
      <PageHeader title="Cấu hình rạp" description="Thông tin chung, giờ hoạt động và chính sách." />

      <form onSubmit={form.handleSubmit(onSubmit)} className="max-w-2xl space-y-4">
        <FieldGroup>
          <Controller
            name="logoUrl"
            control={form.control}
            render={({ field, fieldState }) => (
              <Field data-invalid={fieldState.invalid}>
                <FieldLabel>Logo rạp</FieldLabel>
                <ImageUploader
                  folder="settings"
                  value={field.value}
                  onChange={(result) => field.onChange(result?.url ?? "")}
                />
                {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
              </Field>
            )}
          />

          <FormInputField name="name" control={form.control} label="Tên rạp" />
          <FormInputField name="address" control={form.control} label="Địa chỉ" />

          <div className="grid grid-cols-2 gap-4">
            <FormInputField name="phone" control={form.control} label="Số điện thoại" />
            <FormInputField name="email" control={form.control} label="Email" />
          </div>

          <FormInputField name="taxCode" control={form.control} label="Mã số thuế" />

          <div className="grid grid-cols-2 gap-4">
            <FormTimePickerField
              name="openingTime"
              control={form.control}
              label="Giờ mở cửa"
            />
            <FormTimePickerField
              name="closingTime"
              control={form.control}
              label="Giờ đóng cửa"
            />
          </div>

          <div className="grid grid-cols-3 gap-4">
            <FormInputField
              name="pointsPerAmount"
              control={form.control}
              label="Điểm / VND"
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
          </div>
        </FieldGroup>

        <div className="flex justify-end pt-2">
          <Button type="submit" disabled={updateSettings.isPending}>
            {updateSettings.isPending ? "Đang lưu..." : "Lưu cấu hình"}
          </Button>
        </div>
      </form>
    </div>
  );
};

export default AdminSettingsPage;