import { SelectField } from "@/components/common/select-field";

const DAY_OPTIONS = [
  { value: 7, label: "7 ngày qua" },
  { value: 14, label: "14 ngày qua" },
  { value: 30, label: "30 ngày qua" },
];

interface DashboardDaysFilterProps {
  value: number;
  onChange: (value: number) => void;
}

export const DashboardDaysFilter = ({ value, onChange }: DashboardDaysFilterProps) => {
  return (
    <SelectField
      className="w-40"
      value={value}
      onChange={(v) => onChange(Number(v))}
      options={DAY_OPTIONS}
      getOptionValue={(o) => o.value}
      getOptionLabel={(o) => o.label}
    />
  );
}