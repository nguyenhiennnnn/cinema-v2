import { DatePickerField } from "@/components/common/date-time/date-picker-field";
import { SelectField } from "@/components/common/select-field";
import { Button } from "@/components/ui/button";
import { RotateCcw } from "lucide-react";

const GROUP_BY_OPTIONS = [
  { value: "day", label: "Theo ngày" },
  { value: "week", label: "Theo tuần" },
  { value: "month", label: "Theo tháng" },
];

interface ReportsFilterBarProps {
  fromDate: string;
  toDate: string;
  groupBy: string;
  onFromDateChange: (value: string | null) => void;
  onToDateChange: (value: string | null) => void;
  onGroupByChange: (value: string) => void;
  onReset: () => void;
}

export const ReportsFilterBar = ({
  fromDate,
  toDate,
  groupBy,
  onFromDateChange,
  onToDateChange,
  onGroupByChange,
  onReset,
}: ReportsFilterBarProps) => {
  return (
    <div className="flex flex-wrap items-end gap-3 bg-card p-3">
      <div className="flex flex-col gap-1.5">
        <label className="text-sm font-medium text-foreground leading-none">
          Từ ngày
        </label>
        <DatePickerField
          value={fromDate}
          onChange={onFromDateChange}
          className="w-40"
          clearable={false}
        />
      </div>

      <div className="flex flex-col gap-1.5">
        <label className="text-sm font-medium text-foreground leading-none">
          Đến ngày
        </label>
        <DatePickerField
          value={toDate}
          onChange={onToDateChange}
          className="w-40"
          clearable={false}
        />
      </div>

      <div className="flex flex-col gap-1.5">
        <label className="text-sm font-medium text-foreground leading-none">
          Nhóm theo
        </label>
        <SelectField
          className="w-36"
          value={groupBy}
          onChange={(v) => onGroupByChange(v)}
          options={GROUP_BY_OPTIONS}
          getOptionValue={(o) => o.value}
          getOptionLabel={(o) => o.label}
        />
      </div>

      <div className="flex flex-col gap-1.5">
        <label className="text-sm font-medium leading-none invisible select-none">
          &nbsp;
        </label>
        <Button
          variant="outline"
          onClick={onReset}
          className="gap-2"
        >
          <RotateCcw className="size-4" />
          <span>Đặt lại</span>
        </Button>
      </div>
    </div>
  );
};