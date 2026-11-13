import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { NAME_MAX_LENGTH, FORM_SAVE_CHANGES_LABEL } from "@/config/default-constants";

import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";

import { useAdminUpdateLanguage } from "@/features/admin/catalog/hooks/use-admin-language";
import type { LanguageDto } from "@/features/admin/catalog/types";
import { FormActions } from "@/components/common/form/form-actions";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";

const languageFormSchema = z.object({
  name: z
    .string()
    .min(1, "Vui lòng nhập tên ngôn ngữ.")
    .max(NAME_MAX_LENGTH, `Tên ngôn ngữ tối đa ${NAME_MAX_LENGTH} ký tự.`),
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

  const applyServerErrors = useServerFieldErrors(form);

  const onSubmit = (values: LanguageFormValues) => {
    updateLanguage.mutate({
      id: language.languageId,
      body: values,
    }, {
      onSuccess: () => onSuccess(),
      onError: applyServerErrors,
    });
  };

  return (
    <form
      id="language-update-form"
      onSubmit={form.handleSubmit(onSubmit)}
      className="space-y-4"
    >
      <FieldGroup disabled={updateLanguage.isPending}>
        <FormInputField
          name="name"
          control={form.control}
          label="Tên ngôn ngữ"
          placeholder="Tiếng Việt, Tiếng Anh, Phụ đề Việt..."
          autoFocus
          autoComplete="off"
        />
      </FieldGroup>

      <FormActions
        isPending={updateLanguage.isPending}
        isDirty={form.formState.isDirty}
        submitLabel={FORM_SAVE_CHANGES_LABEL}
        onCancel={onSuccess}
      />
    </form>
  );
};