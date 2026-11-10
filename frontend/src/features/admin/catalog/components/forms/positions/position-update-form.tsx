import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";

import { useAdminUpdatePosition } from "@/features/admin/catalog/hooks/use-admin-position";
import type { PositionDto } from "@/features/admin/catalog/types";
import { FormActions } from "@/components/common/form/form-actions";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";

const positionFormSchema = z.object({
  name: z
    .string()
    .min(1, "Vui lòng nhập tên chức vụ.")
    .max(100, "Tên chức vụ tối đa 100 ký tự."),
});

type PositionFormValues = z.infer<typeof positionFormSchema>;

interface PositionUpdateFormProps {
  position: PositionDto;
  onSuccess: () => void;
}

export const PositionUpdateForm = ({
  position,
  onSuccess,
}: PositionUpdateFormProps) => {
  const updatePosition = useAdminUpdatePosition();

  const form = useForm<PositionFormValues>({
    resolver: zodResolver(positionFormSchema),
    defaultValues: { name: position.name },
  });

  const applyServerErrors = useServerFieldErrors(form);

  const onSubmit = (values: PositionFormValues) => {
    updatePosition.mutate({
      id: position.positionId,
      body: { name: values.name },
    }, {
      onSuccess: () => onSuccess(),
      onError: applyServerErrors,
    });
  };

  return (
    <form
      id="position-update-form"
      onSubmit={form.handleSubmit(onSubmit)}
      className="space-y-4"
    >
      <FieldGroup disabled={updatePosition.isPending}>
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
        isPending={updatePosition.isPending}
        isDirty={form.formState.isDirty}
        submitLabel="Lưu thay đổi"
        onCancel={onSuccess}
      />
    </form>
  );
};