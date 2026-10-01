import { useState } from "react";
import { cn } from "@/lib/utils";

interface ExpandableTextProps {
  text: string;
  className?: string;
}

export const ExpandableText = ({ text, className }: ExpandableTextProps) => {
  const [expanded, setExpanded] = useState(false);

  return (
    <div className={className}>
      <p className={cn("text-sm leading-relaxed text-foreground", !expanded && "line-clamp-4")}>{text}</p>
      {text.length > 200 && (
        <button
          type="button"
          onClick={() => setExpanded((v) => !v)}
          className="mt-1 text-xs font-medium text-primary hover:underline"
        >
          {expanded ? "Thu gọn" : "Xem thêm"}
        </button>
      )}
    </div>
  );
}