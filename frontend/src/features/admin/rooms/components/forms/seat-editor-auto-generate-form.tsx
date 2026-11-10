import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { FieldGroup } from "@/components/ui/field";
import { FormActions } from "@/components/common/form/form-actions";
import { FormInputField } from "@/components/common/form/form-input-field";
import { FormSelectField } from "@/components/common/form/form-select-field";
import { resolveSeatVisualKind } from "@/components/common/seat-map/seat-map-types";
import type { SeatTypeDto } from "@/features/admin/catalog/types";
import type { AutoGenerateOptions } from "@/features/admin/rooms/types/seat-editor";
import { parseAisleCols } from "@/features/admin/rooms/utils/parse-aisle-cols";

const NONE_SEAT_TYPE_ID = 0;

const autoGenerateFormSchema = z.object({
  rows: z.coerce.number().int().min(1, "Tối thiểu 1 hàng").max(52, "Tối đa 52 hàng"),
  cols: z.coerce.number().int().min(1, "Tối thiểu 1 cột").max(60, "Tối đa 60 cột"),
  standardSeatTypeId: z.coerce.number().int(),
  vipSeatTypeId: z.coerce.number().int(),
  vipRowCount: z.coerce.number().int().min(0, "Không hợp lệ"),
  coupleRowCount: z.coerce.number().int().min(0, "Không hợp lệ"),
  aisleColsText: z.string().optional(),
});

type AutoGenerateFormValues = z.infer<typeof autoGenerateFormSchema>;

interface SeatEditorAutoGenerateFormProps {
  rows: number;
  cols: number;
  seatTypes: SeatTypeDto[] | undefined;
  defaultSeatTypeId: number | null;
  onGenerate: (options: AutoGenerateOptions) => void;
  onDone: () => void;
}

export const SeatEditorAutoGenerateForm = ({
  rows,
  cols,
  seatTypes,
  defaultSeatTypeId,
  onGenerate,
  onDone,
}: SeatEditorAutoGenerateFormProps) => {
  const detectedVipId =
    seatTypes?.find((t) => resolveSeatVisualKind(t.name) === "vip")?.seatTypeId ??
    NONE_SEAT_TYPE_ID;

  const form = useForm<AutoGenerateFormValues>({
    resolver: zodResolver(autoGenerateFormSchema),
    defaultValues: {
      rows,
      cols,
      standardSeatTypeId: defaultSeatTypeId ?? NONE_SEAT_TYPE_ID,
      vipSeatTypeId: detectedVipId,
      vipRowCount: 2,
      coupleRowCount: 1,
      aisleColsText: "",
    },
  });

  const standardOptions: SeatTypeDto[] = [
    { seatTypeId: NONE_SEAT_TYPE_ID, name: "— Không đặt —" } as SeatTypeDto,
    ...(seatTypes ?? []),
  ];
  const vipOptions: SeatTypeDto[] = [
    { seatTypeId: NONE_SEAT_TYPE_ID, name: "— Không có hàng VIP —" } as SeatTypeDto,
    ...(seatTypes ?? []),
  ];

  const onSubmit = (values: AutoGenerateFormValues) => {
    const safeRows = Math.min(52, Math.max(1, values.rows || 1));
    const safeCols = Math.min(60, Math.max(1, values.cols || 1));

    onGenerate({
      rows: safeRows,
      cols: safeCols,
      standardSeatTypeId:
        values.standardSeatTypeId === NONE_SEAT_TYPE_ID ? null : values.standardSeatTypeId,
      vipSeatTypeId:
        values.vipSeatTypeId === NONE_SEAT_TYPE_ID ? null : values.vipSeatTypeId,
      vipRowCount: Math.max(0, Math.min(values.vipRowCount, safeRows)),
      coupleRowCount: Math.max(0, Math.min(values.coupleRowCount, safeRows)),
      aisleCols: parseAisleCols(values.aisleColsText).filter((c) => c <= safeCols),
    });
    onDone();
  };

  return (
    <form onSubmit={form.handleSubmit(onSubmit)} className="flex flex-col gap-4">
      <FieldGroup>
        <div className="flex gap-3">
          <div className="flex-1">
            <FormInputField
              name="rows"
              control={form.control}
              label="Số hàng"
              type="number"
              min={1}
              max={52}
            />
          </div>
          <div className="flex-1">
            <FormInputField
              name="cols"
              control={form.control}
              label="Số cột"
              type="number"
              min={1}
              max={60}
            />
          </div>
        </div>

        <FormSelectField
          name="standardSeatTypeId"
          control={form.control}
          label="Loại ghế mặc định"
          options={standardOptions}
          getOptionValue={(o) => o.seatTypeId}
          getOptionLabel={(o) => o.name}
          placeholder="— Không đặt —"
        />

        <div className="flex gap-3">
          <div className="flex-1">
            <FormSelectField
              name="vipSeatTypeId"
              control={form.control}
              label="Loại ghế VIP"
              options={vipOptions}
              getOptionValue={(o) => o.seatTypeId}
              getOptionLabel={(o) => o.name}
              placeholder="— Không có hàng VIP —"
            />
          </div>
          <div className="w-28">
            <FormInputField
              name="vipRowCount"
              control={form.control}
              label="Số hàng VIP"
              type="number"
              min={0}
            />
          </div>
        </div>
        <p className="text-xs text-muted-foreground">
          Hàng VIP được tính từ hàng cuối (gần màn hình nhất) trở lên trên.
        </p>

        <div className="w-28">
          <FormInputField
            name="coupleRowCount"
            control={form.control}
            label="Số hàng ghế đôi (tính từ hàng cuối cùng)"
            type="number"
            min={0}
          />
        </div>

        <div className="flex flex-col gap-1.5">
          <FormInputField
            name="aisleColsText"
            control={form.control}
            label="Cột lối đi (số cột, cách nhau bởi dấu phẩy)"
            placeholder="VD: 4, 9"
          />
          <p className="text-xs text-muted-foreground">
            Các cột này sẽ để trống làm lối đi ở mọi hàng. Có thể bỏ trống.
          </p>
        </div>
      </FieldGroup>

      <FormActions isPending={false} submitLabel="Tạo layout" onCancel={onDone} />
    </form>
  );
};
