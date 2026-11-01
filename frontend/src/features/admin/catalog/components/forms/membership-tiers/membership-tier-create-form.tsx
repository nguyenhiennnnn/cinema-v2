import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";

import { useAdminCreateMembershipTier } from "../../../hooks/use-admin-membership-tier";

const membershipTierFormSchema = z.object({
  tierName: z
    .string()
    .min(1, "Vui lòng nhập tên hạng.")
    .max(100, "Tên hạng tối đa 100 ký tự."),
  minPoints: z.coerce
    .number({ message: "Điểm tối thiểu phải là số." })
    .int("Điểm tối thiểu phải là số nguyên.")
    .min(0, "Điểm tối thiểu không được âm."),
  discountPercent: z.coerce
    .number({ message: "Phần trăm giảm giá phải là số." })
    .min(0, "Phần trăm giảm giá không được âm.")
    .max(100, "Phần trăm giảm giá tối đa 100."),
});

type MembershipTierFormValues = z.infer<typeof membershipTierFormSchema>;

interface MembershipTierCreateFormProps {
  onSuccess: () => void;
}

export const MembershipTierCreateForm = ({
  onSuccess,
}: MembershipTierCreateFormProps) => {
  const createMembershipTier = useAdminCreateMembershipTier();

  const form = useForm<MembershipTierFormValues>({
    resolver: zodResolver(membershipTierFormSchema),
    defaultValues: { tierName: "", minPoints: 0, discountPercent: 0 },
  });

  const onSubmit = async (values: MembershipTierFormValues) => {
    await createMembershipTier.mutateAsync({
      tierName: values.tierName,
      minPoints: values.minPoints,
      discountPercent: values.discountPercent,
    }, {
      onSuccess: () => {
        onSuccess();
      }
    });
  };

  return (
    <form
      id="membership-tier-create-form"
      onSubmit={form.handleSubmit(onSubmit)}
      className="space-y-4"
    >
      <FieldGroup>
        <FormInputField
          name="tierName"
          control={form.control}
          label="Tên hạng"
          placeholder="Bạc, Vàng, Kim Cương..."
          autoFocus
          autoComplete="off"
        />

        <FormInputField
          name="minPoints"
          control={form.control}
          label="Điểm tối thiểu"
          type="number"
          min={0}
          step={1}
          placeholder="0"
        />

        <FormInputField
          name="discountPercent"
          control={form.control}
          label="Phần trăm giảm giá"
          type="number"
          min={0}
          max={100}
          step={0.5}
          placeholder="0"
        />
      </FieldGroup>

      <div className="flex justify-end gap-2 pt-2">
        <Button
          type="button"
          variant="outline"
          onClick={onSuccess}
          disabled={createMembershipTier.isPending}
        >
          Hủy
        </Button>
        <Button type="submit" disabled={createMembershipTier.isPending}>
          {createMembershipTier.isPending ? "Đang lưu..." : "Thêm hạng"}
        </Button>
      </div>
    </form>
  );
};