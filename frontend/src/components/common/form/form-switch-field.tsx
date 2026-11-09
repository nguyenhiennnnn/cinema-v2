import type { Control, FieldPath, FieldValues } from "react-hook-form";
import { Controller } from "react-hook-form";

import { Field, FieldLabel, useFieldDisabled } from "@/components/ui/field";
import { Switch } from "@/components/ui/switch";

interface FormSwitchFieldProps<TFieldValues extends FieldValues> {
  name: FieldPath<TFieldValues>;
  control: Control<TFieldValues>;
  label: string;
  disabled?: boolean;
}

export const FormSwitchField = <TFieldValues extends FieldValues>({
  name,
  control,
  label,
  disabled,
}: FormSwitchFieldProps<TFieldValues>) => {
  const isDisabled = useFieldDisabled(disabled);

  return (
    <Controller
      name={name}
      control={control}
      render={({ field }) => (
        <Field className="flex flex-row items-center justify-between">
          <FieldLabel>{label}</FieldLabel>
          <Switch checked={field.value} onCheckedChange={field.onChange} disabled={isDisabled} />
        </Field>
      )}
    />
  );
};
