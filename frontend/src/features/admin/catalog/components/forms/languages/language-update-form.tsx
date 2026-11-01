import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";

import { useAdminUpdateLanguage } from "../../../hooks/use-admin-language";
import type { LanguageDto } from "../../../types";

const languageFormSchema = z.object({
  name: z
    .string()
    .min(1, "Vui lòng nhập tên ngôn ngữ.")
    .max(100, "Tên ngôn ngữ tối đa 100 ký tự."),
});

type LanguageFormValues = z.infer<typeof languageFormSchema>;

interface LanguageUpdateFormProps {
  language: LanguageDto;
  onSuccess: () => void;
}

export const LanguageUpdateForm = ({
  language,
  onSuccess,
}: LanguageUpdateFormProps) => {
  const updateLanguage = useAdminUpdateLanguage();

  const form = useForm<LanguageFormValues>({
    resolver: zodResolver(languageFormSchema),
    defaultValues: { name: language.name },
  });

  const onSubmit = async (values: LanguageFormValues) => {
    await updateLanguage.mutateAsync({
      id: language.languageId,
      body: values,
    }, {
      onSuccess: () => {
        onSuccess();
      }
    });
  };

  return (
    <form
      id="language-update-form"
      onSubmit={form.handleSubmit(onSubmit)}
      className="space-y-4"
    >
      <FieldGroup>
        <FormInputField
          name="name"
          control={form.control}
          label="Tên ngôn ngữ"
          placeholder="Tiếng Việt, Tiếng Anh, Phụ đề Việt..."
          autoFocus
          autoComplete="off"
        />
      </FieldGroup>

      <div className="flex justify-end gap-2 pt-2">
        <Button
          type="button"
          variant="outline"
          onClick={onSuccess}
          disabled={updateLanguage.isPending}
        >
          Hủy
        </Button>
        <Button type="submit" disabled={updateLanguage.isPending}>
          {updateLanguage.isPending ? "Đang lưu..." : "Lưu thay đổi"}
        </Button>
      </div>
    </form>
  );
};