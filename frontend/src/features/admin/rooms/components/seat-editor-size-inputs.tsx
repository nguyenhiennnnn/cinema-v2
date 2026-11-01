import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

interface SeatEditorSizeInputsProps {
  rows: number;
  cols: number;
  disabled: boolean;
  onResize: (rows: number, cols: number) => void;
}

const toPositiveInt = (value: string) => Math.max(1, Number(value) || 1);

export const SeatEditorSizeInputs = ({ rows, cols, disabled, onResize }: SeatEditorSizeInputsProps) => (
  <>
    <div className="flex flex-col gap-1.5">
      <Label htmlFor="rows-input">Số hàng</Label>
      <Input
        id="rows-input"
        type="number"
        min={1}
        max={52}
        value={rows}
        disabled={disabled}
        onChange={(e) => onResize(toPositiveInt(e.target.value), cols)}
        className="w-24"
      />
    </div>
    <div className="flex flex-col gap-1.5">
      <Label htmlFor="cols-input">Số cột</Label>
      <Input
        id="cols-input"
        type="number"
        min={1}
        max={60}
        value={cols}
        disabled={disabled}
        onChange={(e) => onResize(rows, toPositiveInt(e.target.value))}
        className="w-24"
      />
    </div>
  </>
);
