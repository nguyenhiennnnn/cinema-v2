import { cn } from "@/lib/utils";
import { scorePassword } from "../utils/password-score";

const LEVELS = [
  { label: "Rất yếu", className: "bg-destructive" },
  { label: "Yếu", className: "bg-destructive" },
  { label: "Trung bình", className: "bg-warning" },
  { label: "Khá mạnh", className: "bg-accent" },
  { label: "Mạnh", className: "bg-success" },
];

interface PasswordStrengthMeterProps {
  password: string;
}

export const PasswordStrengthMeter = ({ password }: PasswordStrengthMeterProps) => {
  if (!password) return null;
  const score = scorePassword(password);
  const level = LEVELS[score];

  return (
    <div className="mt-1.5">
      <div className="flex gap-1">
        {Array.from({ length: 4 }).map((_, i) => (
          <div key={i} className={cn("h-1.5 flex-1 bg-muted", i < score && level.className)} />
        ))}
      </div>
      <p className="mt-1 text-xs text-muted-foreground">{level.label}</p>
    </div>
  );
}