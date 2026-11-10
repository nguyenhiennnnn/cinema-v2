import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";

import { useAdminCreatePosition } from "@/features/admin/catalog/hooks/use-admin-position";
import { FormActions } from "@/components/common/form/form-actions";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";

const positionFormSchema = z.object({
  name: z
    .string()
    .min(1, "Vui lòng nhập tên chức vụ.")
    .max(100, "Tên chức vụ tối đa 100 ký tự."),
});

type PositionFormValues = z.infer<typeof positionFormSchema>;

interface PositionCreateFormProps {
  onSuccess: () => void;
}

export const PositionCreateForm = ({ onSuccess }: PositionCreateFormProps) => {
  const createPosition = useAdminCreatePosition();

  const form = useForm<PositionFormValues>({
    resolver: zodResolver(positionFormSchema),
    defaultValues: { name: "" },
  });

  const applyServerErrors = useServerFieldErrors(form);

  const onSubmit = (values: PositionFormValues) => {
    createPosition.mutate({ name: values.name }, {
      onSuccess: () => onSuccess(),
      onError: applyServerErrors,
    });
  };

  return (
    <form
      id="position-create-form"
      onSubmit={form.handleSubmit(onSubmit)}
      className="space-y-4"
    >
      <FieldGroup disabled={createPosition.isPending}>
        <FormInputField
          name="name"
          control={form.control}
          label="Tên chức vụ"
          placeholder="Quản lý rạp, Nhân viên soát vé..."
          autoFocus
          autoComplete="off"
        />
      </FieldGroup>

      <FormActions
        isPending={createPosition.isPending}
        isDirty={form.formState.isDirty}
        submitLabel="Thêm chức vụ"
        onCancel={onSuccess}
      />
    </form>
  );
};