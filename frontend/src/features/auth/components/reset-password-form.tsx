import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Link, useNavigate, useSearchParams } from "react-router";
import { AlertTriangle } from "lucide-react";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";
import { FormErrorAlert } from "@/components/common/form/form-error-alert";
import { SubmitButton } from "@/components/common/form/submit-button";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";
import { useResetPassword } from "../hooks/use-auth";
import { passwordSchema } from "@/lib/validators";
import { PasswordStrengthMeter } from "@/features/client/account/components/password-strength-meter";
import { ROUTES } from "@/config/routes";

const resetPasswordSchema = z
  .object({
    newPassword: passwordSchema,
    confirmPassword: z.string().min(1, "Vui lòng xác nhận mật khẩu."),
  })
  .refine((data) => data.newPassword === data.confirmPassword, {
    message: "Mật khẩu xác nhận không khớp.",
    path: ["confirmPassword"],
  });

type ResetPasswordFormValues = z.infer<typeof resetPasswordSchema>;

export const ResetPasswordForm = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const token = searchParams.get("token") ?? "";
  const { mutate: resetPassword, isPending, error } = useResetPassword();

  const form = useForm<ResetPasswordFormValues>({
    resolver: zodResolver(resetPasswordSchema),
    defaultValues: { newPassword: "", confirmPassword: "" },
  });

  const applyServerErrors = useServerFieldErrors(form);



  if (!token) {
    return (
      <div className="flex flex-col items-center gap-3 text-center">
        <AlertTriangle className="size-10 text-destructive" />
        <h2 className="font-heading text-lg font-bold text-foreground">
          Liên kết không hợp lệ
        </h2>
        <p className="text-sm text-muted-foreground">
          Thiếu mã đặt lại mật khẩu trong đường dẫn.
        </p>
        <Button
          variant="outline"
          render={<Link to="/auth/forgot-password" />}
          nativeButton={false}
        >
          Yêu cầu liên kết mới
        </Button>
      </div>
    );
  }

  const onSubmit = (data: ResetPasswordFormValues) => {
    resetPassword(
      { token, newPassword: data.newPassword },
      {
        onSuccess: () => navigate(ROUTES.auth.login, { replace: true }),
        onError: applyServerErrors,
      },
    );
  };

  const newPassword = form.watch("newPassword");

  return (
    <form
      onSubmit={form.handleSubmit(onSubmit)}
      className="flex flex-col gap-4"
    >
      <FieldGroup disabled={isPending} className="gap-4">
        <FormInputField
          name="newPassword"
          control={form.control}
          label="Mật khẩu mới"
          type="password"
          autoComplete="new-password"
          placeholder="••••••••"
        />

        <FormInputField
          name="confirmPassword"
          control={form.control}
          label="Xác nhận mật khẩu mới"
          type="password"
          autoComplete="new-password"
          placeholder="••••••••"
        />

        <PasswordStrengthMeter password={newPassword} />
      </FieldGroup>

      <FormErrorAlert error={error} />

      <SubmitButton isPending={isPending} label="Đặt lại mật khẩu" className="w-full" />
    </form>
  );
};