import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";

import { useAdminCreateActor } from "@/features/admin/catalog/hooks/use-admin-actor";
import { URL_PLACEHOLDER, PERSON_NAME_MAX_LENGTH, IMAGE_URL_MAX_LENGTH } from "@/config/default-constants";
import { FormActions } from "@/components/common/form/form-actions";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";

const actorFormSchema = z.object({
  fullName: z
    .string()
    .min(1, "Vui lòng nhập họ tên.")
    .max(PERSON_NAME_MAX_LENGTH, `Họ tên tối đa ${PERSON_NAME_MAX_LENGTH} ký tự.`),
  avatarUrl: z
    .string()
    .url("Đường dẫn ảnh không hợp lệ.")
    .max(IMAGE_URL_MAX_LENGTH, `Đường dẫn ảnh tối đa ${IMAGE_URL_MAX_LENGTH} ký tự.`)
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
      <FieldGroup disabled={createActor.isPending}>
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
        isDirty={form.formState.isDirty}
        submitLabel="Thêm diễn viên"
        onCancel={onSuccess}
      />
    </form>
  );
};