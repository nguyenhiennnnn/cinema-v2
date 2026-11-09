import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import { FieldGroup } from "@/components/ui/field";
import { FormDatePickerField } from "@/components/common/form/form-date-picker-field";
import { getBusinessToday, getBusinessTodayDate } from "@/lib/business-time";

import { useAdminCreateEmployee } from "../../hooks/use-admin-employee";
import { useAdminPositions } from "@/features/admin/catalog/hooks/use-admin-position";
import { usePositionQuickCreate } from "@/features/admin/catalog/hooks/use-quick-create";
import { useDebouncedValue } from "@/hooks/use-debounced-value";
import { useAdminUsersSelect } from "@/features/admin/users/hooks/use-admin-users";
import { LOOKUP_PAGE_SIZE } from "@/config/default-constants";
import { FormSearchSelectField } from "@/components/common/form/form-search-select-field";

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
  const [positionSearch, setPositionSearch] = useState("");

  const adminUsers = useAdminUsersSelect({
    search: useDebouncedValue(userSearch),
    availableOnly: true,
  });
  const positions = useAdminPositions({ pageSize: LOOKUP_PAGE_SIZE });
  const positionQuickCreate = usePositionQuickCreate();

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
        onSuccess();
      }
    });
  };

  return (
    <form id="employee-create-form" onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
      <FieldGroup>
        <FormSearchSelectField
          name="userId"
          control={form.control}
          label="Người dùng"
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