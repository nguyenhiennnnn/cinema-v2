import type { Control, FieldPath, FieldValues } from "react-hook-form";
import { Controller } from "react-hook-form";

import { Field, FieldError, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";

interface FormInputFieldProps<TFieldValues extends FieldValues> {
  name: FieldPath<TFieldValues>;
  control: Control<TFieldValues>;
  label: string;
  type?: React.HTMLInputTypeAttribute;
  min?: number;
  max?: number;
  step?: number;
  placeholder?: string;
  autoFocus?: boolean;
  autoComplete?: string;
}

export const FormInputField = <TFieldValues extends FieldValues>({
  name,
  control,
  label,
  type = "text",
  min,
  max,
  step,
  placeholder,
  autoFocus,
  autoComplete,
}: FormInputFieldProps<TFieldValues>) => {
  const id = `field-${name}`;

  return (
    <Controller
      name={name}
      control={control}
      render={({ field, fieldState }) => (
        <Field data-invalid={fieldState.invalid}>
          <FieldLabel htmlFor={id}>{label}</FieldLabel>
          <Input
            {...field}
            id={id}
            type={type}
            min={min}
            max={max}
            step={step}
            placeholder={placeholder}
            autoFocus={autoFocus}
            autoComplete={autoComplete}
            aria-invalid={fieldState.invalid}
          />
          {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
        </Field>
      )}
    />
  );
};