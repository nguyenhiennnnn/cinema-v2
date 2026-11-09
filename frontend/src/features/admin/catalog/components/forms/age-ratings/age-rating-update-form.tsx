import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";
import { FormTextareaField } from "@/components/common/form/form-textarea-field";

import { useAdminUpdateAgeRating } from "../../../hooks/use-admin-age-rating";
import type { AgeRatingDto } from "../../../types";
import { FormActions } from "@/components/common/form/form-actions";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";

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

interface AgeRatingUpdateFormProps {
  ageRating: AgeRatingDto;
  onSuccess: () => void;
}

export const AgeRatingUpdateForm = ({
  ageRating,
  onSuccess,
}: AgeRatingUpdateFormProps) => {
  const updateAgeRating = useAdminUpdateAgeRating();

  const form = useForm<AgeRatingFormValues>({
    resolver: zodResolver(ageRatingFormSchema),
    defaultValues: {
      code: ageRating.code,
      description: ageRating.description ?? "",
    },
  });

  const applyServerErrors = useServerFieldErrors(form);

  const onSubmit = (values: AgeRatingFormValues) => {
    updateAgeRating.mutate({
      id: ageRating.ageRatingId,
      body: {
        code: values.code,
        description: values.description || null,
      },
    }, {
      onSuccess: () => onSuccess(),
      onError: applyServerErrors,
    });
  };

  return (
    <form
      id="age-rating-update-form"
      onSubmit={form.handleSubmit(onSubmit)}
      className="space-y-4"
    >
      <FieldGroup disabled={updateAgeRating.isPending}>
        <FormInputField
          name="code"
          control={form.control}
          label="Mã phân loại"
          placeholder="P, K, T13, T16, T18, C"
          autoFocus
          autoComplete="off"
        />

        <FormTextareaField
          name="description"
          control={form.control}
          label="Mô tả (không bắt buộc)"
          placeholder="Phim dành cho khán giả từ đủ 18 tuổi trở lên..."
        />
      </FieldGroup>

      <FormActions
        isPending={updateAgeRating.isPending}
        submitLabel="Lưu thay đổi"
        onCancel={onSuccess}
      />
    </form>
  );
};