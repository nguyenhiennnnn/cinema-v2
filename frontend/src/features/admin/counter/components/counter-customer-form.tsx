import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { FormInputField } from "@/components/common/form/form-input-field";

const customerFormSchema = z.object({
  customerName: z.string().min(1, "Vui lòng nhập tên khách hàng."),
  customerPhone: z
    .string()
    .optional()
    .refine((v) => !v || /^0\d{9,10}$/.test(v), "Số điện thoại không hợp lệ."),
});

type CustomerFormValues = z.infer<typeof customerFormSchema>;

interface CounterCustomerFormProps {
  customerName: string;
  onCustomerNameChange: (value: string) => void;
  customerPhone: string;
  onCustomerPhoneChange: (value: string) => void;
}

export const CounterCustomerForm = ({
  customerName,
  onCustomerNameChange,
  customerPhone,
  onCustomerPhoneChange,
}: CounterCustomerFormProps) => {
  const form = useForm<CustomerFormValues>({
    resolver: zodResolver(customerFormSchema),
    defaultValues: {
      customerName,
      customerPhone,
    },
  });

  useEffect(() => {
    const subscription = form.watch((values) => {
      onCustomerNameChange(values.customerName ?? "");
      onCustomerPhoneChange(values.customerPhone ?? "");
    });
    return () => subscription.unsubscribe();
  }, [form, onCustomerNameChange, onCustomerPhoneChange]);

  return (
    <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
      <FormInputField
        name="customerName"
        control={form.control}
        label="Tên khách hàng"
        placeholder="Nguyễn Văn A"
        min={2}
      />
      <FormInputField
        name="customerPhone"
        control={form.control}
        label="Số điện thoại"
        placeholder="09xxxxxxxx"
      />
    </div>
  );
};