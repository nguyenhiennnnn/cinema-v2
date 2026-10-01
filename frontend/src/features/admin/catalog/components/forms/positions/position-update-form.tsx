import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { toast } from "@/components/ui/toast";
import { Button } from "@/components/ui/button";
import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";

import { useAdminUpdatePosition } from "../../../hooks/use-admin-position";
import type { PositionDto } from "../../../types";

const positionFormSchema = z.object({
  name: z
    .string()
    .min(1, "Vui lòng nhập tên chức vụ.")
    .max(100, "Tên chức vụ tối đa 100 ký tự."),
});

type PositionFormValues = z.infer<typeof positionFormSchema>;

interface PositionUpdateFormProps {
  position: PositionDto;
  onSuccess: () => void;
}

export const PositionUpdateForm = ({
  position,
  onSuccess,
}: PositionUpdateFormProps) => {
  const updatePosition = useAdminUpdatePosition();

  const form = useForm<PositionFormValues>({
    resolver: zodResolver(positionFormSchema),
    defaultValues: { name: position.name },
  });

  const onSubmit = async (values: PositionFormValues) => {
    await updatePosition.mutateAsync({
      id: position.positionId,
      body: { name: values.name },
    }, {
      onSuccess: () => {

        toast.add({ title: "Đã cập nhật chức vụ.", type: "success" });
        onSuccess();
      }
    });
  };

  return (
    <form
      id="position-update-form"
      onSubmit={form.handleSubmit(onSubmit)}
      className="space-y-4"
    >
      <FieldGroup>
        <FormInputField
          name="name"
          control={form.control}
          label="Tên chức vụ"
          placeholder="Quản lý rạp, Nhân viên soát vé..."
          autoFocus
          autoComplete="off"
        />
      </FieldGroup>

      <div className="flex justify-end gap-2 pt-2">
        <Button
          type="button"
          variant="outline"
          onClick={onSuccess}
          disabled={updatePosition.isPending}
        >
          Hủy
        </Button>
        <Button type="submit" disabled={updatePosition.isPending}>
          {updatePosition.isPending ? "Đang lưu..." : "Lưu thay đổi"}
        </Button>
      </div>
    </form>
  );
};