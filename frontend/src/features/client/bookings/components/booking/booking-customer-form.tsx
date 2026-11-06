import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { isValidVnPhone } from "@/lib/validators";

interface BookingCustomerFormProps {
  customerName: string;
  onCustomerNameChange: (value: string) => void;
  customerPhone: string;
  onCustomerPhoneChange: (value: string) => void;
  isAuthenticated: boolean;
}

export const BookingCustomerForm = ({
  customerName,
  onCustomerNameChange,
  customerPhone,
  onCustomerPhoneChange,
  isAuthenticated,
}: BookingCustomerFormProps) => {
  const phoneInvalid = customerPhone.trim() !== "" && !isValidVnPhone(customerPhone.trim());

  return (
    <div className="flex flex-col gap-3 bg-card p-4">
      <h3 className="font-heading text-sm font-semibold text-foreground">Thông tin nhận vé</h3>
      <div className="flex flex-col gap-1.5">
        <Label htmlFor="booking-customer-name">
          Họ và tên <span className="text-destructive">*</span>
        </Label>
        <Input
          id="booking-customer-name"
          value={customerName}
          onChange={(e) => onCustomerNameChange(e.target.value)}
          disabled={isAuthenticated}
          placeholder="Nguyễn Văn A"
        />
      </div>
      <div className="flex flex-col gap-1.5">
        <Label htmlFor="booking-customer-phone">
          Số điện thoại <span className="text-destructive">*</span>
        </Label>
        <Input
          id="booking-customer-phone"
          value={customerPhone}
          onChange={(e) => onCustomerPhoneChange(e.target.value)}
          type="tel"
          inputMode="tel"
          autoComplete="tel"
          aria-invalid={phoneInvalid}
          aria-describedby={phoneInvalid ? "booking-customer-phone-error" : undefined}
          placeholder="09xxxxxxxx"
        />
        {phoneInvalid && (
          <p id="booking-customer-phone-error" className="text-xs text-destructive">
            Số điện thoại không hợp lệ.
          </p>
        )}
      </div>
    </div>
  );
}
