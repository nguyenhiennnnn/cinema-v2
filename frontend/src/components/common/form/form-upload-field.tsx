import type { Control, FieldPath, FieldValues } from "react-hook-form";
import { Controller } from "react-hook-form";

import { Field, FieldError, FieldLabel } from "@/components/ui/field";
import { ImageUploader } from "@/features/admin/uploads/components/image-uploader";
import { VideoUploader } from "@/features/admin/uploads/components/video-uploader";

interface FormUploadFieldProps<TFieldValues extends FieldValues> {
  name: FieldPath<TFieldValues>;
  control: Control<TFieldValues>;
  label: string;
  folder: string;
  className?: string;
}

export const FormImageField = <TFieldValues extends FieldValues>({
  name,
  control,
  label,
  folder,
  className,
}: FormUploadFieldProps<TFieldValues>) => (
  <Controller
    name={name}
    control={control}
    render={({ field, fieldState }) => (
      <Field data-invalid={fieldState.invalid} className={className}>
        <FieldLabel>{label}</FieldLabel>
        <ImageUploader
          folder={folder}
          value={field.value}
          onChange={(result) => field.onChange(result?.url ?? "")}
        />
        {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
      </Field>
    )}
  />
);

export const FormVideoField = <TFieldValues extends FieldValues>({
  name,
  control,
  label,
  folder,
  className,
}: FormUploadFieldProps<TFieldValues>) => (
  <Controller
    name={name}
    control={control}
    render={({ field, fieldState }) => (
      <Field data-invalid={fieldState.invalid} className={className}>
        <FieldLabel>{label}</FieldLabel>
        <VideoUploader
          folder={folder}
          value={field.value}
          onChange={(result) => field.onChange(result?.url ?? "")}
        />
        {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
      </Field>
    )}
  />
);
