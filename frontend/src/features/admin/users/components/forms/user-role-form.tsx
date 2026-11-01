import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import { Field, FieldError, FieldGroup, FieldLabel } from "@/components/ui/field";
import { EnumSelectFilter } from "@/components/common/filter/enum-select-filter";
import { roleOptions } from "@/lib/enum-options";

import type { AdminUserListItemDto } from "../../types";
import { useAdminUpdateUserRole } from "../../hooks/use-admin-users";

const roleFormSchema = z.object({
  roleId: z.string().min(1, "Vui lòng chọn vai trò."),
});

type RoleFormValues = z.infer<typeof roleFormSchema>;

interface UserRoleFormProps {
  user: AdminUserListItemDto;
  onSuccess: () => void;
}

export const UserRoleForm = ({ user, onSuccess }: UserRoleFormProps) => {
  const updateRole = useAdminUpdateUserRole();

  const form = useForm<RoleFormValues>({
    resolver: zodResolver(roleFormSchema),
    defaultValues: { roleId: String(user.roleId) },
  });

  const onSubmit = async (values: RoleFormValues) => {
    await updateRole.mutateAsync({
      id: user.userId,
      body: { roleId: Number(values.roleId) },
    }, {
      onSuccess: () => {
        onSuccess();
      }
    });
  };

  return (
    <form id="user-role-form" onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
      <FieldGroup>
        <Controller
          name="roleId"
          control={form.control}
          render={({ field, fieldState }) => (
            <Field data-invalid={fieldState.invalid}>
              <FieldLabel>Vai trò</FieldLabel>
              <EnumSelectFilter
                value={field.value}
                onChange={(v) => field.onChange(v ?? "")}
                options={roleOptions}
                placeholder="Chọn vai trò"
                allLabel="Chọn vai trò"
              />
              {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
            </Field>
          )}
        />
      </FieldGroup>

      <div className="flex justify-end gap-2 pt-2">
        <Button type="button" variant="outline" onClick={onSuccess} disabled={updateRole.isPending}>
          Hủy
        </Button>
        <Button type="submit" disabled={updateRole.isPending}>
          {updateRole.isPending ? "Đang lưu..." : "Lưu thay đổi"}
        </Button>
      </div>
    </form>
  );
};