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
import { LOOKUP_PAGE_SIZE } from "@/config/default-constants";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";

import { useAdminInviteEmployee } from "@/features/admin/employees/hooks/use-admin-employee";
import { useAdminPositions } from "@/features/admin/catalog/hooks/use-admin-position";
import { usePositionQuickCreate } from "@/features/admin/catalog/hooks/use-quick-create";

const inviteFormSchema = z.object({
  fullName: z.string().trim().min(1, "Vui lòng nhập họ tên.").max(100, "Họ tên tối đa 100 ký tự."),
  email: z.string().trim().min(1, "Vui lòng nhập email.").email("Email không hợp lệ.").max(150),
  phone: z
    .string()
    .trim()
    .regex(/^(0|\+84)[0-9]{9,10}$/, "Số điện thoại không hợp lệ.")
    .optional()
    .or(z.literal("")),
  positionId: z.coerce.number().optional(),
  joiningDate: z.string().optional().or(z.literal("")),
});

type InviteFormValues = z.infer<typeof inviteFormSchema>;

interface EmployeeInviteFormProps {
  onSuccess: () => void;
}

export const EmployeeInviteForm = ({ onSuccess }: EmployeeInviteFormProps) => {
  const inviteEmployee = useAdminInviteEmployee();
  const [positionSearch, setPositionSearch] = useState("");
  const positions = useAdminPositions({ pageSize: LOOKUP_PAGE_SIZE });
  const positionQuickCreate = usePositionQuickCreate();

  const form = useForm<InviteFormValues>({
    resolver: zodResolver(inviteFormSchema),
    defaultValues: {
      fullName: "",
      email: "",
      phone: "",
      positionId: undefined,
      joiningDate: getBusinessToday(),
    },
  });

  const applyServerErrors = useServerFieldErrors(form);

  const onSubmit = (values: InviteFormValues) => {
    inviteEmployee.mutate(
      {
        fullName: values.fullName,
        email: values.email,
        phone: values.phone || null,
        positionId: values.positionId ? Number(values.positionId) : null,
        joiningDate: values.joiningDate || null,
      },
      {
        onSuccess: () => onSuccess(),
        onError: applyServerErrors,
      },
    );
  };

  return (
    <form id="employee-invite-form" onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
      <FieldGroup disabled={inviteEmployee.isPending}>
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
      </FieldGroup>

      <FormActions
        isPending={inviteEmployee.isPending}
        isDirty={form.formState.isDirty}
        submitLabel="Gửi lời mời"
        onCancel={onSuccess}
      />
    </form>
  );
};
