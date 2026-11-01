import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";

import { useAdminCreateScreenType } from "../../../hooks/use-admin-screen-type";

const screenTypeFormSchema = z.object({
  name: z
    .string()
    .min(1, "Vui lòng nhập tên loại màn hình.")
    .max(100, "Tên loại màn hình tối đa 100 ký tự."),
});

type ScreenTypeFormValues = z.infer<typeof screenTypeFormSchema>;

interface ScreenTypeCreateFormProps {
  onSuccess: () => void;
}

export const ScreenTypeCreateForm = ({
  onSuccess,
}: ScreenTypeCreateFormProps) => {
  const createScreenType = useAdminCreateScreenType();

  const form = useForm<ScreenTypeFormValues>({
    resolver: zodResolver(screenTypeFormSchema),
    defaultValues: { name: "" },
  });

  const onSubmit = async (values: ScreenTypeFormValues) => {
    await createScreenType.mutateAsync(values, {
      onSuccess: () => {
        onSuccess();
      }
    });
  };

  return (
    <form
      id="screen-type-create-form"
      onSubmit={form.handleSubmit(onSubmit)}
      className="space-y-4"
    >
      <FieldGroup>
        <FormInputField
          name="name"
          control={form.control}
          label="Tên loại màn hình"
          placeholder="2D, 3D, IMAX Laser, ScreenX..."
          autoFocus
          autoComplete="off"
        />
      </FieldGroup>

      <div className="flex justify-end gap-2 pt-2">
        <Button
          type="button"
          variant="outline"
          onClick={onSuccess}
          disabled={createScreenType.isPending}
        >
          Hủy
        </Button>
        <Button type="submit" disabled={createScreenType.isPending}>
          {createScreenType.isPending ? "Đang lưu..." : "Thêm loại màn hình"}
        </Button>
      </div>
    </form>
  );
};