import { useEffect, useState } from "react";
import { Controller, useFieldArray, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Plus, Trash2 } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { Field, FieldError, FieldGroup, FieldLabel } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";
import { FormDatePickerField } from "@/components/common/form/form-date-picker-field";
import { FormTextareaField } from "@/components/common/form/form-textarea-field";
import { SingleSelectField } from "@/components/common/form/single-select-field";
import { MultiSelectField } from "@/components/common/form/multi-select-field";
import { ImageUploader } from "@/features/admin/uploads/components/image-uploader";
import { VideoUploader } from "@/features/admin/uploads/components/video-uploader";
import { movieStatusOptions } from "@/lib/enum-options";

import { useAdminDirectors } from "@/features/admin/catalog/hooks/use-admin-director";
import { useAdminLanguages } from "@/features/admin/catalog/hooks/use-admin-language";
import { useAdminAgeRatings } from "@/features/admin/catalog/hooks/use-admin-age-rating";
import { useAdminGenres } from "@/features/admin/catalog/hooks/use-admin-genre";
import { useAdminActors } from "@/features/admin/catalog/hooks/use-admin-actor";

import { useAdminMovieDetail, useAdminUpdateMovie } from "../../hooks/use-admin-movies";
import type { AdminMovieListItemDto } from "../../types";
import { FormSelectField } from "@/components/common/form/form-select-field";
import { showApiErrorToast } from "@/lib/error";
import { movieUpdateFormSchema, type MovieUpdateFormValues } from "../../schemas/movie-schema";

interface MovieUpdateFormProps {
  movie: AdminMovieListItemDto;
  onSuccess: () => void;
}

