import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";

import { useAdminScreenTypes } from "@/features/admin/catalog/hooks/use-admin-screen-type";
import { useScreenTypeQuickCreate } from "@/features/admin/catalog/hooks/use-quick-create";
import { useAdminUpdateRoom } from "../../hooks/use-admin-rooms";
import { LOOKUP_PAGE_SIZE } from "@/config/default-constants";
import type { AdminRoomDto } from "../../types";
import { FormSearchSelectField } from "@/components/common/form/form-search-select-field";
import { FormSwitchField } from "@/components/common/form/form-switch-field";
import { FormActions } from "@/components/common/form/form-actions";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";

const roomFormSchema = z.object({
  roomName: z.string().min(1, "Vui lòng nhập tên phòng chiếu.").max(100),
  screenTypeId: z.coerce.number().optional(),
  isActive: z.boolean(),
});

type RoomFormValues = z.infer<typeof roomFormSchema>;

interface RoomUpdateFormProps {
  room: AdminRoomDto;
  onSuccess: () => void;
}

export const RoomUpdateForm = ({ room, onSuccess }: RoomUpdateFormProps) => {
  const updateRoom = useAdminUpdateRoom();
  const [screenTypeSearch, setScreenTypeSearch] = useState("");
  const screenTypes = useAdminScreenTypes({ pageSize: LOOKUP_PAGE_SIZE });
  const screenTypeQuickCreate = useScreenTypeQuickCreate();

  const form = useForm<RoomFormValues>({
    resolver: zodResolver(roomFormSchema),
    defaultValues: {
      roomName: room.roomName,
      screenTypeId: room.screenTypeId ? Number(room.screenTypeId) : undefined,
      isActive: room.isActive,
    },
  });

  const applyServerErrors = useServerFieldErrors(form);

  const onSubmit = (values: RoomFormValues) => {
    updateRoom.mutate({
      id: room.roomId,
      body: {
        roomName: values.roomName,
        screenTypeId: values.screenTypeId ?? null,
        isActive: values.isActive,
      },
    }, {
      onSuccess: () => onSuccess(),
      onError: applyServerErrors,
    });
  };

  return (
    <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
      <FieldGroup>
        <FormInputField
          name="roomName"
          control={form.control}
          label="Tên phòng chiếu"
          placeholder="VD: Phòng 01"
        />

        <FormSearchSelectField
          name="screenTypeId"
          control={form.control}
          label="Loại màn hình"
          options={screenTypes.data?.items ?? []}
          getOptionValue={(o) => o.screenTypeId}
          getOptionLabel={(o) => o.name}
          selectedLabel={room.screenTypeName ?? undefined}
          searchValue={screenTypeSearch}
          onSearchChange={setScreenTypeSearch}
          isLoading={screenTypes.isLoading}
          placeholder="Chọn loại màn hình"
          {...screenTypeQuickCreate}
        />

        <FormSwitchField name="isActive" control={form.control} label="Đang hoạt động" />
      </FieldGroup>

      <FormActions
        isPending={updateRoom.isPending}
        submitLabel="Lưu thay đổi"
        onCancel={onSuccess}
      />
    </form>
  );
};