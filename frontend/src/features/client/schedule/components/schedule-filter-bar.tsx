import { cn } from "@/lib/utils";

interface ScheduleFilterBarProps {
  formats: string[];
  activeFormat: string | null;
  onFormatChange: (format: string | null) => void;
}

export const ScheduleFilterBar = ({ formats, activeFormat, onFormatChange }: ScheduleFilterBarProps) => {
  if (formats.length === 0) return null;

  return (
    <div className="flex flex-wrap items-center gap-2">
      <button
        type="button"
        onClick={() => onFormatChange(null)}
        className={cn(
          "border px-4 py-1.5 text-sm font-medium transition-colors",
          activeFormat === null
            ? "border-primary bg-primary text-primary-foreground"
            : "border-border bg-secondary text-secondary-foreground hover:border-primary/50 hover:bg-muted",
        )}
      >
        Tất cả định dạng
      </button>
      {formats.map((format) => (
        <button
          key={format}
          type="button"
          onClick={() => onFormatChange(format)}
          className={cn(
            "border px-4 py-1.5 text-sm font-medium transition-colors",
            activeFormat === format
              ? "border-primary bg-primary text-primary-foreground"
              : "border-border bg-secondary text-secondary-foreground hover:border-primary/50 hover:bg-muted",
          )}
        >
          {format}
        </button>
      ))}
    </div>
  );
}