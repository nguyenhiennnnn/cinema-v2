import { useState } from "react";
import { Controller, useFieldArray, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Plus, Trash2 } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Field, FieldError, FieldGroup, FieldLabel } from "@/components/ui/field";
import { FormActions } from "@/components/common/form/form-actions";
import { FormInputField } from "@/components/common/form/form-input-field";
import { FormDatePickerField } from "@/components/common/form/form-date-picker-field";
import { FormTextareaField } from "@/components/common/form/form-textarea-field";
import { SingleSelectField } from "@/components/common/form/single-select-field";

import { useAdminDirectors } from "@/features/admin/catalog/hooks/use-admin-director";
import { useAdminLanguages } from "@/features/admin/catalog/hooks/use-admin-language";
import { useAdminAgeRatings } from "@/features/admin/catalog/hooks/use-admin-age-rating";
import { useAdminGenres } from "@/features/admin/catalog/hooks/use-admin-genre";
import { useAdminActors } from "@/features/admin/catalog/hooks/use-admin-actor";
import {
  useActorQuickCreate,
  useAgeRatingQuickCreate,
  useDirectorQuickCreate,
  useGenreQuickCreate,
  useLanguageQuickCreate,
} from "@/features/admin/catalog/hooks/use-quick-create";
import { useDebouncedValue } from "@/hooks/use-debounced-value";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";
import { LOOKUP_PAGE_SIZE } from "@/config/default-constants";

import { useAdminCreateMovie } from "../../hooks/use-admin-movies";
import { movieFormSchema, type MovieCreateFormValues } from "../../schemas/movie-schema";
import { FormMultiSelectField, FormSearchSelectField } from "@/components/common/form/form-search-select-field";
import { FormImageField, FormVideoField } from "@/components/common/form/form-upload-field";

interface MovieCreateFormProps {
  onSuccess: () => void;
}

