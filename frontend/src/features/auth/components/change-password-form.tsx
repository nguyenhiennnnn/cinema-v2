import type { ReactNode } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";
import { SubmitButton } from "@/components/common/form/submit-button";
import { PasswordStrengthMeter } from "@/components/common/form/password-strength-meter";
import { useChangePassword } from "@/features/client/account/hooks/use-account";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";

import {
  changePasswordSchema,
  type ChangePasswordFormValues,
} from "../schemas/password-schemas";

interface ChangePasswordFormProps {
  layout: "panel" | "stack";
  heading?: string;
  submitLabel: string;
  submitIcon?: ReactNode;
  skipSuccessToast?: boolean;
  onSuccess?: () => void;
}

export const ChangePasswordForm = ({
  layout,
  heading,
  submitLabel,
  submitIcon,
  skipSuccessToast,
  onSuccess,
}: ChangePasswordFormProps) => {
  const changePassword = useChangePassword({ skipSuccessToast });

  const form = useForm<ChangePasswordFormValues>({
    resolver: zodResolver(changePasswordSchema),
    defaultValues: { currentPassword: "", newPassword: "", confirmPassword: "" },
  });

  const applyServerErrors = useServerFieldErrors(form);

  const newPassword = form.watch("newPassword");

  const onSubmit = (values: ChangePasswordFormValues) => {
    changePassword.mutate(
      { currentPassword: values.currentPassword, newPassword: values.newPassword },
      {
        onSuccess: () => {
          form.reset();
          onSuccess?.();
        },
        onError: applyServerErrors,
      },
    );
  };

  const isPanel = layout === "panel";

  const currentPasswordField = (
    <FormInputField
      name="currentPassword"
      control={form.control}
      label="Mật khẩu hiện tại"
      type="password"
      autoComplete="current-password"
    />
  );

  const newPasswordField = (
    <>
      <FormInputField
        name="newPassword"
        control={form.control}
        label="Mật khẩu mới"
        type="password"
        autoComplete="new-password"
      />
      <PasswordStrengthMeter password={newPassword} />
    </>
  );

  const confirmPasswordField = (
    <FormInputField
      name="confirmPassword"
      control={form.control}
      label="Xác nhận mật khẩu mới"
      type="password"
      autoComplete="new-password"
    />
  );

  if (!isPanel) {
    return (
      <form onSubmit={form.handleSubmit(onSubmit)} className="flex flex-col gap-4">
        <FieldGroup disabled={changePassword.isPending} className="gap-4">
          {currentPasswordField}
          <div>{newPasswordField}</div>
          {confirmPasswordField}
        </FieldGroup>

        <SubmitButton
          isPending={changePassword.isPending}
          label={submitLabel}
          icon={submitIcon}
          className="w-full"
        />
      </form>
    );
  }

  return (
    <form onSubmit={form.handleSubmit(onSubmit)} className="bg-card p-5">
      {heading && (
        <h2 className="font-heading text-lg font-semibold text-foreground">{heading}</h2>
      )}

      <FieldGroup
        disabled={changePassword.isPending}
        className="mt-5 grid items-start gap-4 sm:grid-cols-2"
      >
        <div className="sm:col-span-2">{currentPasswordField}</div>
        <div>{newPasswordField}</div>
        {confirmPasswordField}
      </FieldGroup>

      <div className="mt-6 flex justify-end">
        <SubmitButton
          isPending={changePassword.isPending}
          label={submitLabel}
          icon={submitIcon}
        />
      </div>
    </form>
  );
};
