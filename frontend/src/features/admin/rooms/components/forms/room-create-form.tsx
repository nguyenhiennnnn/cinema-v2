import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { toast } from "@/components/ui/toast";
import { Button } from "@/components/ui/button";
import { Field, FieldError, FieldGroup, FieldLabel } from "@/components/ui/field";
import { FormInputField } from "@/components/common/form/form-input-field";
import { SingleSelectField } from "@/components/common/form/single-select-field";

import { useAdminScreenTypes } from "@/features/admin/catalog/hooks/use-admin-screen-type";
import { useAdminCreateRoom } from "../../hooks/use-admin-rooms";

const roomFormSchema = z.object({
  roomName: z.string().min(1, "Vui lòng nhập tên phòng chiếu.").max(100),
  screenTypeId: z.coerce.number().optional(),
  rows: z.coerce.number().int().min(1, "Số hàng phải lớn hơn 0.").max(50),
  cols: z.coerce.number().int().min(1, "Số cột phải lớn hơn 0.").max(50),
});

type RoomFormValues = z.infer<typeof roomFormSchema>;

interface RoomCreateFormProps {
  onSuccess: () => void;
}

export const RoomCreateForm = ({ onSuccess }: RoomCreateFormProps) => {
  const createRoom = useAdminCreateRoom();
  const [screenTypeSearch, setScreenTypeSearch] = useState("");
  const screenTypes = useAdminScreenTypes();

  const form = useForm<RoomFormValues>({
    resolver: zodResolver(roomFormSchema),
    defaultValues: { roomName: "", rows: 10, cols: 10 },
  });

  const onSubmit = async (values: RoomFormValues) => {
    await createRoom.mutateAsync({
      roomName: values.roomName,
      screenTypeId: values.screenTypeId ?? null,
      rows: values.rows,
      cols: values.cols,
    }, {
      onSuccess: () => {
        toast.add({ title: "Đã thêm phòng chiếu mới.", type: "success" });
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

        <Controller
          name="screenTypeId"
          control={form.control}
          render={({ field, fieldState }) => (
            <Field data-invalid={fieldState.invalid}>
              <FieldLabel>Loại màn hình (không bắt buộc)</FieldLabel>
              <SingleSelectField
                value={field.value}
                onChange={(v) => field.onChange(v)}
                options={screenTypes.data?.items ?? []}
                getOptionValue={(o) => o.screenTypeId}
                getOptionLabel={(o) => o.name}
                searchValue={screenTypeSearch}
                onSearchChange={setScreenTypeSearch}
                isLoading={screenTypes.isLoading}
                placeholder="Chọn loại màn hình"
              />
              {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
            </Field>
          )}
        />
      </FieldGroup>

      <div className="flex justify-end gap-2 pt-2">
        <Button type="button" variant="outline" onClick={onSuccess} disabled={createRoom.isPending}>
          Hủy
        </Button>
        <Button type="submit" disabled={createRoom.isPending}>
          {createRoom.isPending ? "Đang lưu..." : "Thêm phòng chiếu"}
        </Button>
      </div>
    </form>
  );
};