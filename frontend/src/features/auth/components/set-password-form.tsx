import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";
import { FormErrorAlert } from "@/components/common/form/form-error-alert";
import { SubmitButton } from "@/components/common/form/submit-button";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";
import { PasswordStrengthMeter } from "@/components/common/form/password-strength-meter";

import { useResetPassword } from "../hooks/use-auth";
import { setPasswordSchema, type SetPasswordFormValues } from "../schemas/password-schemas";

interface SetPasswordFormProps {
  token: string;
  submitLabel: string;
  newPasswordLabel?: string;
  confirmPasswordLabel?: string;
  skipSuccessToast?: boolean;
  onSuccess: () => void;
  onError?: (error: unknown) => void;
}

export const SetPasswordForm = ({
  token,
  submitLabel,
  newPasswordLabel = "Mật khẩu mới",
  confirmPasswordLabel = "Xác nhận mật khẩu mới",
  skipSuccessToast,
  onSuccess,
  onError,
}: SetPasswordFormProps) => {
  const { mutate: resetPassword, isPending, error } = useResetPassword({ skipSuccessToast });

  const form = useForm<SetPasswordFormValues>({
    resolver: zodResolver(setPasswordSchema),
    defaultValues: { newPassword: "", confirmPassword: "" },
  });

  const applyServerErrors = useServerFieldErrors(form);

  const onSubmit = (data: SetPasswordFormValues) => {
    resetPassword(
      { token, newPassword: data.newPassword },
      {
        onSuccess: () => onSuccess(),
        onError: (err) => {
          applyServerErrors(err);
          onError?.(err);
        },
      },
    );
  };

  const newPassword = form.watch("newPassword");

  return (
    <form onSubmit={form.handleSubmit(onSubmit)} className="flex flex-col gap-4">
      <FieldGroup disabled={isPending} className="gap-4">
        <FormInputField
          name="newPassword"
          control={form.control}
          label={newPasswordLabel}
          type="password"
          autoComplete="new-password"
          placeholder="••••••••"
        />

        <FormInputField
          name="confirmPassword"
          control={form.control}
          label={confirmPasswordLabel}
          type="password"
          autoComplete="new-password"
          placeholder="••••••••"
        />

        <PasswordStrengthMeter password={newPassword} />
      </FieldGroup>

      <FormErrorAlert error={error} />

      <SubmitButton isPending={isPending} label={submitLabel} className="w-full" />
    </form>
  );
};
