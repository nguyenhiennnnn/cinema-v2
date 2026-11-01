import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";
import { FormTextareaField } from "@/components/common/form/form-textarea-field";

import { useAdminUpdateAgeRating } from "../../../hooks/use-admin-age-rating";
import type { AgeRatingDto } from "../../../types";

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

  const onSubmit = async (values: AgeRatingFormValues) => {
    await updateAgeRating.mutateAsync({
      id: ageRating.ageRatingId,
      body: {
        code: values.code,
        description: values.description || null,
      },
    }, {
      onSuccess: () => {
        onSuccess();
      }
    });
  };

  return (
    <form
      id="age-rating-update-form"
      onSubmit={form.handleSubmit(onSubmit)}
      className="space-y-4"
    >
      <FieldGroup>
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

      <div className="flex justify-end gap-2 pt-2">
        <Button
          type="button"
          variant="outline"
          onClick={onSuccess}
          disabled={updateAgeRating.isPending}
        >
          Hủy
        </Button>
        <Button type="submit" disabled={updateAgeRating.isPending}>
          {updateAgeRating.isPending ? "Đang lưu..." : "Lưu thay đổi"}
        </Button>
      </div>
    </form>
  );
};