import type { Control, FieldPath, FieldValues } from "react-hook-form";
import { Controller } from "react-hook-form";
import type { Matcher } from "react-day-picker";

import { Field, FieldError, FieldLabel } from "@/components/ui/field";
import { DateTimePickerField } from "@/components/common/date-time/datetime-picker-field";

interface FormDateTimePickerFieldProps<TFieldValues extends FieldValues> {
  name: FieldPath<TFieldValues>;
  control: Control<TFieldValues>;
  label: string;
  placeholder?: string;
  disabled?: boolean;
  clearable?: boolean;
  calendarDisabled?: Matcher | Matcher[];
  minuteStep?: number;
  emptyValue?: string | null;
}

export const FormDateTimePickerField = <TFieldValues extends FieldValues>({
  name,
  control,
  label,
  placeholder,
  disabled,
  clearable,
  calendarDisabled,
  minuteStep,
  emptyValue = "",
}: FormDateTimePickerFieldProps<TFieldValues>) => {
  const id = `field-${name}`;

  return (
    <Controller
      name={name}
      control={control}
      render={({ field, fieldState }) => (
        <Field data-invalid={fieldState.invalid}>
          <FieldLabel htmlFor={id}>{label}</FieldLabel>
          <DateTimePickerField
            id={id}
            value={field.value}
            onChange={(v) => field.onChange(v ?? emptyValue)}
            placeholder={placeholder}
            disabled={disabled}
            clearable={clearable}
            calendarDisabled={calendarDisabled}
            minuteStep={minuteStep}
          />
          {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
        </Field>
      )}
    />
  );
};
