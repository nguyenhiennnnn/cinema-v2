import { cn } from "@/lib/utils";
import { PaymentMethod } from "@/types/enum";

const ONLINE_PAYMENT_METHODS = [
  { value: PaymentMethod.VNPay, label: "VNPay" },
  { value: PaymentMethod.Momo, label: "MoMo" },
  { value: PaymentMethod.Stripe, label: "Thẻ quốc tế (Stripe)" },
] as const;

interface BookingPaymentMethodProps {
  value: PaymentMethod;
  onChange: (value: PaymentMethod) => void;
}

export const BookingPaymentMethod = ({ value, onChange }: BookingPaymentMethodProps) => {
  return (
    <div className="flex flex-col gap-3 bg-card p-4">
      <h3 className="font-heading text-sm font-semibold text-foreground">Phương thức thanh toán</h3>
      <div className="grid grid-cols-1 gap-3 sm:grid-cols-3">
        {ONLINE_PAYMENT_METHODS.map((method) => (
          <button
            key={method.value}
            type="button"
            onClick={() => onChange(method.value)}
            className={cn(
              "border px-4 py-3 text-sm font-medium transition-all",
              value === method.value
                ? "border-primary bg-primary/10 text-primary"
                : "border-border bg-card text-foreground hover:bg-muted",
            )}
          >
            {method.label}
          </button>
        ))}
      </div>
    </div>
  );
}
