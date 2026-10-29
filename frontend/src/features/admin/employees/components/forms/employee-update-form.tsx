import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { toast } from "@/components/ui/toast";
import { Button } from "@/components/ui/button";
import { Field, FieldGroup, FieldLabel } from "@/components/ui/field";
import { Switch } from "@/components/ui/switch";
import { FormSelectField } from "@/components/common/form/form-select-field";
import { FormDatePickerField } from "@/components/common/form/form-date-picker-field";

import { useAdminUpdateEmployee } from "../../hooks/use-admin-employee";
import { useAdminPositions } from "@/features/admin/catalog/hooks/use-admin-position";
import type { AdminEmployeeDto } from "../../types";
import { LOOKUP_PAGE_SIZE } from "@/config/default-constants";

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
  const positions = useAdminPositions({ pageSize: LOOKUP_PAGE_SIZE });

  const form = useForm<EmployeeFormValues>({
    resolver: zodResolver(employeeFormSchema),
    defaultValues: {
      positionId: employee.positionId ?? undefined,
      hireDate: employee.hireDate ?? "",
      isActive: employee.isActive,
    },
  });

  const onSubmit = async (values: EmployeeFormValues) => {
    await updateEmployee.mutateAsync({
      id: employee.employeeId,
      body: {
        positionId: values.positionId ? Number(values.positionId) : null,
        hireDate: values.hireDate || null,
        isActive: values.isActive,
      },
    }, {
      onSuccess: () => {
        toast.add({ title: "Đã cập nhật nhân viên.", type: "success" });
        onSuccess();
      }
    });
  };

  return (
    <form id="employee-update-form" onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
      <div className="border bg-muted/40 p-3 space-y-0.5">
        <p className="text-sm font-medium">{employee.fullName}</p>
        <p className="text-xs text-muted-foreground">{employee.email}</p>
      </div>

      <FieldGroup>
        <FormSelectField
          name="positionId"
          control={form.control}
          label="Vị trí (không bắt buộc)"
          placeholder="Chọn vị trí"
          options={positions.data?.items ?? []}
          getOptionValue={(o) => o.positionId}
          getOptionLabel={(o) => o.name}
          disabled={positions.isLoading}
        />

        <FormDatePickerField
          name="hireDate"
          control={form.control}
          label="Ngày vào làm (không bắt buộc)"
          placeholder="Chọn ngày vào làm"
          calendarDisabled={{ after: new Date() }}
        />

        <Controller
          name="isActive"
          control={form.control}
          render={({ field }) => (
            <Field className="flex flex-row items-center justify-between">
              <FieldLabel>Đang làm việc</FieldLabel>
              <Switch checked={field.value} onCheckedChange={field.onChange} />
            </Field>
          )}
        />
      </FieldGroup>

      <div className="flex justify-end gap-2 pt-2">
        <Button type="button" variant="outline" onClick={onSuccess} disabled={updateEmployee.isPending}>
          Hủy
        </Button>
        <Button type="submit" disabled={updateEmployee.isPending}>
          {updateEmployee.isPending ? "Đang lưu..." : "Lưu thay đổi"}
        </Button>
      </div>
    </form>
  );
};