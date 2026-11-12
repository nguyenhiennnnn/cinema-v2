import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { NAME_MAX_LENGTH } from "@/config/default-constants";

import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";

import { useAdminCreateScreenType } from "@/features/admin/catalog/hooks/use-admin-screen-type";
import { FormActions } from "@/components/common/form/form-actions";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";

const screenTypeFormSchema = z.object({
  name: z
    .string()
    .min(1, "Vui lòng nhập tên loại màn hình.")
    .max(NAME_MAX_LENGTH, `Tên loại màn hình tối đa ${NAME_MAX_LENGTH} ký tự.`),
});

type ScreenTypeFormValues = z.infer<typeof screenTypeFormSchema>;

interface ScreenTypeCreateFormProps {
  onSuccess: () => void;
}

export const ScreenTypeCreateForm = ({
  onSuccess,
}: ScreenTypeCreateFormProps) => {
  const createScreenType = useAdminCreateScreenType();

  const form = useForm<ScreenTypeFormValues>({
    resolver: zodResolver(screenTypeFormSchema),
    defaultValues: { name: "" },
  });

  const applyServerErrors = useServerFieldErrors(form);

  const onSubmit = (values: ScreenTypeFormValues) => {
    createScreenType.mutate(values, {
      onSuccess: () => onSuccess(),
      onError: applyServerErrors,
    });
  };

  return (
    <form
      id="screen-type-create-form"
      onSubmit={form.handleSubmit(onSubmit)}
      className="space-y-4"
    >
      <FieldGroup disabled={createScreenType.isPending}>
        <FormInputField
          name="name"
          control={form.control}
          label="Tên loại màn hình"
          placeholder="2D, 3D, IMAX Laser, ScreenX..."
          autoFocus
          autoComplete="off"
        />
      </FieldGroup>

      <FormActions
        isPending={createScreenType.isPending}
        isDirty={form.formState.isDirty}
        submitLabel="Thêm loại màn hình"
        onCancel={onSuccess}
      />
    </form>
  );
};