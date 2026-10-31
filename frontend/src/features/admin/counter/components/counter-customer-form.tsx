import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { FormInputField } from "@/components/common/form/form-input-field";
import { CustomerSearchCombobox } from "./customer-search-combobox";
import type { CustomerLookupItemDto } from "@/features/admin/users/types";

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
  userId: number | null;
  onUserIdChange: (id: number | null) => void;
}

export const CounterCustomerForm = ({
  customerName,
  onCustomerNameChange,
  customerPhone,
  onCustomerPhoneChange,
  userId,
  onUserIdChange,
}: CounterCustomerFormProps) => {
  const [selectedCustomer, setSelectedCustomer] =
    useState<CustomerLookupItemDto | null>(null);

  const form = useForm<CustomerFormValues>({
    resolver: zodResolver(customerFormSchema),
    defaultValues: {
      customerName,
      customerPhone,
    },
  });

  const handleCustomerSelect = (customer: CustomerLookupItemDto | null) => {
    setSelectedCustomer(customer);
    if (customer) {
      form.setValue("customerName", customer.fullName, { shouldValidate: true });
      form.setValue("customerPhone", "", { shouldValidate: false });
      onCustomerNameChange(customer.fullName);
      onCustomerPhoneChange("");
      onUserIdChange(customer.userId);
    } else {
      form.setValue("customerName", "", { shouldValidate: false });
      form.setValue("customerPhone", "", { shouldValidate: false });
      onCustomerNameChange("");
      onCustomerPhoneChange("");
      onUserIdChange(null);
    }
  };

  useEffect(() => {
    if (selectedCustomer) return;
    const subscription = form.watch((values) => {
      onCustomerNameChange(values.customerName ?? "");
      onCustomerPhoneChange(values.customerPhone ?? "");
    });
    return () => subscription.unsubscribe();
  }, [form, onCustomerNameChange, onCustomerPhoneChange, selectedCustomer]);

  const isLinked = selectedCustomer !== null;

  return (
    <div className="flex flex-col gap-4">
      <CustomerSearchCombobox
        selected={selectedCustomer}
        onSelect={handleCustomerSelect}
      />

      {!isLinked && (
        <div className="flex items-center gap-2">
          <div className="h-px flex-1 bg-border" />
          <span className="text-xs text-muted-foreground">hoặc nhập thủ công</span>
          <div className="h-px flex-1 bg-border" />
        </div>
      )}

      <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
        <FormInputField
          name="customerName"
          control={form.control}
          label={isLinked ? "Tên khách hàng (từ tài khoản)" : "Tên khách hàng"}
          placeholder="Nguyễn Văn A"
          min={2}
          disabled={isLinked}
        />
        <FormInputField
          name="customerPhone"
          control={form.control}
          label={
            isLinked
              ? `Số điện thoại${selectedCustomer?.phoneLast4 ? ` (****${selectedCustomer.phoneLast4})` : ""}`
              : "Số điện thoại"
          }
          placeholder={isLinked ? "Lấy từ tài khoản" : "09xxxxxxxx"}
          disabled={isLinked}
        />
      </div>

      {isLinked && selectedCustomer?.discountPercent > 0 && (
        <p className="text-xs text-success">
          Khách hàng hạng{" "}
          <span className="font-medium">{selectedCustomer.tierName}</span> —
          giảm{" "}
          <span className="font-medium t-numeral">
            {selectedCustomer.discountPercent}%
          </span>{" "}
          sẽ được áp dụng.
        </p>
      )}

      <input type="hidden" value={userId ?? ""} />
    </div>
  );
};
