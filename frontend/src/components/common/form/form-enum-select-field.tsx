import type { Control, FieldPath, FieldValues } from "react-hook-form";
import { Controller } from "react-hook-form";

import { Field, FieldError, FieldLabel } from "@/components/ui/field";
import { EnumSelectFilter } from "@/components/common/filter/enum-select-filter";
import type { EnumOption } from "@/lib/enum-options";

interface FormEnumSelectFieldProps<TFieldValues extends FieldValues> {
  name: FieldPath<TFieldValues>;
  control: Control<TFieldValues>;
  label: string;
  options: EnumOption[];
  placeholder: string;
  className?: string;
}

/** Select theo danh sách enum cố định; giá trị form là chuỗi ("" khi chưa chọn). */
export const FormEnumSelectField = <TFieldValues extends FieldValues>({
  name,
  control,
  label,
  options,
  placeholder,
  className,
}: FormEnumSelectFieldProps<TFieldValues>) => (
  <Controller
    name={name}
    control={control}
    render={({ field, fieldState }) => (
      <Field data-invalid={fieldState.invalid}>
        <FieldLabel>{label}</FieldLabel>
        <EnumSelectFilter
          value={field.value}
          onChange={(v) => field.onChange(v ?? "")}
          options={options}
          placeholder={placeholder}
          allLabel={placeholder}
          className={className}
        />
        {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
      </Field>
    )}
  />
);
