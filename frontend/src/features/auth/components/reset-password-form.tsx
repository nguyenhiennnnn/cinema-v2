import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Link, useNavigate, useSearchParams } from "react-router";
import { AlertTriangle, Loader2 } from "lucide-react";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import { FormInputField } from "@/components/common/form/form-input-field";
import { useResetPassword } from "../hooks/use-auth";
import { getDisplayMessage } from "@/lib/error";
import { passwordSchema } from "@/lib/validators";
import { PasswordStrengthMeter } from "@/features/client/account/components/password-strength-meter";

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
        <Button variant="outline">
          <Link to="/auth/forgot-password">Yêu cầu liên kết mới</Link>
        </Button>
      </div>
    );
  }

  const onSubmit = (data: ResetPasswordFormValues) => {
    resetPassword(
      { token, newPassword: data.newPassword },
      { onSuccess: () => navigate("/auth/login", { replace: true }) },
    );
  };

  const newPassword = form.watch("newPassword");

  return (
    <form
      onSubmit={form.handleSubmit(onSubmit)}
      className="flex flex-col gap-4"
    >
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

      {error && (
        <p className="cut-sm bg-destructive/10 px-3 py-2 text-sm text-destructive">
          {getDisplayMessage(error)}
        </p>
      )}

      <Button type="submit" disabled={isPending} className="w-full">
        {isPending && <Loader2 className="size-4 animate-spin" />}
        Đặt lại mật khẩu
      </Button>
    </form>
  );
};