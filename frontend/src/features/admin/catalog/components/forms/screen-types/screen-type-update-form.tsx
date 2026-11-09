import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";

import { useAdminUpdateScreenType } from "../../../hooks/use-admin-screen-type";
import type { ScreenTypeDto } from "../../../types";
import { FormActions } from "@/components/common/form/form-actions";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";

const screenTypeFormSchema = z.object({
  name: z
    .string()
    .min(1, "Vui lòng nhập tên loại màn hình.")
    .max(100, "Tên loại màn hình tối đa 100 ký tự."),
});

type ScreenTypeFormValues = z.infer<typeof screenTypeFormSchema>;

interface ScreenTypeUpdateFormProps {
  screenType: ScreenTypeDto;
  onSuccess: () => void;
}

export const ScreenTypeUpdateForm = ({
  screenType,
  onSuccess,
}: ScreenTypeUpdateFormProps) => {
  const updateScreenType = useAdminUpdateScreenType();

  const form = useForm<ScreenTypeFormValues>({
    resolver: zodResolver(screenTypeFormSchema),
    defaultValues: { name: screenType.name },
  });

  const applyServerErrors = useServerFieldErrors(form);

  const onSubmit = (values: ScreenTypeFormValues) => {
    updateScreenType.mutate({
      id: screenType.screenTypeId,
      body: values,
    }, {
      onSuccess: () => onSuccess(),
      onError: applyServerErrors,
    });
  };

  return (
    <form
      id="screen-type-update-form"
      onSubmit={form.handleSubmit(onSubmit)}
      className="space-y-4"
    >
      <FieldGroup disabled={updateScreenType.isPending}>
        <FormInputField
          name="name"
          control={form.control}
          label="Tên loại màn hình"
          placeholder="2D, 3D, IMAX Laser, ScreenX..."
          autoFocus
          autoComplete="off"
        />
      </FieldGroup>

      <FormActions
        isPending={updateScreenType.isPending}
        submitLabel="Lưu thay đổi"
        onCancel={onSuccess}
      />
    </form>
  );
};