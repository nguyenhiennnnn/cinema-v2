import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { NAME_MAX_LENGTH } from "@/config/default-constants";

import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";

import { useAdminCreateSeatType } from "@/features/admin/catalog/hooks/use-admin-seat-type";
import { FormActions } from "@/components/common/form/form-actions";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";

const seatTypeFormSchema = z.object({
  name: z
    .string()
    .min(1, "Vui lòng nhập tên loại ghế.")
    .max(NAME_MAX_LENGTH, `Tên loại ghế tối đa ${NAME_MAX_LENGTH} ký tự.`),
  priceMultiplier: z.coerce
    .number({ message: "Hệ số giá phải là số." })
    .min(0.1, "Hệ số giá tối thiểu là 0.1."),
});

type SeatTypeFormValues = z.infer<typeof seatTypeFormSchema>;

interface SeatTypeCreateFormProps {
  onSuccess: () => void;
}

export const SeatTypeCreateForm = ({ onSuccess }: SeatTypeCreateFormProps) => {
  const createSeatType = useAdminCreateSeatType();

  const form = useForm<SeatTypeFormValues>({
    resolver: zodResolver(seatTypeFormSchema),
    defaultValues: { name: "", priceMultiplier: 1 },
  });

  const applyServerErrors = useServerFieldErrors(form);

  const onSubmit = (values: SeatTypeFormValues) => {
    createSeatType.mutate({
      name: values.name,
      priceMultiplier: values.priceMultiplier,
    }, {
      onSuccess: () => onSuccess(),
      onError: applyServerErrors,
    });
  };

  return (
    <form
      id="seat-type-create-form"
      onSubmit={form.handleSubmit(onSubmit)}
      className="space-y-4"
    >
      <FieldGroup disabled={createSeatType.isPending}>
        <FormInputField
          name="name"
          control={form.control}
          label="Tên loại ghế"
          placeholder="Thường, VIP, Sweetbox..."
          autoFocus
          autoComplete="off"
        />

        <FormInputField
          name="priceMultiplier"
          control={form.control}
          label="Hệ số giá"
          type="number"
          min={0.1}
          step={0.1}
          placeholder="1.0"
        />
      </FieldGroup>

      <FormActions
        isPending={createSeatType.isPending}
        isDirty={form.formState.isDirty}
        submitLabel="Thêm loại ghế"
        onCancel={onSuccess}
      />
    </form>
  );
};