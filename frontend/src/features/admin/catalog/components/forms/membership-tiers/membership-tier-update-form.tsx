import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { toast } from "@/components/ui/toast";
import { Button } from "@/components/ui/button";
import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";

import { useAdminUpdateMembershipTier } from "../../../hooks/use-admin-membership-tier";
import type { MembershipTierDto } from "../../../types";

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

interface MembershipTierUpdateFormProps {
  tier: MembershipTierDto;
  onSuccess: () => void;
}

export const MembershipTierUpdateForm = ({
  tier,
  onSuccess,
}: MembershipTierUpdateFormProps) => {
  const updateMembershipTier = useAdminUpdateMembershipTier();

  const form = useForm<MembershipTierFormValues>({
    resolver: zodResolver(membershipTierFormSchema),
    defaultValues: {
      tierName: tier.tierName,
      minPoints: tier.minPoints,
      discountPercent: tier.discountPercent,
    },
  });

  const onSubmit = async (values: MembershipTierFormValues) => {
    await updateMembershipTier.mutateAsync({
      id: tier.tierId,
      body: {
        tierName: values.tierName,
        minPoints: values.minPoints,
        discountPercent: values.discountPercent,
      },
    }, {
      onSuccess: () => {
        toast.add({ title: "Đã cập nhật hạng thành viên.", type: "success" });
        onSuccess();
      }
    });
  };

  return (
    <form
      id="membership-tier-update-form"
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
        />

        <FormInputField
          name="discountPercent"
          control={form.control}
          label="Phần trăm giảm giá"
          type="number"
          min={0}
          max={100}
          step={0.5}
        />
      </FieldGroup>

      <div className="flex justify-end gap-2 pt-2">
        <Button
          type="button"
          variant="outline"
          onClick={onSuccess}
          disabled={updateMembershipTier.isPending}
        >
          Hủy
        </Button>
        <Button type="submit" disabled={updateMembershipTier.isPending}>
          {updateMembershipTier.isPending ? "Đang lưu..." : "Lưu thay đổi"}
        </Button>
      </div>
    </form>
  );
};