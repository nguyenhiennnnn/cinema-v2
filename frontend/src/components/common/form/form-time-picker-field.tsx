import type { Control, FieldPath, FieldValues } from "react-hook-form";
import { Controller } from "react-hook-form";

import { Field, FieldError, FieldLabel } from "@/components/ui/field";
import { TimePickerField } from "@/components/common/date-time/time-picker-field";

interface FormTimePickerFieldProps<TFieldValues extends FieldValues> {
  name: FieldPath<TFieldValues>;
  control: Control<TFieldValues>;
  label: string;
  disabled?: boolean;
  minuteStep?: number;
  emptyValue?: string | null;
}

export const FormTimePickerField = <TFieldValues extends FieldValues>({
  name,
  control,
  label,
  disabled,
  minuteStep,
  emptyValue = "",
}: FormTimePickerFieldProps<TFieldValues>) => {
  const id = `field-${name}`;

  return (
    <Controller
      name={name}
      control={control}
      render={({ field, fieldState }) => (
        <Field data-invalid={fieldState.invalid}>
          <FieldLabel htmlFor={id}>{label}</FieldLabel>
          <TimePickerField
            id={id}
            value={field.value}
            onChange={(v) => field.onChange(v ?? emptyValue)}
            disabled={disabled}
            minuteStep={minuteStep}
            aria-invalid={fieldState.invalid}
          />
          {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
        </Field>
      )}
    />
  );
};
