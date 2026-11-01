import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";

import { useAdminUpdateDirector } from "../../../hooks/use-admin-director";
import type { DirectorDto } from "../../../types";
import { URL_PLACEHOLDER } from "@/config/default-constants";

const directorFormSchema = z.object({
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

type DirectorFormValues = z.infer<typeof directorFormSchema>;

interface DirectorUpdateFormProps {
  director: DirectorDto;
  onSuccess: () => void;
}

export const DirectorUpdateForm = ({
  director,
  onSuccess,
}: DirectorUpdateFormProps) => {
  const updateDirector = useAdminUpdateDirector();

  const form = useForm<DirectorFormValues>({
    resolver: zodResolver(directorFormSchema),
    defaultValues: {
      fullName: director.fullName,
      avatarUrl: director.avatarUrl ?? "",
    },
  });

  const onSubmit = async (values: DirectorFormValues) => {
    await updateDirector.mutateAsync({
      id: director.directorId,
      body: {
        fullName: values.fullName,
        avatarUrl: values.avatarUrl || null,
      },
    }, {
      onSuccess: () => {
        onSuccess();
      }
    });
  };

  return (
    <form
      id="director-update-form"
      onSubmit={form.handleSubmit(onSubmit)}
      className="space-y-4"
    >
      <FieldGroup>
        <FormInputField
          name="fullName"
          control={form.control}
          label="Họ và tên"
          placeholder="Nhập họ và tên đạo diễn"
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

      <div className="flex justify-end gap-2 pt-2">
        <Button
          type="button"
          variant="outline"
          onClick={onSuccess}
          disabled={updateDirector.isPending}
        >
          Hủy
        </Button>
        <Button type="submit" disabled={updateDirector.isPending}>
          {updateDirector.isPending ? "Đang lưu..." : "Lưu thay đổi"}
        </Button>
      </div>
    </form>
  );
};