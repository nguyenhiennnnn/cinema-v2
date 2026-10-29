import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useNavigate, useSearchParams } from "react-router";
import { Loader2 } from "lucide-react";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import { FormInputField } from "@/components/common/form/form-input-field";
import { useAdminLogin } from "@/features/auth/hooks/use-auth";
import { extractApiError } from "@/lib/error";
import { BRAND } from "@/config/brand";

const adminLoginSchema = z.object({
  email: z.string().min(1, "Vui lòng nhập email.").email("Email không hợp lệ."),
  password: z.string().min(6, "Mật khẩu phải có ít nhất 6 ký tự."),
});

type AdminLoginFormValues = z.infer<typeof adminLoginSchema>;

export const AdminLoginForm = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const { mutate: adminLogin, isPending, error } = useAdminLogin();

  const form = useForm<AdminLoginFormValues>({
    resolver: zodResolver(adminLoginSchema),
    defaultValues: { email: "", password: "" },
  });

  const onSubmit = (data: AdminLoginFormValues) => {
    adminLogin(data, {
      onSuccess: (res) => {
        const defaultTarget =
          res.data.user.role === "Employee" ? "/admin/bookings/counter" : "/admin/dashboard";
        const redirect = searchParams.get("redirect") || defaultTarget;
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
        label="Email quản trị"
        type="email"
        autoComplete="email"
        placeholder={BRAND.adminEmailPlaceholder}
      />

      <FormInputField
        name="password"
        control={form.control}
        label="Mật khẩu"
        type="password"
        autoComplete="current-password"
        placeholder="••••••••"
      />

      {error && (
        <p className="cut-sm bg-destructive/10 px-3 py-2 text-sm text-destructive">
          {extractApiError(error).message}
        </p>
      )}

      <Button type="submit" disabled={isPending} className="w-full">
        {isPending && <Loader2 className="size-4 animate-spin" />}
        Đăng nhập quản trị
      </Button>
    </form>
  );
};
