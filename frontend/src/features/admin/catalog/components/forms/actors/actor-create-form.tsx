import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";

import { useAdminCreateActor } from "../../../hooks/use-admin-actor";
import { URL_PLACEHOLDER } from "@/config/default-constants";
import { FormActions } from "@/components/common/form/form-actions";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";

const actorFormSchema = z.object({
  fullName: z
    .string()
    .min(1, "Vui lòng nhập họ tên.")
    .max(255, "Họ tên tối đa 255 ký tự."),
  avatarUrl: z
    .string()
    .url("Đường dẫn ảnh không hợp lệ.")
    .max(500, "Đường dẫn ảnh tối đa 500 ký tự.")
    .optional()
    .or(z.literal("")),
});

type ActorFormValues = z.infer<typeof actorFormSchema>;

interface ActorCreateFormProps {
  onSuccess: () => void;
}

export const ActorCreateForm = ({ onSuccess }: ActorCreateFormProps) => {
  const createActor = useAdminCreateActor();

  const form = useForm<ActorFormValues>({
    resolver: zodResolver(actorFormSchema),
    defaultValues: { fullName: "", avatarUrl: "" },
  });

  const applyServerErrors = useServerFieldErrors(form);

  const onSubmit = (values: ActorFormValues) => {
    createActor.mutate({
      fullName: values.fullName,
      avatarUrl: values.avatarUrl || null,
    }, {
      onSuccess: () => onSuccess(),
      onError: applyServerErrors,
    });
  };

  return (
    <form
      id="actor-create-form"
      onSubmit={form.handleSubmit(onSubmit)}
      className="space-y-4"
    >
      <FieldGroup>
        <FormInputField
          name="fullName"
          control={form.control}
          label="Họ và tên"
          placeholder="Nhập họ và tên diễn viên"
          autoFocus
          autoComplete="off"
        />

        <FormInputField
          name="avatarUrl"
          control={form.control}
          label="Ảnh đại diện (không bắt buộc)"
          placeholder={URL_PLACEHOLDER}
          autoComplete="off"
        />
      </FieldGroup>

      <FormActions
        isPending={createActor.isPending}
        submitLabel="Thêm diễn viên"
        onCancel={onSuccess}
      />
    </form>
  );
};