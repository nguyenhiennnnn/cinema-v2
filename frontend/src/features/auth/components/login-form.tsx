import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Link, useNavigate, useSearchParams } from "react-router";
import { z } from "zod";

import { PasswordInput } from "@/components/ui/input";
import { Field, FieldError, FieldGroup, FieldLabel } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";
import { FormErrorAlert } from "@/components/common/form/form-error-alert";
import { SubmitButton } from "@/components/common/form/submit-button";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";
import { useLogin } from "../hooks/use-auth";
import { GoogleSignInButton } from "./google-sign-in-button";
import { ROUTES } from "@/config/routes";
import { getSafeRedirect } from "@/lib/utils";

const loginSchema = z.object({
  email: z.string().min(1, "Vui lòng nhập email.").email("Email không hợp lệ."),
  password: z.string().min(6, "Mật khẩu phải có ít nhất 6 ký tự."),
});

type LoginFormValues = z.infer<typeof loginSchema>;

export const LoginForm = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const { mutate: login, isPending, error } = useLogin();

  const form = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: { email: "", password: "" },
  });

  const applyServerErrors = useServerFieldErrors(form);

  const onSubmit = (data: LoginFormValues) => {
    login(data, {
      onSuccess: () => {
        navigate(getSafeRedirect(searchParams.get("redirect")), { replace: true });
      },
      onError: applyServerErrors,
    });
  };

  return (
    <form
      onSubmit={form.handleSubmit(onSubmit)}
      className="flex flex-col gap-4"
    >
      <FieldGroup disabled={isPending} className="gap-4">
        <FormInputField
          name="email"
          control={form.control}
          label="Email"
          type="email"
          autoComplete="email"
          placeholder="you@example.com"
        />

        <Controller
          name="password"
          control={form.control}
          render={({ field, fieldState }) => (
            <Field data-invalid={fieldState.invalid}>
              <div className="flex items-center justify-between">
                <FieldLabel htmlFor="password">Mật khẩu</FieldLabel>
                <Link
                  to="/auth/forgot-password"
                  className="text-xs text-muted-foreground transition-colors hover:text-primary hover:underline"
                >
                  Quên mật khẩu?
                </Link>
              </div>
              <PasswordInput
                {...field}
                id="password"
                autoComplete="current-password"
                placeholder="••••••••"
                aria-invalid={fieldState.invalid}
              />
              {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
            </Field>
          )}
        />
      </FieldGroup>

      <FormErrorAlert error={error} preferServerMessage />

      <SubmitButton isPending={isPending} label="Đăng nhập" className="w-full" />

      <div className="relative flex items-center py-1">
        <div className="grow border-t border-border" />
        <span className="mx-3 shrink-0 text-xs text-muted-foreground">
          Hoặc
        </span>
        <div className="grow border-t border-border" />
      </div>

      <GoogleSignInButton />

      <p className="text-center text-sm text-muted-foreground">
        Chưa có tài khoản?{" "}
        <Link
          to={ROUTES.auth.register}
          className="font-medium text-primary hover:underline"
        >
          Đăng ký ngay
        </Link>
      </p>
    </form>
  );
};
