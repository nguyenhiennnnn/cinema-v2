import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";

import { useAdminUpdateGenre } from "../../../hooks/use-admin-genre";
import type { GenreDto } from "../../../types";

const genreFormSchema = z.object({
  name: z
    .string()
    .min(1, "Vui lòng nhập tên thể loại.")
    .max(100, "Tên thể loại tối đa 100 ký tự."),
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

  const onSubmit = async (values: GenreFormValues) => {
    await updateGenre.mutateAsync({ id: genre.genreId, body: values }, {
      onSuccess: () => {
        onSuccess();
      }
    });
  };

  return (
    <form
      id="genre-update-form"
      onSubmit={form.handleSubmit(onSubmit)}
      className="space-y-4"
    >
      <FieldGroup>
        <FormInputField
          name="name"
          control={form.control}
          label="Tên thể loại"
          placeholder="Hành động, Kinh dị, Hoạt hình..."
          autoFocus
          autoComplete="off"
        />
      </FieldGroup>

      <div className="flex justify-end gap-2 pt-2">
        <Button
          type="button"
          variant="outline"
          onClick={onSuccess}
          disabled={updateGenre.isPending}
        >
          Hủy
        </Button>
        <Button type="submit" disabled={updateGenre.isPending}>
          {updateGenre.isPending ? "Đang lưu..." : "Lưu thay đổi"}
        </Button>
      </div>
    </form>
  );
};