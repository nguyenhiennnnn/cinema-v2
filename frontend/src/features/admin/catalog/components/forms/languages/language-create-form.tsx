import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { NAME_MAX_LENGTH } from "@/config/default-constants";

import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";

import { useAdminCreateLanguage } from "@/features/admin/catalog/hooks/use-admin-language";
import { FormActions } from "@/components/common/form/form-actions";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";

const languageFormSchema = z.object({
  name: z
    .string()
    .min(1, "Vui lòng nhập tên ngôn ngữ.")
    .max(NAME_MAX_LENGTH, `Tên ngôn ngữ tối đa ${NAME_MAX_LENGTH} ký tự.`),
});

type LanguageFormValues = z.infer<typeof languageFormSchema>;

interface LanguageCreateFormProps {
  onSuccess: () => void;
}

export const LanguageCreateForm = ({ onSuccess }: LanguageCreateFormProps) => {
  const createLanguage = useAdminCreateLanguage();

  const form = useForm<LanguageFormValues>({
    resolver: zodResolver(languageFormSchema),
    defaultValues: { name: "" },
  });

  const applyServerErrors = useServerFieldErrors(form);

  const onSubmit = (values: LanguageFormValues) => {
    createLanguage.mutate(values, {
      onSuccess: () => onSuccess(),
      onError: applyServerErrors,
    });
  };

  return (
    <form
      id="language-create-form"
      onSubmit={form.handleSubmit(onSubmit)}
      className="space-y-4"
    >
      <FieldGroup disabled={createLanguage.isPending}>
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
        isPending={createLanguage.isPending}
        isDirty={form.formState.isDirty}
        submitLabel="Thêm ngôn ngữ"
        onCancel={onSuccess}
      />
    </form>
  );
};