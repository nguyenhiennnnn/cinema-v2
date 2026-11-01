import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import {
  Field,
  FieldError,
  FieldGroup,
  FieldLabel,
} from "@/components/ui/field";
import { Textarea } from "@/components/ui/textarea";
import { FormInputField } from "@/components/common/form/form-input-field";
import { ImageUploader } from "@/features/admin/uploads/components/image-uploader";

import { useAdminUpdateCombo } from "../../hooks/use-admin-combo";
import type { AdminComboDto } from "../../types";

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

interface ComboUpdateFormProps {
  combo: AdminComboDto;
  onSuccess: () => void;
}

export const ComboUpdateForm = ({ combo, onSuccess }: ComboUpdateFormProps) => {
  const updateCombo = useAdminUpdateCombo();

  const form = useForm<ComboFormValues>({
    resolver: zodResolver(comboFormSchema),
    defaultValues: {
      name: combo.name,
      description: combo.description ?? "",
      price: combo.price,
      imageUrl: combo.imageUrl ?? "",
    },
  });

  const onSubmit = async (values: ComboFormValues) => {
    await updateCombo.mutateAsync({
      id: combo.comboId,
      body: {
        name: values.name,
        description: values.description || null,
        price: values.price,
        imageUrl: values.imageUrl || null,
      },
    }, {
      onSuccess: () => {
        onSuccess();
      }
    });
  };

  return (
    <form
      id="combo-update-form"
      onSubmit={form.handleSubmit(onSubmit)}
      className="space-y-4"
    >
      <FieldGroup>
        <Controller
          name="imageUrl"
          control={form.control}
          render={({ field, fieldState }) => (
            <Field data-invalid={fieldState.invalid}>
              <FieldLabel>Hình ảnh combo (không bắt buộc)</FieldLabel>
              <ImageUploader
                folder="combos"
                value={field.value}
                onChange={(result) => field.onChange(result?.url ?? "")}
              />
              {fieldState.invalid && (
                <FieldError errors={[fieldState.error]} />
              )}
            </Field>
          )}
        />

        <FormInputField
          name="name"
          control={form.control}
          label="Tên combo"
          placeholder="Combo bắp nước siêu to..."
          autoComplete="off"
        />

        <Controller
          name="description"
          control={form.control}
          render={({ field, fieldState }) => (
            <Field data-invalid={fieldState.invalid}>
              <FieldLabel htmlFor="combo-update-description">
                Mô tả (không bắt buộc)
              </FieldLabel>
              <Textarea
                {...field}
                id="combo-update-description"
                placeholder="1 bắp lớn + 2 nước ngọt vừa..."
                rows={3}
                className="min-h-20 resize-none"
                aria-invalid={fieldState.invalid}
              />
              {fieldState.invalid && (
                <FieldError errors={[fieldState.error]} />
              )}
            </Field>
          )}
        />

        <FormInputField
          name="price"
          control={form.control}
          label="Giá (VND)"
          type="number"
          min={0}
          step={1000}
        />
      </FieldGroup>

      <div className="flex justify-end gap-2 pt-2">
        <Button
          type="button"
          variant="outline"
          onClick={onSuccess}
          disabled={updateCombo.isPending}
        >
          Hủy
        </Button>
        <Button type="submit" disabled={updateCombo.isPending}>
          {updateCombo.isPending ? "Đang lưu..." : "Lưu thay đổi"}
        </Button>
      </div>
    </form>
  );
};