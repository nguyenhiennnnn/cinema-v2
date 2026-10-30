import { useEffect, useRef } from "react";
import { Link, useSearchParams } from "react-router";
import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { CheckCircle2, Loader2, XCircle } from "lucide-react";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
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
        <Button variant="outline">
          <Link to={ROUTES.auth.login}>Về trang đăng nhập</Link>
        </Button>
      </div>
    );
  }

  if (isPending) {
    return (
      <div className="flex flex-col items-center gap-3 text-center">
        <Loader2 className="size-10 animate-spin text-primary" />
        <p className="text-sm text-muted-foreground">Đang xác thực email...</p>
      </div>
    );
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
        <Button>
          <Link to={ROUTES.auth.login}>Đăng nhập</Link>
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
              {isResending && <Loader2 className="size-4 animate-spin" />}
              Gửi lại liên kết xác thực
            </Button>
          </form>
        )}

        <Button variant="ghost">
          <Link to={ROUTES.auth.login}>Về trang đăng nhập</Link>
        </Button>
      </div>
    );
  }

  return null;
};

export default ConfirmEmailPage;