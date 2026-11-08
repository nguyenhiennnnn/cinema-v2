import type { Control, FieldPath, FieldValues } from "react-hook-form";
import { Controller } from "react-hook-form";

import { Field, FieldError, FieldLabel } from "@/components/ui/field";
import {
  SingleSelectField,
  type SingleSelectFieldProps,
} from "@/components/common/form/single-select-field";
import {
  MultiSelectField,
  type MultiSelectFieldProps,
} from "@/components/common/form/multi-select-field";

interface FormFieldBase<TFieldValues extends FieldValues> {
  name: FieldPath<TFieldValues>;
  control: Control<TFieldValues>;
  label: string;
}

type FormSearchSelectFieldProps<TFieldValues extends FieldValues, TOption> =
  FormFieldBase<TFieldValues> & Omit<SingleSelectFieldProps<TOption>, "value" | "onChange">;

export const FormSearchSelectField = <TFieldValues extends FieldValues, TOption>({
  name,
  control,
  label,
  ...selectProps
}: FormSearchSelectFieldProps<TFieldValues, TOption>) => (
  <Controller
    name={name}
    control={control}
    render={({ field, fieldState }) => (
      <Field data-invalid={fieldState.invalid}>
        <FieldLabel>{label}</FieldLabel>
        <SingleSelectField
          {...selectProps}
          value={field.value}
          onChange={(v) => field.onChange(v)}
        />
        {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
      </Field>
    )}
  />
);

type FormMultiSelectFieldProps<TFieldValues extends FieldValues, TOption> =
  FormFieldBase<TFieldValues> & Omit<MultiSelectFieldProps<TOption>, "value" | "onChange">;

export const FormMultiSelectField = <TFieldValues extends FieldValues, TOption>({
  name,
  control,
  label,
  ...selectProps
}: FormMultiSelectFieldProps<TFieldValues, TOption>) => (
  <Controller
    name={name}
    control={control}
    render={({ field, fieldState }) => (
      <Field data-invalid={fieldState.invalid}>
        <FieldLabel>{label}</FieldLabel>
        <MultiSelectField
          {...selectProps}
          value={field.value ?? []}
          onChange={(values) => field.onChange(values)}
        />
        {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
      </Field>
    )}
  />
);
