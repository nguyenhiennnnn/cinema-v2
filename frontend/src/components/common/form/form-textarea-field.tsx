import type { Control, FieldPath, FieldValues } from "react-hook-form";
import { Controller } from "react-hook-form";

import { Field, FieldError, FieldLabel } from "@/components/ui/field";
import { Textarea } from "@/components/ui/textarea";

interface FormTextareaFieldProps<TFieldValues extends FieldValues> {
  name: FieldPath<TFieldValues>;
  control: Control<TFieldValues>;
  label: string;
  placeholder?: string;
  rows?: number;
  className?: string;
  autoFocus?: boolean;
  disabled?: boolean;
}

export const FormTextareaField = <TFieldValues extends FieldValues>({
  name,
  control,
  label,
  placeholder,
  rows = 3,
  className = "min-h-20 resize-none",
  autoFocus,
  disabled,
}: FormTextareaFieldProps<TFieldValues>) => {
  const id = `field-${name}`;

  return (
    <Controller
      name={name}
      control={control}
      render={({ field, fieldState }) => (
        <Field data-invalid={fieldState.invalid}>
          <FieldLabel htmlFor={id}>{label}</FieldLabel>
          <Textarea
            {...field}
            id={id}
            placeholder={placeholder}
            rows={rows}
            className={className}
            autoFocus={autoFocus}
            disabled={disabled}
            aria-invalid={fieldState.invalid}
          />
          {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
        </Field>
      )}
    />
  );
};