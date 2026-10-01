import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { toast } from "@/components/ui/toast";
import { Button } from "@/components/ui/button";
import {
  Field,
  FieldError,
  FieldGroup,
  FieldLabel,
} from "@/components/ui/field";
import { Textarea } from "@/components/ui/textarea";
import { FormInputField } from "@/components/common/form/form-input-field";

import { useAdminCreateAgeRating } from "../../../hooks/use-admin-age-rating";

const ageRatingFormSchema = z.object({
  code: z
    .string()
    .min(1, "Vui lòng nhập mã phân loại.")
    .max(10, "Mã phân loại tối đa 10 ký tự."),
  description: z
    .string()
    .max(255, "Mô tả tối đa 255 ký tự.")
    .optional()
    .or(z.literal("")),
});

type AgeRatingFormValues = z.infer<typeof ageRatingFormSchema>;

interface AgeRatingCreateFormProps {
  onSuccess: () => void;
}

export const AgeRatingCreateForm = ({ onSuccess }: AgeRatingCreateFormProps) => {
  const createAgeRating = useAdminCreateAgeRating();

  const form = useForm<AgeRatingFormValues>({
    resolver: zodResolver(ageRatingFormSchema),
    defaultValues: { code: "", description: "" },
  });

  const onSubmit = async (values: AgeRatingFormValues) => {
    await createAgeRating.mutateAsync({
      code: values.code,
      description: values.description || null,
    }, {
      onSuccess: () => {
        toast.add({ title: "Đã thêm phân loại độ tuổi mới.", type: "success" });
        onSuccess();
      }
    });
  };

  return (
    <form
      id="age-rating-create-form"
      onSubmit={form.handleSubmit(onSubmit)}
      className="space-y-4"
    >
      <FieldGroup>
        <FormInputField
          name="code"
          control={form.control}
          label="Mã phân loại"
          placeholder="P, K, T13, T16, T18, C"
          autoComplete="off"
          autoFocus
        />

        <Controller
          name="description"
          control={form.control}
          render={({ field, fieldState }) => (
            <Field data-invalid={fieldState.invalid}>
              <FieldLabel htmlFor="age-rating-create-description">
                Mô tả (không bắt buộc)
              </FieldLabel>
              <Textarea
                {...field}
                id="age-rating-create-description"
                placeholder="Phim dành cho khán giả từ đủ 18 tuổi trở lên..."
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
      </FieldGroup>

      <div className="flex justify-end gap-2 pt-2">
        <Button
          type="button"
          variant="outline"
          onClick={onSuccess}
          disabled={createAgeRating.isPending}
        >
          Hủy
        </Button>
        <Button type="submit" disabled={createAgeRating.isPending}>
          {createAgeRating.isPending ? "Đang lưu..." : "Thêm phân loại"}
        </Button>
      </div>
    </form>
  );
};