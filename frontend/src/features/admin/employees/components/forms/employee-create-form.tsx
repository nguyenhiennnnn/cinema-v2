import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { toast } from "@/components/ui/toast";
import { Button } from "@/components/ui/button";
import { Field, FieldError, FieldGroup, FieldLabel } from "@/components/ui/field";
import { SingleSelectField } from "@/components/common/form/single-select-field";
import { FormSelectField } from "@/components/common/form/form-select-field";
import { FormDatePickerField } from "@/components/common/form/form-date-picker-field";
import { getBusinessToday, getBusinessTodayDate } from "@/lib/business-time";

import { useAdminCreateEmployee } from "../../hooks/use-admin-employee";
import { useAdminPositions } from "@/features/admin/catalog/hooks/use-admin-position";
import { useAdminUsersSelect } from "@/features/admin/users/hooks/use-admin-users";
import { LOOKUP_PAGE_SIZE } from "@/config/default-constants";

const employeeFormSchema = z.object({
  userId: z.coerce
    .number({ message: "Vui lòng chọn người dùng." })
    .min(1, "Vui lòng chọn người dùng."),
  positionId: z.coerce.number().optional(),
  hireDate: z.string().optional().or(z.literal("")),
});

type EmployeeFormValues = z.infer<typeof employeeFormSchema>;

interface EmployeeCreateFormProps {
  onSuccess: () => void;
}

export const EmployeeCreateForm = ({ onSuccess }: EmployeeCreateFormProps) => {
  const createEmployee = useAdminCreateEmployee();
  const [userSearch, setUserSearch] = useState("");

  const adminUsers = useAdminUsersSelect({ search: userSearch, availableOnly: true });
  const positions = useAdminPositions({ pageSize: LOOKUP_PAGE_SIZE });

  const form = useForm<EmployeeFormValues>({
    resolver: zodResolver(employeeFormSchema),
    defaultValues: {
      userId: undefined,
      positionId: undefined,
      hireDate: getBusinessToday(),
    },
  });

  const onSubmit = async (values: EmployeeFormValues) => {
    await createEmployee.mutateAsync({
      userId: Number(values.userId),
      positionId: values.positionId ? Number(values.positionId) : null,
      hireDate: values.hireDate || null,
    }, {
      onSuccess: () => {
        toast.add({ title: "Đã thêm nhân viên mới.", type: "success" });
        onSuccess();
      }
    });
  };

  return (
    <form id="employee-create-form" onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
      <FieldGroup>
        <Controller
          name="userId"
          control={form.control}
          render={({ field, fieldState }) => (
            <Field data-invalid={fieldState.invalid}>
              <FieldLabel>Người dùng</FieldLabel>
              <SingleSelectField
                value={field.value}
                onChange={(v) => field.onChange(v)}
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
              {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
            </Field>
          )}
        />

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
          calendarDisabled={{ after: getBusinessTodayDate() }}
        />
      </FieldGroup>

      <div className="flex justify-end gap-2 pt-2">
        <Button type="button" variant="outline" onClick={onSuccess} disabled={createEmployee.isPending}>
          Hủy
        </Button>
        <Button type="submit" disabled={createEmployee.isPending}>
          {createEmployee.isPending ? "Đang lưu..." : "Thêm nhân viên"}
        </Button>
      </div>
    </form>
  );
};