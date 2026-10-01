import { cn } from "@/lib/utils";

function scorePassword(password: string) {
  let score = 0;
  if (password.length >= 8) score++;
  if (/[a-z]/.test(password) && /[A-Z]/.test(password)) score++;
  if (/\d/.test(password)) score++;
  if (/[^a-zA-Z0-9]/.test(password)) score++;
  return score;
}

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
          <div key={i} className={cn("h-1.5 flex-1 rounded-full bg-muted", i < score && level.className)} />
        ))}
      </div>
      <p className="mt-1 text-xs text-muted-foreground">{level.label}</p>
    </div>
  );
}