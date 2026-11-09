import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";

import { useAdminUpdateSeatType } from "../../../hooks/use-admin-seat-type";
import type { SeatTypeDto } from "../../../types";
import { FormActions } from "@/components/common/form/form-actions";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";

const seatTypeFormSchema = z.object({
  name: z
    .string()
    .min(1, "Vui lòng nhập tên loại ghế.")
    .max(100, "Tên loại ghế tối đa 100 ký tự."),
  priceMultiplier: z.coerce
    .number({ message: "Hệ số giá phải là số." })
    .min(0.1, "Hệ số giá tối thiểu là 0.1."),
});

type SeatTypeFormValues = z.infer<typeof seatTypeFormSchema>;

interface SeatTypeUpdateFormProps {
  seatType: SeatTypeDto;
  onSuccess: () => void;
}

export const SeatTypeUpdateForm = ({
  seatType,
  onSuccess,
}: SeatTypeUpdateFormProps) => {
  const updateSeatType = useAdminUpdateSeatType();

  const form = useForm<SeatTypeFormValues>({
    resolver: zodResolver(seatTypeFormSchema),
    defaultValues: {
      name: seatType.name,
      priceMultiplier: seatType.priceMultiplier,
    },
  });

  const applyServerErrors = useServerFieldErrors(form);

  const onSubmit = (values: SeatTypeFormValues) => {
    updateSeatType.mutate({
      id: seatType.seatTypeId,
      body: {
        name: values.name,
        priceMultiplier: values.priceMultiplier,
      },
    }, {
      onSuccess: () => onSuccess(),
      onError: applyServerErrors,
    });
  };

  return (
    <form
      id="seat-type-update-form"
      onSubmit={form.handleSubmit(onSubmit)}
      className="space-y-4"
    >
      <FieldGroup>
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
        />
      </FieldGroup>

      <FormActions
        isPending={updateSeatType.isPending}
        submitLabel="Lưu thay đổi"
        onCancel={onSuccess}
      />
    </form>
  );
};