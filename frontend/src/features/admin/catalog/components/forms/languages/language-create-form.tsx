import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { toast } from "@/components/ui/toast";
import { Button } from "@/components/ui/button";
import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";

import { useAdminCreateLanguage } from "../../../hooks/use-admin-language";

const languageFormSchema = z.object({
  name: z
    .string()
    .min(1, "Vui lòng nhập tên ngôn ngữ.")
    .max(100, "Tên ngôn ngữ tối đa 100 ký tự."),
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

  const onSubmit = async (values: LanguageFormValues) => {
    await createLanguage.mutateAsync(values, {
      onSuccess: () => {
        toast.add({ title: "Đã thêm ngôn ngữ mới.", type: "success" });
        onSuccess();
      }
    });
  };

  return (
    <form
      id="language-create-form"
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
          disabled={createLanguage.isPending}
        >
          Hủy
        </Button>
        <Button type="submit" disabled={createLanguage.isPending}>
          {createLanguage.isPending ? "Đang lưu..." : "Thêm ngôn ngữ"}
        </Button>
      </div>
    </form>
  );
};