import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";

import { useAdminUpdateActor } from "@/features/admin/catalog/hooks/use-admin-actor";
import type { ActorDto } from "@/features/admin/catalog/types";
import { URL_PLACEHOLDER, PERSON_NAME_MAX_LENGTH, IMAGE_URL_MAX_LENGTH, FORM_SAVE_CHANGES_LABEL } from "@/config/default-constants";
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

interface ActorUpdateFormProps {
  actor: ActorDto;
  onSuccess: () => void;
}

export const ActorUpdateForm = ({ actor, onSuccess }: ActorUpdateFormProps) => {
  const updateActor = useAdminUpdateActor();

  const form = useForm<ActorFormValues>({
    resolver: zodResolver(actorFormSchema),
    defaultValues: {
      fullName: actor.fullName,
      avatarUrl: actor.avatarUrl ?? "",
    },
  });

  const applyServerErrors = useServerFieldErrors(form);

  const onSubmit = (values: ActorFormValues) => {
    updateActor.mutate({
      id: actor.actorId,
      body: {
        fullName: values.fullName,
        avatarUrl: values.avatarUrl || null,
      },
    }, {
      onSuccess: () => onSuccess(),
      onError: applyServerErrors,
    });
  };

  return (
    <form
      id="actor-update-form"
      onSubmit={form.handleSubmit(onSubmit)}
      className="space-y-4"
    >
      <FieldGroup disabled={updateActor.isPending}>
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
        isPending={updateActor.isPending}
        isDirty={form.formState.isDirty}
        submitLabel={FORM_SAVE_CHANGES_LABEL}
        onCancel={onSuccess}
      />
    </form>
  );
};