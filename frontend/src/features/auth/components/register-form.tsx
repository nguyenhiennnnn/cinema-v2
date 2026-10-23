import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Link } from "react-router";
import { CheckCircle2, Loader2 } from "lucide-react";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import { FormInputField } from "@/components/common/form/form-input-field";
import { useRegister } from "../hooks/use-auth";
import { getDisplayMessage, getFieldErrors } from "@/lib/error";
import { passwordSchema } from "@/lib/validators";
import type { RegisterRequest } from "../types";
import { PasswordStrengthMeter } from "@/features/client/account/components/password-strength-meter";

const registerSchema = z
  .object({
    fullName: z.string().min(2, "Họ tên phải có ít nhất 2 ký tự."),
    email: z
      .string()
      .min(1, "Vui lòng nhập email.")
      .email("Email không hợp lệ."),
    phone: z
      .string()
      .optional()
      .refine(
        (v) => !v || /^0\d{9,10}$/.test(v),
        "Số điện thoại không hợp lệ.",
      ),
    password: passwordSchema,
    confirmPassword: z.string().min(1, "Vui lòng xác nhận mật khẩu."),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: "Mật khẩu xác nhận không khớp.",
    path: ["confirmPassword"],
  });

type RegisterFormValues = z.infer<typeof registerSchema>;

export const RegisterForm = () => {
  const [submittedEmail, setSubmittedEmail] = useState<string | null>(null);
  const { mutate: registerUser, isPending, error } = useRegister();

  const form = useForm<RegisterFormValues>({
    resolver: zodResolver(registerSchema),
    defaultValues: {
      fullName: "",
      email: "",
      phone: "",
      password: "",
      confirmPassword: "",
    },
  });

  const onSubmit = (data: RegisterFormValues) => {
    const payload: RegisterRequest = {
      fullName: data.fullName,
      email: data.email,
      password: data.password,
      phone: data.phone || null,
    };
    registerUser(payload, {
      onSuccess: () => setSubmittedEmail(data.email),
      onError: (err) => {
        const fieldErrors = getFieldErrors(err);
        Object.entries(fieldErrors).forEach(([field, message]) => {
          form.setError(field as keyof RegisterFormValues, { message });
        });
      },
    });
  };

  const password = form.watch("password");

  if (submittedEmail) {
    return (
      <div className="flex flex-col items-center gap-3 text-center">
        <CheckCircle2 className="size-10 text-success" />
        <h2 className="font-heading text-lg font-bold text-foreground">
          Đăng ký thành công
        </h2>
        <p className="text-sm text-muted-foreground">
          Vui lòng kiểm tra hộp thư{" "}
          <span className="font-medium text-foreground">{submittedEmail}</span>{" "}
          để xác thực tài khoản.
        </p>
        <Button variant="outline">
          <Link to="/auth/login">Về trang đăng nhập</Link>
        </Button>
      </div>
    );
  }

  return (
    <form
      onSubmit={form.handleSubmit(onSubmit)}
      className="flex flex-col gap-4"
    >
      <FormInputField
        name="fullName"
        control={form.control}
        label="Họ và tên"
        placeholder="Nguyễn Văn A"
      />

      <FormInputField
        name="email"
        control={form.control}
        label="Email"
        type="email"
        autoComplete="email"
        placeholder="you@example.com"
      />

      <FormInputField
        name="phone"
        control={form.control}
        label="Số điện thoại (không bắt buộc)"
        placeholder="0901234567"
      />

      <div className="flex flex-col gap-2">
        <FormInputField
          name="password"
          control={form.control}
          label="Mật khẩu"
          type="password"
          autoComplete="new-password"
          placeholder="••••••••"
        />
        <PasswordStrengthMeter password={password} />
      </div>

      <FormInputField
        name="confirmPassword"
        control={form.control}
        label="Xác nhận mật khẩu"
        type="password"
        autoComplete="new-password"
        placeholder="••••••••"
      />

      {error && Object.keys(getFieldErrors(error)).length === 0 && (
        <p className="cut-sm bg-destructive/10 px-3 py-2 text-sm text-destructive">
          {getDisplayMessage(error)}
        </p>
      )}

      <Button type="submit" disabled={isPending} className="w-full">
        {isPending && <Loader2 className="size-4 animate-spin" />}
        Đăng ký
      </Button>

      <p className="text-center text-sm text-muted-foreground">
        Đã có tài khoản?{" "}
        <Link
          to="/auth/login"
          className="font-medium text-primary hover:underline"
        >
          Đăng nhập
        </Link>
      </p>
    </form>
  );
};