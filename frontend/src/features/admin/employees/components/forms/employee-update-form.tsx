import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { FieldGroup } from "@/components/ui/field";
import { FormSearchSelectField } from "@/components/common/form/form-search-select-field";
import { FormDatePickerField } from "@/components/common/form/form-date-picker-field";

import { useAdminUpdateEmployee } from "@/features/admin/employees/hooks/use-admin-employee";
import { useAdminPositions } from "@/features/admin/catalog/hooks/use-admin-position";
import { usePositionQuickCreate } from "@/features/admin/catalog/hooks/use-quick-create";
import type { AdminEmployeeDto } from "@/features/admin/employees/types";
import { LOOKUP_PAGE_SIZE } from "@/config/default-constants";
import { getBusinessTodayDate } from "@/lib/business-time";
import { FormSwitchField } from "@/components/common/form/form-switch-field";
import { FormActions } from "@/components/common/form/form-actions";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";

const employeeFormSchema = z.object({
  positionId: z.coerce.number().optional(),
  hireDate: z.string().optional().or(z.literal("")),
  isActive: z.boolean(),
});

type EmployeeFormValues = z.infer<typeof employeeFormSchema>;

interface EmployeeUpdateFormProps {
  employee: AdminEmployeeDto;
  onSuccess: () => void;
}

export const EmployeeUpdateForm = ({ employee, onSuccess }: EmployeeUpdateFormProps) => {
  const updateEmployee = useAdminUpdateEmployee();
  const [positionSearch, setPositionSearch] = useState("");
  const positions = useAdminPositions({ pageSize: LOOKUP_PAGE_SIZE });
  const positionQuickCreate = usePositionQuickCreate();

  const form = useForm<EmployeeFormValues>({
    resolver: zodResolver(employeeFormSchema),
    defaultValues: {
      positionId: employee.positionId ?? undefined,
      hireDate: employee.hireDate ?? "",
      isActive: employee.isActive,
    },
  });

  const applyServerErrors = useServerFieldErrors(form);

  const onSubmit = (values: EmployeeFormValues) => {
    updateEmployee.mutate({
      id: employee.employeeId,
      body: {
        positionId: values.positionId ? Number(values.positionId) : null,
        hireDate: values.hireDate || null,
        isActive: values.isActive,
      },
    }, {
      onSuccess: () => onSuccess(),
      onError: applyServerErrors,
    });
  };

  return (
    <form id="employee-update-form" onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
      <div className="border bg-muted/40 p-3 space-y-0.5">
        <p className="text-sm font-medium">{employee.fullName}</p>
        <p className="text-xs text-muted-foreground">{employee.email}</p>
      </div>

      <FieldGroup disabled={updateEmployee.isPending}>
        <FormSearchSelectField
          name="positionId"
          control={form.control}
          label="Vị trí (không bắt buộc)"
          placeholder="Chọn vị trí"
          options={positions.data?.items ?? []}
          selectedLabel={employee.positionName ?? undefined}
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

        <FormSwitchField name="isActive" control={form.control} label="Đang làm việc" />
      </FieldGroup>

      <FormActions
        isPending={updateEmployee.isPending}
        isDirty={form.formState.isDirty}
        submitLabel="Lưu thay đổi"
        onCancel={onSuccess}
      />
    </form>
  );
};