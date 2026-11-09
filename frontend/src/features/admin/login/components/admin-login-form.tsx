import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useNavigate, useSearchParams } from "react-router";
import { z } from "zod";

import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";
import { FormErrorAlert } from "@/components/common/form/form-error-alert";
import { SubmitButton } from "@/components/common/form/submit-button";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";
import { useAdminLogin } from "@/features/auth/hooks/use-auth";
import { BRAND } from "@/config/brand";
import { getAdminHomePath } from "@/config/routes";

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

  const applyServerErrors = useServerFieldErrors(form);

  const onSubmit = (data: AdminLoginFormValues) => {
    adminLogin(data, {
      onSuccess: (res) => {
        const redirect = searchParams.get("redirect") || getAdminHomePath(res.data.user.role);
        navigate(redirect, { replace: true });
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
      </FieldGroup>

      <FormErrorAlert error={error} preferServerMessage />

      <SubmitButton isPending={isPending} label="Đăng nhập quản trị" className="w-full" />
    </form>
  );
};
