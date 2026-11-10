import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import {
  FieldGroup,
  } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";

import { useAdminCreateCombo } from "@/features/admin/combos/hooks/use-admin-combo";
import { FormImageField } from "@/components/common/form/form-upload-field";
import { FormTextareaField } from "@/components/common/form/form-textarea-field";
import { FormActions } from "@/components/common/form/form-actions";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";

const comboFormSchema = z.object({
  name: z
    .string()
    .min(1, "Vui lòng nhập tên combo.")
    .max(150, "Tên combo tối đa 150 ký tự."),
  description: z
    .string()
    .max(500, "Mô tả tối đa 500 ký tự.")
    .optional()
    .or(z.literal("")),
  price: z.coerce
    .number({ message: "Giá phải là số." })
    .min(0, "Giá không được âm."),
  imageUrl: z.string().optional().or(z.literal("")),
});

type ComboFormValues = z.infer<typeof comboFormSchema>;

interface ComboCreateFormProps {
  onSuccess: () => void;
}

export const ComboCreateForm = ({ onSuccess }: ComboCreateFormProps) => {
  const createCombo = useAdminCreateCombo();

  const form = useForm<ComboFormValues>({
    resolver: zodResolver(comboFormSchema),
    defaultValues: { name: "", description: "", price: 0, imageUrl: "" },
  });

  const applyServerErrors = useServerFieldErrors(form);

  const onSubmit = (values: ComboFormValues) => {
    createCombo.mutate({
      name: values.name,
      description: values.description || null,
      price: values.price,
      imageUrl: values.imageUrl || null,
    }, {
      onSuccess: () => onSuccess(),
      onError: applyServerErrors,
    });
  };

  return (
    <form
      id="combo-create-form"
      onSubmit={form.handleSubmit(onSubmit)}
      className="space-y-4"
    >
      <FieldGroup disabled={createCombo.isPending}>
        <FormImageField
          name="imageUrl"
          control={form.control}
          label="Hình ảnh combo (không bắt buộc)"
          folder="combos"
        />

        <FormInputField
          name="name"
          control={form.control}
          label="Tên combo"
          placeholder="Combo bắp nước siêu to..."
          autoComplete="off"
        />

        <FormTextareaField
          name="description"
          control={form.control}
          label="Mô tả (không bắt buộc)"
          placeholder="1 bắp lớn + 2 nước ngọt vừa..."
        />

        <FormInputField
          name="price"
          control={form.control}
          label="Giá (VND)"
          type="number"
          min={0}
          step={1000}
          placeholder="0"
        />
      </FieldGroup>

      <FormActions
        isPending={createCombo.isPending}
        isDirty={form.formState.isDirty}
        submitLabel="Thêm combo"
        onCancel={onSuccess}
      />
    </form>
  );
};