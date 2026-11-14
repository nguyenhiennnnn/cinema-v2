import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { FieldGroup } from "@/components/ui/field";
import { FormDatePickerField } from "@/components/common/form/form-date-picker-field";
import { getBusinessToday, getBusinessTodayDate } from "@/lib/business-time";

import { useAdminCreateEmployee } from "@/features/admin/employees/hooks/use-admin-employee";
import { useAdminPositions } from "@/features/admin/catalog/hooks/use-admin-position";
import { usePositionQuickCreate } from "@/features/admin/catalog/hooks/use-quick-create";
import { useDebouncedValue } from "@/hooks/use-debounced-value";
import { useAdminUsersSelect } from "@/features/admin/users/hooks/use-admin-users";
import { LOOKUP_PAGE_SIZE } from "@/config/default-constants";
import { FormSearchSelectField } from "@/components/common/form/form-search-select-field";
import { FormActions } from "@/components/common/form/form-actions";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";

const fromAccountFormSchema = z.object({
  userId: z.coerce
    .number({ message: "Vui lòng chọn người dùng." })
    .min(1, "Vui lòng chọn người dùng."),
  positionId: z.coerce.number().optional(),
  hireDate: z.string().optional().or(z.literal("")),
});

type FromAccountFormValues = z.infer<typeof fromAccountFormSchema>;

interface EmployeeFromAccountFormProps {
  onSuccess: () => void;
}

export const EmployeeFromAccountForm = ({ onSuccess }: EmployeeFromAccountFormProps) => {
  const createEmployee = useAdminCreateEmployee();
  const [userSearch, setUserSearch] = useState("");
  const [positionSearch, setPositionSearch] = useState("");

  const adminUsers = useAdminUsersSelect({
    search: useDebouncedValue(userSearch),
    availableOnly: true,
    includeCustomers: true,
  });
  const positions = useAdminPositions({ pageSize: LOOKUP_PAGE_SIZE });
  const positionQuickCreate = usePositionQuickCreate();

  const form = useForm<FromAccountFormValues>({
    resolver: zodResolver(fromAccountFormSchema),
    defaultValues: {
      userId: undefined,
      positionId: undefined,
      hireDate: getBusinessToday(),
    },
  });

  const applyServerErrors = useServerFieldErrors(form);

  const onSubmit = (values: FromAccountFormValues) => {
    createEmployee.mutate({
      userId: Number(values.userId),
      positionId: values.positionId ? Number(values.positionId) : null,
      hireDate: values.hireDate || null,
      promoteCustomer: true,
    }, {
      onSuccess: () => onSuccess(),
      onError: applyServerErrors,
    });
  };

  return (
    <form id="employee-from-account-form" onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
      <FieldGroup disabled={createEmployee.isPending}>
        <FormSearchSelectField
          name="userId"
          control={form.control}
          label="Tài khoản có sẵn (nhân viên, quản trị viên hoặc khách hàng)"
          options={adminUsers.data ?? []}
          getOptionValue={(o) => o.userId}
          getOptionLabel={(o) => `${o.fullName} (${o.email})`}
          searchValue={userSearch}
          onSearchChange={setUserSearch}
          isLoading={adminUsers.isLoading}
          placeholder="Chọn người dùng"
          searchPlaceholder="Tìm theo tên hoặc email..."
          emptyText="Không tìm thấy người dùng phù hợp."
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
          name="hireDate"
          control={form.control}
          label="Ngày vào làm (không bắt buộc)"
          placeholder="Chọn ngày vào làm"
          calendarDisabled={{ after: getBusinessTodayDate() }}
        />
      </FieldGroup>

      <FormActions
        isPending={createEmployee.isPending}
        isDirty={form.formState.isDirty}
        submitLabel="Thêm từ tài khoản"
        onCancel={onSuccess}
      />
    </form>
  );
};
