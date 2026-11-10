import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";

import { useAdminCreateGenre } from "@/features/admin/catalog/hooks/use-admin-genre";
import { FormActions } from "@/components/common/form/form-actions";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";

const genreFormSchema = z.object({
  name: z
    .string()
    .min(1, "Vui lòng nhập tên thể loại.")
    .max(100, "Tên thể loại tối đa 100 ký tự."),
});

type GenreFormValues = z.infer<typeof genreFormSchema>;

interface GenreCreateFormProps {
  onSuccess: () => void;
}

export const GenreCreateForm = ({ onSuccess }: GenreCreateFormProps) => {
  const createGenre = useAdminCreateGenre();

  const form = useForm<GenreFormValues>({
    resolver: zodResolver(genreFormSchema),
    defaultValues: { name: "" },
  });

  const applyServerErrors = useServerFieldErrors(form);

  const onSubmit = (values: GenreFormValues) => {
    createGenre.mutate(values, {
      onSuccess: () => onSuccess(),
      onError: applyServerErrors,
    });
  };

  return (
    <form
      id="genre-create-form"
      onSubmit={form.handleSubmit(onSubmit)}
      className="space-y-4"
    >
      <FieldGroup disabled={createGenre.isPending}>
        <FormInputField
          name="name"
          control={form.control}
          label="Tên thể loại"
          placeholder="Hành động, Kinh dị, Hoạt hình..."
          autoFocus
          autoComplete="off"
        />
      </FieldGroup>

      <FormActions
        isPending={createGenre.isPending}
        isDirty={form.formState.isDirty}
        submitLabel="Thêm thể loại"
        onCancel={onSuccess}
      />
    </form>
  );
};