export const MovieCreateForm = ({ onSuccess }: MovieCreateFormProps) => {
  const createMovie = useAdminCreateMovie();

  const [directorSearch, setDirectorSearch] = useState("");
  const [languageSearch, setLanguageSearch] = useState("");
  const [ageRatingSearch, setAgeRatingSearch] = useState("");
  const [genreSearch, setGenreSearch] = useState("");
  const [actorSearch, setActorSearch] = useState("");

  const directors = useAdminDirectors({ search: useDebouncedValue(directorSearch) });
  const languages = useAdminLanguages({ pageSize: LOOKUP_PAGE_SIZE });
  const ageRatings = useAdminAgeRatings({ pageSize: LOOKUP_PAGE_SIZE });
  const genres = useAdminGenres({ pageSize: LOOKUP_PAGE_SIZE });
  const actors = useAdminActors({ search: useDebouncedValue(actorSearch) });

  const directorQuickCreate = useDirectorQuickCreate();
  const languageQuickCreate = useLanguageQuickCreate();
  const ageRatingQuickCreate = useAgeRatingQuickCreate();
  const genreQuickCreate = useGenreQuickCreate();
  const actorQuickCreate = useActorQuickCreate();

  const form = useForm<MovieCreateFormValues>({
    resolver: zodResolver(movieFormSchema),
    defaultValues: {
      title: "",
      originalTitle: "",
      description: "",
      durationMinutes: 120,
      releaseDate: "",
      subtitle: "",
      posterUrl: "",
      trailerUrl: "",
      genreIds: [],
      cast: [],
    },
  });

  const applyServerErrors = useServerFieldErrors(form);

  const { fields, append, remove } = useFieldArray({
    control: form.control,
    name: "cast",
  });

  const onSubmit = (values: MovieCreateFormValues) => {
    createMovie.mutate(
      {
        title: values.title,
        originalTitle: values.originalTitle || null,
        description: values.description || null,
        directorId: values.directorId ?? null,
        durationMinutes: values.durationMinutes,
        releaseDate: values.releaseDate || null,
        languageId: values.languageId ?? null,
        subtitle: values.subtitle || null,
        ageRatingId: values.ageRatingId ?? null,
        posterUrl: values.posterUrl || null,
        trailerUrl: values.trailerUrl || null,
        status: "ComingSoon",
        genreIds: values.genreIds,
        cast: values.cast.map((c) => ({
          actorId: c.actorId,
          characterName: c.characterName || null,
        })),
      },
      {
        onSuccess: () => onSuccess(),
        onError: applyServerErrors,
      }
    );
  };

  return (
    <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
      <FieldGroup>
        <FormImageField
          name="posterUrl"
          control={form.control}
          label="Poster phim (2:3)"
          folder="movies"
        />

        <FormInputField
          name="title"
          control={form.control}
          label="Tên phim"
          placeholder="Nhập tên phim..."
        />
        <FormInputField
          name="originalTitle"
          control={form.control}
          label="Tên gốc (không bắt buộc)"
          placeholder="VD: Original English Title"
        />
        <FormTextareaField
          name="description"
          control={form.control}
          label="Mô tả nội dung"
          placeholder="Tóm tắt nội dung phim..."
        />

        <div className="grid grid-cols-2 gap-3">
          <FormSearchSelectField
            name="directorId"
            control={form.control}
            label="Đạo diễn"
            options={directors.data?.items ?? []}
            getOptionValue={(o) => o.directorId}
            getOptionLabel={(o) => o.fullName}
            searchValue={directorSearch}
            onSearchChange={setDirectorSearch}
            isLoading={directors.isLoading}
            placeholder="Chọn đạo diễn"
            {...directorQuickCreate}
          />

          <FormSearchSelectField
            name="languageId"
            control={form.control}
            label="Ngôn ngữ"
            options={languages.data?.items ?? []}
            getOptionValue={(o) => o.languageId}
            getOptionLabel={(o) => o.name}
            searchValue={languageSearch}
            onSearchChange={setLanguageSearch}
            isLoading={languages.isLoading}
            placeholder="Chọn ngôn ngữ"
            {...languageQuickCreate}
          />
        </div>

        <div className="grid grid-cols-2 gap-3">
          <FormInputField
            name="durationMinutes"
            control={form.control}
            label="Thời lượng (phút)"
            type="number"
            min={1}
          />
          <FormDatePickerField
            name="releaseDate"
            control={form.control}
            label="Ngày khởi chiếu"
            placeholder="Chọn ngày khởi chiếu"
          />
        </div>

        <div className="grid grid-cols-2 gap-3">
          <FormSearchSelectField
            name="ageRatingId"
            control={form.control}
            label="Phân loại độ tuổi"
            options={ageRatings.data?.items ?? []}
            getOptionValue={(o) => o.ageRatingId}
            getOptionLabel={(o) => o.code}
            searchValue={ageRatingSearch}
            onSearchChange={setAgeRatingSearch}
            isLoading={ageRatings.isLoading}
            placeholder="Chọn độ tuổi"
            {...ageRatingQuickCreate}
          />

          <FormInputField
            name="subtitle"
            control={form.control}
            label="Phụ đề (không bắt buộc)"
            placeholder="VD: Phụ đề Việt"
          />
        </div>

        <FormVideoField
          name="trailerUrl"
          control={form.control}
          label="Trailer phim"
          folder="movies/trailers"
        />

        <FormMultiSelectField
          name="genreIds"
          control={form.control}
          label="Thể loại"
          options={genres.data?.items ?? []}
          getOptionValue={(o) => o.genreId}
          getOptionLabel={(o) => o.name}
          searchValue={genreSearch}
          onSearchChange={setGenreSearch}
          isLoading={genres.isLoading}
          placeholder="Chọn thể loại phim"
          {...genreQuickCreate}
        />

        <Field>
          <FieldLabel>Diễn viên & Vai diễn</FieldLabel>
          <div className="space-y-2">
            {fields.map((item, index) => (
              <div key={item.id} className="space-y-1">
                <div className="flex items-center gap-2">
                  <div className="flex-1">
                    <Controller
                      name={`cast.${index}.actorId`}
                      control={form.control}
                      render={({ field, fieldState }) => (
                        <div data-invalid={fieldState.invalid} className="group/field">
                          <SingleSelectField
                            value={field.value}
                            onChange={(v) => field.onChange(v ?? undefined)}
                            options={actors.data?.items ?? []}
                            getOptionValue={(o) => o.actorId}
                            getOptionLabel={(o) => o.fullName}
                            searchValue={actorSearch}
                            onSearchChange={setActorSearch}
                            isLoading={actors.isLoading}
                            placeholder="Chọn diễn viên"
                            {...actorQuickCreate}
                          />
                          {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
                        </div>
                      )}
                    />
                  </div>
                  <div className="w-40">
                    <Controller
                      name={`cast.${index}.characterName`}
                      control={form.control}
                      render={({ field }) => (
                        <input
                          {...field}
                          placeholder="Vai diễn"
                          className="h-9 w-full border border-input bg-transparent px-3 text-sm outline-none focus-visible:ring-2 focus-visible:ring-ring/50"
                        />
                      )}
                    />
                  </div>
                  <Button
                    type="button"
                    variant="ghost"
                    size="icon"
                    className="size-8 shrink-0 text-destructive"
                    onClick={() => remove(index)}
                  >
                    <Trash2 className="size-4" />
                  </Button>
                </div>
              </div>
            ))}
            <Button
              type="button"
              variant="outline"
              size="sm"
              onClick={() => append({ actorId: undefined as unknown as number, characterName: "" })}
            >
              <Plus className="size-4" /> Thêm diễn viên
            </Button>
          </div>
        </Field>
      </FieldGroup>

      <FormActions
        isPending={createMovie.isPending}
        submitLabel="Thêm phim"
        onCancel={onSuccess}
      />
    </form>
  );
};