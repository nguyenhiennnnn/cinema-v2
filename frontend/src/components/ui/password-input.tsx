import * as React from "react";
import { Eye, EyeOff } from "lucide-react";
import { cn } from "@/lib/utils";

export interface PasswordInputProps extends React.ComponentProps<"input"> {
  wrapperClassName?: string;
}

export const PasswordInput = React.forwardRef<HTMLInputElement, PasswordInputProps>(
  ({ className, wrapperClassName, disabled, ...props }, ref) => {
    const [showPassword, setShowPassword] = React.useState(false);

    return (
      <div className={cn("relative flex w-full items-center", wrapperClassName)}>
        <input
          ref={ref}
          type={showPassword ? "text" : "password"}
          data-slot="input"
          disabled={disabled}
          className={cn(
            "h-10 w-full min-w-0 bg-secondary px-3 py-2 pr-10 text-base transition-colors outline-none",
            "border-0 border-b-2 border-input placeholder:text-muted-foreground",
            "focus-visible:border-b-primary",
            "disabled:pointer-events-none disabled:cursor-not-allowed disabled:bg-input/50 disabled:opacity-50",
            "aria-invalid:border-b-destructive",
            "md:text-sm",
            className,
          )}
          {...props}
        />
        <button
          type="button"
          tabIndex={-1}
          disabled={disabled}
          onMouseDown={(e) => e.preventDefault()}
          onClick={() => setShowPassword((prev) => !prev)}
          className={cn(
            "absolute right-2 top-1/2 -translate-y-1/2 flex size-7 items-center justify-center",
            "text-muted-foreground/70 hover:text-foreground transition-colors cursor-pointer outline-none select-none",
            "disabled:pointer-events-none disabled:opacity-50",
          )}
          aria-label={showPassword ? "Ẩn mật khẩu" : "Hiện mật khẩu"}
        >
          {showPassword ? (
            <EyeOff className="size-4" />
          ) : (
            <Eye className="size-4" />
          )}
        </button>
      </div>
    );
  },
);

PasswordInput.displayName = "PasswordInput";
