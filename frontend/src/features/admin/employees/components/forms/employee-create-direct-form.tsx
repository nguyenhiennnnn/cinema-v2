import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";
import { FormDatePickerField } from "@/components/common/form/form-date-picker-field";
import { FormSearchSelectField } from "@/components/common/form/form-search-select-field";
import { FormActions } from "@/components/common/form/form-actions";
import { getBusinessToday, getBusinessTodayDate } from "@/lib/business-time";
import { strongPasswordSchema } from "@/lib/validators";
import { LOOKUP_PAGE_SIZE, NAME_MAX_LENGTH, VN_PHONE_REGEX, EMAIL_MAX_LENGTH } from "@/config/default-constants";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";

import { useAdminCreateEmployeeDirect } from "@/features/admin/employees/hooks/use-admin-employee";
import { useAdminPositions } from "@/features/admin/catalog/hooks/use-admin-position";
import { usePositionQuickCreate } from "@/features/admin/catalog/hooks/use-quick-create";

const createDirectFormSchema = z.object({
  fullName: z.string().trim().min(1, "Vui lòng nhập họ tên.").max(NAME_MAX_LENGTH, `Họ tên tối đa ${NAME_MAX_LENGTH} ký tự.`),
  email: z.string().trim().min(1, "Vui lòng nhập email.").email("Email không hợp lệ.").max(EMAIL_MAX_LENGTH),
  phone: z
    .string()
    .trim()
    .regex(VN_PHONE_REGEX, "Số điện thoại không hợp lệ.")
    .optional()
    .or(z.literal("")),
  positionId: z.coerce.number().optional(),
  joiningDate: z.string().optional().or(z.literal("")),
  temporaryPassword: z.string().min(1, "Vui lòng nhập mật khẩu tạm.").pipe(strongPasswordSchema),
});

type CreateDirectFormValues = z.infer<typeof createDirectFormSchema>;

interface EmployeeCreateDirectFormProps {
  onSuccess: () => void;
}

export const EmployeeCreateDirectForm = ({ onSuccess }: EmployeeCreateDirectFormProps) => {
  const createEmployee = useAdminCreateEmployeeDirect();
  const [positionSearch, setPositionSearch] = useState("");
  const positions = useAdminPositions({ pageSize: LOOKUP_PAGE_SIZE });
  const positionQuickCreate = usePositionQuickCreate();

  const form = useForm<CreateDirectFormValues>({
    resolver: zodResolver(createDirectFormSchema),
    defaultValues: {
      fullName: "",
      email: "",
      phone: "",
      positionId: undefined,
      joiningDate: getBusinessToday(),
      temporaryPassword: "",
    },
  });

  const applyServerErrors = useServerFieldErrors(form);

  const onSubmit = (values: CreateDirectFormValues) => {
    createEmployee.mutate(
      {
        fullName: values.fullName,
        email: values.email,
        phone: values.phone || null,
        positionId: values.positionId ? Number(values.positionId) : null,
        joiningDate: values.joiningDate || null,
        temporaryPassword: values.temporaryPassword,
      },
      {
        onSuccess: () => onSuccess(),
        onError: applyServerErrors,
      },
    );
  };

  return (
    <form id="employee-create-direct-form" onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
      <FieldGroup disabled={createEmployee.isPending}>
        <FormInputField
          name="fullName"
          control={form.control}
          label="Họ tên"
          autoComplete="off"
          placeholder="Nguyễn Văn A"
        />

        <FormInputField
          name="email"
          control={form.control}
          label="Email"
          type="email"
          autoComplete="off"
          placeholder="nhanvien@example.com"
        />

        <FormInputField
          name="phone"
          control={form.control}
          label="Số điện thoại (không bắt buộc)"
          autoComplete="off"
          placeholder="0901234567"
        />

        <FormSearchSelectField
          name="positionId"
          control={form.control}
          label="Vị trí (không bắt buộc)"
          placeholder="Chọn vị trí"
          options={positions.data?.items ?? []}
          getOptionValue={(o) => o.positionId}
          getOptionLabel={(o) => o.name}
          searchValue={positionSearch}
          onSearchChange={setPositionSearch}
          isLoading={positions.isLoading}
          {...positionQuickCreate}
        />

        <FormDatePickerField
          name="joiningDate"
          control={form.control}
          label="Ngày vào làm (không bắt buộc)"
          placeholder="Chọn ngày vào làm"
          calendarDisabled={{ after: getBusinessTodayDate() }}
        />

        <div className="space-y-1.5">
          <FormInputField
            name="temporaryPassword"
            control={form.control}
            label="Mật khẩu tạm"
            type="password"
            autoComplete="new-password"
          />
          <p className="text-xs text-muted-foreground">
            Nhân viên bắt buộc đổi mật khẩu ở lần đăng nhập đầu tiên.
          </p>
        </div>
      </FieldGroup>

      <FormActions
        isPending={createEmployee.isPending}
        isDirty={form.formState.isDirty}
        submitLabel="Tạo nhân viên"
        onCancel={onSuccess}
      />
    </form>
  );
};
