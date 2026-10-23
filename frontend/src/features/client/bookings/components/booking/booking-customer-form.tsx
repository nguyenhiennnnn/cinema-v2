import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

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
  return (
    <div className="flex flex-col gap-3 border border-border bg-card p-4">
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
          placeholder="09xxxxxxxx"
        />
      </div>
    </div>
  );
}
