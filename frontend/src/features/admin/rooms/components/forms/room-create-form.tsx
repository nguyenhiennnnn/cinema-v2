import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { FieldGroup } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";

import { useAdminScreenTypes } from "@/features/admin/catalog/hooks/use-admin-screen-type";
import { useScreenTypeQuickCreate } from "@/features/admin/catalog/hooks/use-quick-create";
import { useAdminCreateRoom } from "@/features/admin/rooms/hooks/use-admin-rooms";
import { LOOKUP_PAGE_SIZE, NAME_MAX_LENGTH, ROOM_CREATE_MAX_ROWS, ROOM_CREATE_MAX_COLS } from "@/config/default-constants";
import { FormSearchSelectField } from "@/components/common/form/form-search-select-field";
import { FormActions } from "@/components/common/form/form-actions";
import { useServerFieldErrors } from "@/hooks/use-server-field-errors";

const roomFormSchema = z.object({
  roomName: z.string().min(1, "Vui lòng nhập tên phòng chiếu.").max(NAME_MAX_LENGTH),
  screenTypeId: z.coerce.number().optional(),
  rows: z.coerce.number().int().min(1, "Số hàng phải lớn hơn 0.").max(ROOM_CREATE_MAX_ROWS),
  cols: z.coerce.number().int().min(1, "Số cột phải lớn hơn 0.").max(ROOM_CREATE_MAX_COLS),
});

type RoomFormValues = z.infer<typeof roomFormSchema>;

interface RoomCreateFormProps {
  onSuccess: () => void;
}

export const RoomCreateForm = ({ onSuccess }: RoomCreateFormProps) => {
  const createRoom = useAdminCreateRoom();
  const [screenTypeSearch, setScreenTypeSearch] = useState("");
  const screenTypes = useAdminScreenTypes({ pageSize: LOOKUP_PAGE_SIZE });
  const screenTypeQuickCreate = useScreenTypeQuickCreate();

  const form = useForm<RoomFormValues>({
    resolver: zodResolver(roomFormSchema),
    defaultValues: { roomName: "", rows: 10, cols: 10 },
  });

  const applyServerErrors = useServerFieldErrors(form);

  const onSubmit = (values: RoomFormValues) => {
    createRoom.mutate({
      roomName: values.roomName,
      screenTypeId: values.screenTypeId ?? null,
      rows: values.rows,
      cols: values.cols,
    }, {
      onSuccess: () => onSuccess(),
      onError: applyServerErrors,
    });
  };

  return (
    <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
      <FieldGroup disabled={createRoom.isPending}>
        <FormInputField
          name="roomName"
          control={form.control}
          label="Tên phòng chiếu"
          placeholder="VD: Phòng 01"
        />

        <div className="grid grid-cols-2 gap-4">
          <FormInputField
            name="rows"
            control={form.control}
            label="Số hàng"
            type="number"
            placeholder="VD: 10"
          />
          <FormInputField
            name="cols"
            control={form.control}
            label="Số cột"
            type="number"
            placeholder="VD: 10"
          />
        </div>

        <FormSearchSelectField
          name="screenTypeId"
          control={form.control}
          label="Loại màn hình (không bắt buộc)"
          options={screenTypes.data?.items ?? []}
          getOptionValue={(o) => o.screenTypeId}
          getOptionLabel={(o) => o.name}
          searchValue={screenTypeSearch}
          onSearchChange={setScreenTypeSearch}
          isLoading={screenTypes.isLoading}
          placeholder="Chọn loại màn hình"
          {...screenTypeQuickCreate}
        />
      </FieldGroup>

      <FormActions
        isPending={createRoom.isPending}
        isDirty={form.formState.isDirty}
        submitLabel="Thêm phòng chiếu"
        onCancel={onSuccess}
      />
    </form>
  );
};