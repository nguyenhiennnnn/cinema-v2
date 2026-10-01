import type { Control, FieldPath, FieldValues } from "react-hook-form";
import { Controller } from "react-hook-form";
import { Field, FieldError, FieldLabel } from "@/components/ui/field";
import { SelectField } from "@/components/common/select-field";

interface FormSelectFieldProps<
  TFieldValues extends FieldValues,
  TName extends FieldPath<TFieldValues>,
  TOption,
> {
  name: TName;
  control: Control<TFieldValues>;
  label?: string;
  placeholder?: string;
  options: TOption[];
  getOptionValue: (option: TOption) => string | number;
  getOptionLabel: (option: TOption) => string;
  disabled?: boolean;
  className?: string;
}

export const FormSelectField =
  <TFieldValues extends FieldValues,
    TName extends FieldPath<TFieldValues>,
    TOption,
  >({
    name,
    control,
    label,
    placeholder,
    options,
    getOptionValue,
    getOptionLabel,
    disabled,
    className,
  }: FormSelectFieldProps<TFieldValues, TName, TOption>) => {
    return (
      <Controller
        name={name}
        control={control}
        render={({ field, fieldState }) => (
          <Field data-invalid={fieldState.invalid} className="group">
            {label && <FieldLabel htmlFor={field.name}>{label}</FieldLabel>}
            <SelectField
              value={field.value ?? null}
              onChange={(value) => field.onChange(value || null)}
              options={options}
              getOptionValue={getOptionValue}
              getOptionLabel={getOptionLabel}
              placeholder={placeholder}
              disabled={disabled}
              className={className}
            />
            {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
          </Field>
        )}
      />
    );
  };