import type { Control, FieldPath, FieldValues } from "react-hook-form";
import { Controller } from "react-hook-form";
import type { Matcher } from "react-day-picker";

import { Field, FieldError, FieldLabel } from "@/components/ui/field";
import { DatePickerField } from "@/components/common/date-time/date-picker-field";

interface FormDatePickerFieldProps<TFieldValues extends FieldValues> {
  name: FieldPath<TFieldValues>;
  control: Control<TFieldValues>;
  label: string;
  placeholder?: string;
  disabled?: boolean;
  clearable?: boolean;
  captionLayout?: "label" | "dropdown";
  fromYear?: number;
  toYear?: number;
  calendarDisabled?: Matcher | Matcher[];
  emptyValue?: string | null;
}

export const FormDatePickerField = <TFieldValues extends FieldValues>({
  name,
  control,
  label,
  placeholder,
  disabled,
  clearable,
  captionLayout,
  fromYear,
  toYear,
  calendarDisabled,
  emptyValue = "",
}: FormDatePickerFieldProps<TFieldValues>) => {
  const id = `field-${name}`;

  return (
    <Controller
      name={name}
      control={control}
      render={({ field, fieldState }) => (
        <Field data-invalid={fieldState.invalid}>
          <FieldLabel htmlFor={id}>{label}</FieldLabel>
          <DatePickerField
            id={id}
            value={field.value}
            onChange={(v) => field.onChange(v ?? emptyValue)}
            placeholder={placeholder}
            disabled={disabled}
            clearable={clearable}
            captionLayout={captionLayout}
            fromYear={fromYear}
            toYear={toYear}
            calendarDisabled={calendarDisabled}
          />
          {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
        </Field>
      )}
    />
  );
};
