import { forwardRef } from "react";
import { QrCode } from "lucide-react";
import { Input } from "@/components/ui/input";

interface CheckinCodeInputProps {
  value: string;
  onChange: (value: string) => void;
  onSubmit: () => void;
  disabled?: boolean;
}

export const CheckinCodeInput = forwardRef<HTMLInputElement, CheckinCodeInputProps>(
  ({ value, onChange, onSubmit, disabled }, ref) => {
    return (
      <div className="relative">
        <QrCode className="absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
        <Input
          ref={ref}
          autoFocus
          disabled={disabled}
          value={value}
          onChange={(e) => onChange(e.target.value)}
          onKeyDown={(e) => {
            if (e.key === "Enter") {
              e.preventDefault();
              onSubmit();
            }
          }}
          placeholder="Quét hoặc nhập mã QR vé"
          className="h-12 pl-9 text-base font-mono tracking-wide"
        />
      </div>
    );
  },
);

CheckinCodeInput.displayName = "CheckinCodeInput";