export const MovieUpdateForm = ({ movie, onSuccess }: MovieUpdateFormProps) => {
  const movieDetail = useAdminMovieDetail(movie.movieId);
  const updateMovie = useAdminUpdateMovie();

  const [directorSearch, setDirectorSearch] = useState("");
  const [languageSearch, setLanguageSearch] = useState("");
  const [ageRatingSearch, setAgeRatingSearch] = useState("");
  const [genreSearch, setGenreSearch] = useState("");
  const [actorSearch, setActorSearch] = useState("");

  const directors = useAdminDirectors({ search: directorSearch });
  const languages = useAdminLanguages();
  const ageRatings = useAdminAgeRatings();
  const genres = useAdminGenres();
  const actors = useAdminActors({ search: actorSearch });

  const form = useForm<MovieUpdateFormValues>({
    resolver: zodResolver(movieUpdateFormSchema),
    defaultValues: {
      title: movie.title,
      originalTitle: "",
      description: "",
      directorId: undefined,
      durationMinutes: 120,
      releaseDate: movie.releaseDate ?? "",
      languageId: undefined,
      subtitle: "",
      ageRatingId: undefined,
      posterUrl: movie.posterUrl ?? "",
      trailerUrl: "",
      status: movie.status,
      genreIds: [],
      cast: [],
    },
  });

  const { fields, append, remove } = useFieldArray({
    control: form.control,
    name: "cast",
  });

  useEffect(() => {
    if (movieDetail.data) {
      const d = movieDetail.data;
      form.reset({
        title: d.title ?? "",
        originalTitle: d.originalTitle ?? "",
        description: d.description ?? "",
        directorId: d.director?.directorId ?? undefined,
        durationMinutes: d.durationMinutes ?? 120,
        releaseDate: d.releaseDate ? String(d.releaseDate) : "",
        languageId: d.language?.languageId ?? undefined,
        subtitle: d.subtitle ?? "",
        ageRatingId: d.ageRating?.ageRatingId ?? undefined,
        posterUrl: d.posterUrl ?? "",
        trailerUrl: d.trailerUrl ?? "",
        status: d.status ?? movie.status,
        genreIds: d.genres?.map((g) => g.genreId) ?? [],
        cast:
          d.cast?.map((c) => ({
            actorId: c.actorId,
            characterName: c.characterName ?? "",
          })) ?? [],
      });
    }
  }, [movieDetail.data, form, movie.status]);

  const onSubmit = async (values: MovieUpdateFormValues) => {
    try {
      await updateMovie.mutateAsync(
        {
          id: movie.movieId,
          body: {
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
            status: values.status,
            genreIds: values.genreIds,
            cast: values.cast.map((c) => ({
              actorId: c.actorId,
              characterName: c.characterName || null,
            })),
          },
        },
        {
          onSuccess: () => {
            onSuccess();
          },
        }
      );
    } catch (error) {
      showApiErrorToast(error, { title: "Không thể cập nhật phim" });
    }
  };

  if (movieDetail.isLoading) {
    return <Skeleton className="h-64 w-full " />;
  }

  return (
    <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
      <FieldGroup>
        <Controller
          name="posterUrl"
          control={form.control}
          render={({ field, fieldState }) => (
            <Field data-invalid={fieldState.invalid}>
              <FieldLabel>Poster phim (2:3)</FieldLabel>
              <ImageUploader
                folder="movies"
                value={field.value}
                onChange={(result) => field.onChange(result?.url ?? "")}
              />
              {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
            </Field>
          )}
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
          <Controller
            name="directorId"
            control={form.control}
            render={({ field, fieldState }) => (
              <Field data-invalid={fieldState.invalid}>
                <FieldLabel>Đạo diễn</FieldLabel>
                <SingleSelectField
                  value={field.value}
                  onChange={(v) => field.onChange(v)}
                  options={directors.data?.items ?? []}
                  selectedLabel={movieDetail.data?.director?.fullName}
                  getOptionValue={(o) => o.directorId}
                  getOptionLabel={(o) => o.fullName}
                  searchValue={directorSearch}
                  onSearchChange={setDirectorSearch}
                  isLoading={directors.isLoading}
                  placeholder="Chọn đạo diễn"
                />
                {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
              </Field>
            )}
          />

          <Controller
            name="languageId"
            control={form.control}
            render={({ field, fieldState }) => (
              <Field data-invalid={fieldState.invalid}>
                <FieldLabel>Ngôn ngữ</FieldLabel>
                <SingleSelectField
                  value={field.value}
                  onChange={(v) => field.onChange(v)}
                  options={languages.data?.items ?? []}
                  selectedLabel={movieDetail.data?.language?.name}
                  getOptionValue={(o) => o.languageId}
                  getOptionLabel={(o) => o.name}
                  searchValue={languageSearch}
                  onSearchChange={setLanguageSearch}
                  isLoading={languages.isLoading}
                  placeholder="Chọn ngôn ngữ"
                />
                {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
              </Field>
            )}
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
          <Controller
            name="ageRatingId"
            control={form.control}
            render={({ field, fieldState }) => (
              <Field data-invalid={fieldState.invalid}>
                <FieldLabel>Phân loại độ tuổi</FieldLabel>
                <SingleSelectField
                  value={field.value}
                  onChange={(v) => field.onChange(v)}
                  options={ageRatings.data?.items ?? []}
                  selectedLabel={movieDetail.data?.ageRating?.code}
                  getOptionValue={(o) => o.ageRatingId}
                  getOptionLabel={(o) => o.code}
                  searchValue={ageRatingSearch}
                  onSearchChange={setAgeRatingSearch}
                  isLoading={ageRatings.isLoading}
                  placeholder="Chọn độ tuổi"
                />
                {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
              </Field>
            )}
          />

          <FormInputField
            name="subtitle"
            control={form.control}
            label="Phụ đề (không bắt buộc)"
            placeholder="VD: Phụ đề Việt"
          />
        </div>

        <Controller
          name="trailerUrl"
          control={form.control}
          render={({ field, fieldState }) => (
            <Field data-invalid={fieldState.invalid}>
              <FieldLabel>Trailer phim</FieldLabel>
              <VideoUploader
                folder="movies/trailers"
                value={field.value}
                onChange={(result) => field.onChange(result?.url ?? "")}
              />
              {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
            </Field>
          )}
        />

        <FormSelectField
          name="status"
          control={form.control}
          label="Trạng thái phát hành"
          placeholder="Chọn trạng thái"
          options={movieStatusOptions}
          getOptionValue={(o) => o.value}
          getOptionLabel={(o) => o.label}
        />

        <Controller
          name="genreIds"
          control={form.control}
          render={({ field, fieldState }) => (
            <Field data-invalid={fieldState.invalid}>
              <FieldLabel>Thể loại</FieldLabel>
              <MultiSelectField
                value={field.value ?? []}
                onChange={(values) => field.onChange(values)}
                options={genres.data?.items ?? []}
                selectedOptions={movieDetail.data?.genres ?? []}
                getOptionValue={(o) => o.genreId}
                getOptionLabel={(o) => o.name}
                searchValue={genreSearch}
                onSearchChange={setGenreSearch}
                isLoading={genres.isLoading}
                placeholder="Chọn thể loại phim"
              />
              {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
            </Field>
          )}
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
                            onChange={(v) => field.onChange(v)}
                            options={actors.data?.items ?? []}
                            selectedLabel={
                              movieDetail.data?.cast?.find((c) => c.actorId === field.value)?.fullName
                            }
                            getOptionValue={(o) => o.actorId}
                            getOptionLabel={(o) => o.fullName}
                            searchValue={actorSearch}
                            onSearchChange={setActorSearch}
                            isLoading={actors.isLoading}
                            placeholder="Chọn diễn viên"
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

      <div className="flex justify-end gap-2 pt-2">
        <Button type="button" variant="outline" onClick={onSuccess} disabled={updateMovie.isPending}>
          Hủy
        </Button>
        <Button type="submit" disabled={updateMovie.isPending}>
          {updateMovie.isPending ? "Đang lưu..." : "Lưu thay đổi"}
        </Button>
      </div>
    </form>
  );
};