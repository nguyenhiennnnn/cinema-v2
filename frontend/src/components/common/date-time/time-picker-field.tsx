import { ClockIcon } from "lucide-react";

import { cn } from "@/lib/utils";
import { InputGroup, InputGroupAddon, InputGroupInput } from "@/components/ui/input-group";

interface TimePickerFieldProps {
  value: string | null | undefined;
  onChange: (value: string | null) => void;
  disabled?: boolean;
  minuteStep?: number;
  className?: string;
  id?: string;
  "aria-invalid"?: boolean;
}

export const TimePickerField = ({
  value,
  onChange,
  disabled,
  minuteStep = 1,
  className,
  id,
  "aria-invalid": ariaInvalid,
}: TimePickerFieldProps) => {
  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    onChange(e.target.value || null);
  };

  return (
    <InputGroup className={cn(className)}>
      <InputGroupAddon>
        <ClockIcon />
      </InputGroupAddon>
      <InputGroupInput
        id={id}
        type="time"
        step={minuteStep * 60}
        value={value ?? ""}
        onChange={handleChange}
        disabled={disabled}
        aria-invalid={ariaInvalid}
      />
    </InputGroup>
  );
};
