import { useEffect, useRef } from "react";
import { Link, useSearchParams } from "react-router";
import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { CheckCircle2, XCircle } from "lucide-react";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { PageLoader, Spinner } from "@/components/common/spinner";
import { useConfirmEmail, useResendConfirmation } from "../hooks/use-auth";
import { getDisplayMessage } from "@/lib/error";
import { ROUTES } from "@/config/routes";

const resendSchema = z.object({
  email: z.string().min(1, "Vui lòng nhập email.").email("Email không hợp lệ."),
});
type ResendFormValues = z.infer<typeof resendSchema>;

const ConfirmEmailPage = () => {
  const [searchParams] = useSearchParams();
  const token = searchParams.get("token") ?? "";
  const calledRef = useRef(false);

  const {
    mutate: confirmEmail,
    isPending,
    isSuccess,
    isError,
    error,
  } = useConfirmEmail();
  const {
    mutate: resend,
    isPending: isResending,
    isSuccess: resendSuccess,
  } = useResendConfirmation();

  useEffect(() => {
    if (!token || calledRef.current) return;
    calledRef.current = true;
    confirmEmail({ token });
  }, [token, confirmEmail]);

  const resendForm = useForm<ResendFormValues>({
    resolver: zodResolver(resendSchema),
    defaultValues: { email: "" },
  });

  if (!token) {
    return (
      <div className="flex flex-col items-center gap-3 text-center">
        <XCircle className="size-10 text-destructive" />
        <h1 className="font-heading text-xl font-bold text-foreground">
          Liên kết không hợp lệ
        </h1>
        <p className="text-sm text-muted-foreground">
          Thiếu mã xác thực trong đường dẫn.
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

  if (isPending) {
    return <PageLoader variant="section" label="Đang xác thực email..." />;
  }

  if (isSuccess) {
    return (
      <div className="flex flex-col items-center gap-3 text-center">
        <CheckCircle2 className="size-10 text-success" />
        <h1 className="font-heading text-xl font-bold text-foreground">
          Xác thực thành công
        </h1>
        <p className="text-sm text-muted-foreground">
          Bạn có thể đăng nhập ngay bây giờ.
        </p>
        <Button
          render={<Link to={ROUTES.auth.login} />}
          nativeButton={false}
        >
          Đăng nhập
        </Button>
      </div>
    );
  }

  if (isError) {
    return (
      <div className="flex flex-col items-center gap-4 text-center">
        <XCircle className="size-10 text-destructive" />
        <h1 className="font-heading text-xl font-bold text-foreground">
          Xác thực thất bại
        </h1>
        <p className="text-sm text-muted-foreground">
          {getDisplayMessage(error)}
        </p>

        {resendSuccess ? (
          <p className="cut-sm bg-success/10 px-3 py-2 text-sm text-success">
            Nếu email tồn tại và chưa xác thực, liên kết mới đã được gửi.
          </p>
        ) : (
          <form
            onSubmit={resendForm.handleSubmit((data) => resend(data))}
            className="flex w-full flex-col gap-3"
          >
            <Controller
              name="email"
              control={resendForm.control}
              render={({ field, fieldState }) => (
                <div className="flex flex-col gap-1.5 text-left">
                  <Input
                    {...field}
                    type="email"
                    placeholder="Nhập email để gửi lại liên kết"
                    aria-invalid={fieldState.invalid}
                  />
                  {fieldState.invalid && (
                    <p className="text-xs text-destructive">
                      {fieldState.error?.message}
                    </p>
                  )}
                </div>
              )}
            />
            <Button type="submit" variant="outline" disabled={isResending}>
              {isResending && <Spinner size="sm" decorative className="text-current" />}
              Gửi lại liên kết xác thực
            </Button>
          </form>
        )}

        <Button
          variant="ghost"
          render={<Link to={ROUTES.auth.login} />}
          nativeButton={false}
        >
          Về trang đăng nhập
        </Button>
      </div>
    );
  }

  return null;
};

export default ConfirmEmailPage;