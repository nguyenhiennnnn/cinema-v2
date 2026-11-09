import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Link } from "react-router";
import { CheckCircle2 } from "lucide-react";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import { FormInputField } from "@/components/common/form/form-input-field";
import { SubmitButton } from "@/components/common/form/submit-button";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";
import { useForgotPassword } from "../hooks/use-auth";
import { getDisplayMessage, hasFieldErrors } from "@/lib/error";
import { ROUTES } from "@/config/routes";

const forgotPasswordSchema = z.object({
  email: z.string().min(1, "Vui lòng nhập email.").email("Email không hợp lệ."),
});
type ForgotPasswordFormValues = z.infer<typeof forgotPasswordSchema>;

export const ForgotPasswordForm = () => {
  const [sent, setSent] = useState(false);
  const { mutate: forgotPassword, isPending, error } = useForgotPassword();

  const form = useForm<ForgotPasswordFormValues>({
    resolver: zodResolver(forgotPasswordSchema),
    defaultValues: { email: "" },
  });

  const applyServerErrors = useServerFieldErrors(form);

  if (sent) {
    return (
      <div className="flex flex-col items-center gap-3 text-center">
        <CheckCircle2 className="size-10 text-success" />
        <h2 className="font-heading text-lg font-bold text-foreground">
          Đã gửi yêu cầu
        </h2>
        <p className="text-sm text-muted-foreground">
          Nếu email tồn tại, hướng dẫn đặt lại mật khẩu đã được gửi tới hộp thư
          của bạn.
        </p>
        <Button
          variant="outline"
          render={<Link to={ROUTES.auth.login} />}
          nativeButton={false}
        >
          Về trang đăng nhập
        </Button>
      </div>
    );
  }

  return (
    <form
      onSubmit={form.handleSubmit((data) =>
        forgotPassword(data, {
          onSuccess: () => setSent(true),
          onError: applyServerErrors,
        }),
      )}
      className="flex flex-col gap-4"
    >
      <FormInputField
        name="email"
        control={form.control}
        label="Email"
        type="email"
        autoComplete="email"
        placeholder="you@example.com"
      />

      {error && !hasFieldErrors(error) && (
        <p className="cut-sm bg-destructive/10 px-3 py-2 text-sm text-destructive">
          {getDisplayMessage(error)}
        </p>
      )}

      <SubmitButton
        isPending={isPending}
        label="Gửi liên kết đặt lại mật khẩu"
        className="w-full"
      />

      <p className="text-center text-sm text-muted-foreground">
        Nhớ mật khẩu?{" "}
        <Link
          to={ROUTES.auth.login}
          className="font-medium text-primary hover:underline"
        >
          Đăng nhập
        </Link>
      </p>
    </form>
  );
};