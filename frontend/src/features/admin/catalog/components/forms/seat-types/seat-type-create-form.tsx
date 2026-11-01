import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";

import { useAdminCreateSeatType } from "../../../hooks/use-admin-seat-type";

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

interface SeatTypeCreateFormProps {
  onSuccess: () => void;
}

export const SeatTypeCreateForm = ({ onSuccess }: SeatTypeCreateFormProps) => {
  const createSeatType = useAdminCreateSeatType();

  const form = useForm<SeatTypeFormValues>({
    resolver: zodResolver(seatTypeFormSchema),
    defaultValues: { name: "", priceMultiplier: 1 },
  });

  const onSubmit = async (values: SeatTypeFormValues) => {
    await createSeatType.mutateAsync({
      name: values.name,
      priceMultiplier: values.priceMultiplier,
    }, {
      onSuccess: () => {
        onSuccess();
      }
    });
  };

  return (
    <form
      id="seat-type-create-form"
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
          placeholder="1.0"
        />
      </FieldGroup>

      <div className="flex justify-end gap-2 pt-2">
        <Button
          type="button"
          variant="outline"
          onClick={onSuccess}
          disabled={createSeatType.isPending}
        >
          Hủy
        </Button>
        <Button type="submit" disabled={createSeatType.isPending}>
          {createSeatType.isPending ? "Đang lưu..." : "Thêm loại ghế"}
        </Button>
      </div>
    </form>
  );
};