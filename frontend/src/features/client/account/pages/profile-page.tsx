import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { PERSON_NAME_MAX_LENGTH } from "@/config/default-constants";
import { Save, BadgeCheck } from "lucide-react";

import { Input } from "@/components/ui/input";
import { Field, FieldGroup, FieldLabel } from "@/components/ui/field";
import { Skeleton } from "@/components/ui/skeleton";
import { FormInputField } from "@/components/common/form/form-input-field";
import { SubmitButton } from "@/components/common/form/submit-button";
import { FormSelectField } from "@/components/common/form/form-select-field";
import { FormDatePickerField } from "@/components/common/form/form-date-picker-field";
import { AvatarUpload } from "@/features/client/account/components/avatar-upload";
import { ProfileStatsCard } from "@/features/client/account/components/profile-stats-card";
import { useMyProfile, useUpdateMyProfile } from "@/features/client/account/hooks/use-account";
import { toDateInputValue } from "@/lib/formatters";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";
import { useEffect } from "react";
import { getBusinessTodayDate } from "@/lib/business-time";

const GENDER_OPTIONS = [
  { value: "Male", label: "Nam" },
  { value: "Female", label: "Nữ" },
  { value: "Other", label: "Khác" },
];

const profileFormSchema = z.object({
  fullName: z.string().min(1, "Vui lòng nhập họ tên.").max(PERSON_NAME_MAX_LENGTH, `Họ tên tối đa ${PERSON_NAME_MAX_LENGTH} ký tự.`),
  phone: z.string().max(20, "Số điện thoại tối đa 20 ký tự.").optional().or(z.literal("")),
  dateOfBirth: z.string().nullable().optional(),
  gender: z.string().nullable().optional(),
  avatar: z.string().nullable().optional(),
});

type ProfileFormValues = z.infer<typeof profileFormSchema>;

const AccountProfilePage = () => {
  const { data: profile, isLoading } = useMyProfile();
  const updateProfile = useUpdateMyProfile();

  const form = useForm<ProfileFormValues>({
    resolver: zodResolver(profileFormSchema),
    defaultValues: {
      fullName: "",
      phone: "",
      dateOfBirth: null,
      gender: null,
      avatar: null,
    },
  });

  const applyServerErrors = useServerFieldErrors(form);

  useEffect(() => {
    if (!profile) return;
    form.reset({
      fullName: profile.fullName,
      phone: profile.phone ?? "",
      dateOfBirth: toDateInputValue(profile.dateOfBirth),
      gender: profile.gender ?? null,
      avatar: profile.avatar ?? null,
    });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [profile]);

  const onSubmit = (values: ProfileFormValues) => {
    updateProfile.mutate({
      fullName: values.fullName,
      phone: values.phone || null,
      avatar: values.avatar ?? null,
      dateOfBirth: values.dateOfBirth || null,
      gender: values.gender || null,
    }, { onError: applyServerErrors });
  };

  if (isLoading || !profile) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-40 w-full " />
        <Skeleton className="h-72 w-full " />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <ProfileStatsCard profile={profile} />

      <form onSubmit={form.handleSubmit(onSubmit)} className="bg-card p-5">
        <h2 className="font-heading text-lg font-semibold text-foreground">Thông tin cá nhân</h2>

        <FieldGroup
          disabled={updateProfile.isPending}
          className="mt-5 flex-row items-center gap-4"
        >
          <Controller
            name="avatar"
            control={form.control}
            render={({ field }) => (
              <AvatarUpload
                value={field.value ?? null}
                name={form.watch("fullName")}
                onChange={(url) => field.onChange(url)}
                folder="avatars"
              />
            )}
          />
        </FieldGroup>

        <FieldGroup
          disabled={updateProfile.isPending}
          className="mt-5 grid gap-4 sm:grid-cols-2"
        >
          <FormInputField name="fullName" control={form.control} label="Họ và tên" autoComplete="off" />

          <FormInputField name="phone" control={form.control} label="Số điện thoại" autoComplete="off" />

          <FormDatePickerField
            name="dateOfBirth"
            control={form.control}
            label="Ngày sinh"
            placeholder="Chọn ngày sinh"
            captionLayout="dropdown"
            fromYear={getBusinessTodayDate().getFullYear() - 100}
            toYear={getBusinessTodayDate().getFullYear()}
            emptyValue={null}
          />

          <FormSelectField
            name="gender"
            control={form.control}
            label="Giới tính"
            placeholder="Chọn giới tính"
            options={GENDER_OPTIONS}
            getOptionValue={(o) => o.value}
            getOptionLabel={(o) => o.label}
          />

          <Field className="sm:col-span-2">
            <FieldLabel>Email</FieldLabel>
            <div className="flex items-center gap-2">
              <Input value={profile.email} disabled className="flex-1" />
              <span className="flex items-center gap-1 cut-xs bg-primary/10 px-2 py-1 text-xs font-medium text-primary">
                <BadgeCheck className="size-3.5" />
                Đã xác thực
              </span>
            </div>
          </Field>
        </FieldGroup>

        <div className="mt-6 flex justify-end">
          <SubmitButton
            isPending={updateProfile.isPending}
            label="Lưu thay đổi"
            icon={<Save className="size-4" />}
          />
        </div>
      </form>
    </div>
  );
};

export default AccountProfilePage;