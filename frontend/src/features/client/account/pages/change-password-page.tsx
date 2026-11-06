import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { Loader2, KeyRound } from "lucide-react";

import { Button } from "@/components/ui/button";
import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";
import { PasswordStrengthMeter } from "@/features/client/account/components/password-strength-meter";
import { useChangePassword } from "@/features/client/account/hooks/use-account";

const changePasswordFormSchema = z
  .object({
    currentPassword: z.string().min(1, "Vui lòng nhập mật khẩu hiện tại."),
    newPassword: z.string()
      .min(8, "Mật khẩu phải có ít nhất 8 ký tự.")
      .refine((v) => /[a-z]/.test(v) && /[A-Z]/.test(v), {
        message: "Mật khẩu phải có cả chữ hoa và chữ thường.",
      })
      .refine(
        (v) => {
          let score = 0;
          if (v.length >= 8) score++;
          if (/[a-z]/.test(v) && /[A-Z]/.test(v)) score++;
          if (/\d/.test(v)) score++;
          if (/[^a-zA-Z0-9]/.test(v)) score++;
          return score >= 3;
        },
        { message: "Mật khẩu chưa đủ mạnh. Vui lòng thêm số hoặc ký tự đặc biệt." },
      ),
    confirmPassword: z.string().min(1, "Vui lòng xác nhận mật khẩu mới."),
  })
  .refine((data) => data.newPassword === data.confirmPassword, {
    message: "Mật khẩu xác nhận không khớp.",
    path: ["confirmPassword"],
  });

type ChangePasswordFormValues = z.infer<typeof changePasswordFormSchema>;

const ChangePasswordPage = () => {
  const changePassword = useChangePassword();

  const form = useForm<ChangePasswordFormValues>({
    resolver: zodResolver(changePasswordFormSchema),
    defaultValues: {
      currentPassword: "",
      newPassword: "",
      confirmPassword: "",
    },
  });

  const newPassword = form.watch("newPassword");

  const onSubmit = (values: ChangePasswordFormValues) => {
    changePassword.mutate(
      { currentPassword: values.currentPassword, newPassword: values.newPassword },
      { onSuccess: () => form.reset() },
    );
  };

  return (
    <form onSubmit={form.handleSubmit(onSubmit)} className="border border-border bg-card p-5">
      <h2 className="font-heading text-lg font-semibold text-foreground">Đổi mật khẩu</h2>

      <FieldGroup className="mt-5 grid items-start gap-4 sm:grid-cols-2">
        <div className="sm:col-span-2">
          <FormInputField
            name="currentPassword"
            control={form.control}
            label="Mật khẩu hiện tại"
            type="password"
            autoComplete="current-password"
          />
        </div>

        <div>
          <FormInputField
            name="newPassword"
            control={form.control}
            label="Mật khẩu mới"
            type="password"
            autoComplete="new-password"
          />
          <PasswordStrengthMeter password={newPassword} />
        </div>

        <FormInputField
          name="confirmPassword"
          control={form.control}
          label="Xác nhận mật khẩu mới"
          type="password"
          autoComplete="new-password"
        />
      </FieldGroup>

      <div className="mt-6 flex justify-end">
        <Button type="submit" disabled={changePassword.isPending}>
          {changePassword.isPending ? <Loader2 className="size-4 animate-spin" /> : <KeyRound className="size-4" />}
          <span>Cập nhật mật khẩu</span>
        </Button>
      </div>
    </form>
  );
};

export default ChangePasswordPage;