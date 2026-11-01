import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";

import { useAdminCreatePosition } from "../../../hooks/use-admin-position";

const positionFormSchema = z.object({
  name: z
    .string()
    .min(1, "Vui lòng nhập tên chức vụ.")
    .max(100, "Tên chức vụ tối đa 100 ký tự."),
});

type PositionFormValues = z.infer<typeof positionFormSchema>;

interface PositionCreateFormProps {
  onSuccess: () => void;
}

export const PositionCreateForm = ({ onSuccess }: PositionCreateFormProps) => {
  const createPosition = useAdminCreatePosition();

  const form = useForm<PositionFormValues>({
    resolver: zodResolver(positionFormSchema),
    defaultValues: { name: "" },
  });

  const onSubmit = async (values: PositionFormValues) => {
    await createPosition.mutateAsync({ name: values.name }, {
      onSuccess: () => {
        onSuccess();
      }
    });
  };

  return (
    <form
      id="position-create-form"
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
          disabled={createPosition.isPending}
        >
          Hủy
        </Button>
        <Button type="submit" disabled={createPosition.isPending}>
          {createPosition.isPending ? "Đang lưu..." : "Thêm chức vụ"}
        </Button>
      </div>
    </form>
  );
};