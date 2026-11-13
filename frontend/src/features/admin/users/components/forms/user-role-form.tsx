import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { FieldGroup } from "@/components/ui/field";
import { roleOptions } from "@/lib/enum-options";

import type { AdminUserListItemDto } from "@/features/admin/users/types";
import { useAdminUpdateUserRole } from "@/features/admin/users/hooks/use-admin-users";
import { FormEnumSelectField } from "@/components/common/form/form-enum-select-field";
import { FormActions } from "@/components/common/form/form-actions";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";

import { FORM_SAVE_CHANGES_LABEL } from "@/config/default-constants";

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

  const applyServerErrors = useServerFieldErrors(form);

  const onSubmit = (values: RoleFormValues) => {
    updateRole.mutate({
      id: user.userId,
      body: { roleId: Number(values.roleId) },
    }, {
      onSuccess: () => onSuccess(),
      onError: applyServerErrors,
    });
  };

  return (
    <form id="user-role-form" onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
      <FieldGroup disabled={updateRole.isPending}>
        <FormEnumSelectField
          name="roleId"
          control={form.control}
          label="Vai trò"
          options={roleOptions}
          placeholder="Chọn vai trò"
        />
      </FieldGroup>

      <FormActions
        isPending={updateRole.isPending}
        isDirty={form.formState.isDirty}
        submitLabel={FORM_SAVE_CHANGES_LABEL}
        onCancel={onSuccess}
      />
    </form>
  );
};