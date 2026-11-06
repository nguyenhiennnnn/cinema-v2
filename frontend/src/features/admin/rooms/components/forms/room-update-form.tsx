import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";

import { useAdminScreenTypes } from "@/features/admin/catalog/hooks/use-admin-screen-type";
import { useAdminUpdateRoom } from "../../hooks/use-admin-rooms";
import type { AdminRoomDto } from "../../types";
import { FormSearchSelectField } from "@/components/common/form/form-search-select-field";
import { FormSwitchField } from "@/components/common/form/form-switch-field";

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
  const screenTypes = useAdminScreenTypes();

  const form = useForm<RoomFormValues>({
    resolver: zodResolver(roomFormSchema),
    defaultValues: {
      roomName: room.roomName,
      screenTypeId: Number(room.screenTypeId),
      isActive: room.isActive,
    },
  });

  const onSubmit = async (values: RoomFormValues) => {
    await updateRoom.mutateAsync({
      id: room.roomId,
      body: {
        roomName: values.roomName,
        screenTypeId: values.screenTypeId ?? null,
        isActive: values.isActive,
      },
    }, {
      onSuccess: () => {
        onSuccess();
      }
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
        />

        <FormSwitchField name="isActive" control={form.control} label="Đang hoạt động" />
      </FieldGroup>

      <div className="flex justify-end gap-2 pt-2">
        <Button type="button" variant="outline" onClick={onSuccess} disabled={updateRoom.isPending}>
          Hủy
        </Button>
        <Button type="submit" disabled={updateRoom.isPending}>
          {updateRoom.isPending ? "Đang lưu..." : "Lưu thay đổi"}
        </Button>
      </div>
    </form>
  );
};