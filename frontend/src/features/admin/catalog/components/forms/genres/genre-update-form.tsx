import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { NAME_MAX_LENGTH, FORM_SAVE_CHANGES_LABEL } from "@/config/default-constants";

import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";

import { useAdminUpdateGenre } from "@/features/admin/catalog/hooks/use-admin-genre";
import type { GenreDto } from "@/features/admin/catalog/types";
import { FormActions } from "@/components/common/form/form-actions";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";

const genreFormSchema = z.object({
  name: z
    .string()
    .min(1, "Vui lòng nhập tên thể loại.")
    .max(NAME_MAX_LENGTH, `Tên thể loại tối đa ${NAME_MAX_LENGTH} ký tự.`),
});

type GenreFormValues = z.infer<typeof genreFormSchema>;

interface GenreUpdateFormProps {
  genre: GenreDto;
  onSuccess: () => void;
}

export const GenreUpdateForm = ({ genre, onSuccess }: GenreUpdateFormProps) => {
  const updateGenre = useAdminUpdateGenre();

  const form = useForm<GenreFormValues>({
    resolver: zodResolver(genreFormSchema),
    defaultValues: { name: genre.name },
  });

  const applyServerErrors = useServerFieldErrors(form);

  const onSubmit = (values: GenreFormValues) => {
    updateGenre.mutate({ id: genre.genreId, body: values }, {
      onSuccess: () => onSuccess(),
      onError: applyServerErrors,
    });
  };

  return (
    <form
      id="genre-update-form"
      onSubmit={form.handleSubmit(onSubmit)}
      className="space-y-4"
    >
      <FieldGroup disabled={updateGenre.isPending}>
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
        isPending={updateGenre.isPending}
        isDirty={form.formState.isDirty}
        submitLabel={FORM_SAVE_CHANGES_LABEL}
        onCancel={onSuccess}
      />
    </form>
  );
};