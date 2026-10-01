import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Link, useNavigate, useSearchParams } from "react-router";
import { Eye, EyeOff, Loader2 } from "lucide-react";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  InputGroup,
  InputGroupAddon,
  InputGroupButton,
} from "@/components/ui/input-group";
import { FormInputField } from "@/components/common/form/form-input-field";
import { useLogin } from "../hooks/use-auth";
import { extractApiError } from "@/lib/error";
import { GoogleSignInButton } from "./google-sign-in-button";

const loginSchema = z.object({
  email: z.string().min(1, "Vui lòng nhập email.").email("Email không hợp lệ."),
  password: z.string().min(6, "Mật khẩu phải có ít nhất 6 ký tự."),
});

type LoginFormValues = z.infer<typeof loginSchema>;

export const LoginForm = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const [showPassword, setShowPassword] = useState(false);
  const { mutate: login, isPending, error } = useLogin();

  const form = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: { email: "", password: "" },
  });

  const onSubmit = (data: LoginFormValues) => {
    login(data, {
      onSuccess: () => {
        const redirect = searchParams.get("redirect") || "/";
        navigate(redirect, { replace: true });
      },
    });
  };

  return (
    <form
      onSubmit={form.handleSubmit(onSubmit)}
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

      <Controller
        name="password"
        control={form.control}
        render={({ field, fieldState }) => (
          <div className="flex flex-col gap-1.5">
            <div className="flex items-center justify-between">
              <label
                htmlFor="password"
                className="text-sm font-medium text-foreground"
              >
                Mật khẩu
              </label>
              <Link
                to="/auth/forgot-password"
                className="text-xs text-primary hover:underline"
              >
                Quên mật khẩu?
              </Link>
            </div>
            <InputGroup>
              <Input
                {...field}
                id="password"
                type={showPassword ? "text" : "password"}
                autoComplete="current-password"
                placeholder="••••••••"
                aria-invalid={fieldState.invalid}
              />
              <InputGroupAddon align="inline-end">
                <InputGroupButton
                  type="button"
                  variant="ghost"
                  size="icon-xs"
                  onClick={() => setShowPassword((s) => !s)}
                  aria-label={showPassword ? "Ẩn mật khẩu" : "Hiện mật khẩu"}
                >
                  {showPassword ? (
                    <EyeOff className="size-4" />
                  ) : (
                    <Eye className="size-4" />
                  )}
                </InputGroupButton>
              </InputGroupAddon>
            </InputGroup>
            {fieldState.invalid && (
              <p className="text-xs text-destructive">
                {fieldState.error?.message}
              </p>
            )}
          </div>
        )}
      />

      {error && (
        <p className="rounded-md bg-destructive/10 px-3 py-2 text-sm text-destructive">
          {extractApiError(error).message}
        </p>
      )}

      <Button type="submit" disabled={isPending} className="w-full">
        {isPending && <Loader2 className="size-4 animate-spin" />}
        Đăng nhập
      </Button>

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
          to="/auth/register"
          className="font-medium text-primary hover:underline"
        >
          Đăng ký ngay
        </Link>
      </p>
    </form>
  );
};